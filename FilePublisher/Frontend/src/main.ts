import './style.css';
declare const hljs: any;

interface FileListResponse {
  files: string[];
}

interface FileContentResponse {
  content: string;
}

class FileViewer {
  private sessionInput!: HTMLInputElement;
  private fileSelect!: HTMLSelectElement;
  private contentDisplay!: HTMLPreElement;
  private currentSessionId: string = '';
  private currentETag: string = '';
  private refreshInterval: number | null = null;

  constructor() {
    this.setupUI();
    this.setupEventListeners();
    
    // Load saved session ID if it exists
    const savedSessionId = localStorage.getItem('lastSessionId');
    if (savedSessionId) {
      this.sessionInput.value = savedSessionId;
      this.loadFiles();
    }
  }

  private setupUI() {
    const app = document.querySelector<HTMLDivElement>('#app')!;
    app.innerHTML = `
      <div class="title-bar">
        <div class="controls">
          <input type="text" id="session-id" placeholder="Enter session ID" />
          <button id="load-files">Load Files</button>
          <select id="file-select"></select>
          <button id="refresh-file">Refresh File</button>
        </div>
      </div>
      <div class="content-area">
        <pre id="content-display"><code class="language-typescript"></code></pre>
      </div>
    `;

    this.sessionInput =
      document.querySelector<HTMLInputElement>('#session-id')!;
    this.fileSelect =
      document.querySelector<HTMLSelectElement>('#file-select')!;
    this.contentDisplay =
      document.querySelector<HTMLPreElement>('#content-display')!;
  }

  private setupEventListeners() {
    document
      .querySelector('#load-files')!
      .addEventListener('click', () => this.loadFiles());
    document
      .querySelector('#refresh-file')!
      .addEventListener('click', () => this.loadSelectedFile());
    this.fileSelect.addEventListener('change', () => {
      this.loadSelectedFile();
      this.setupAutoRefresh();
    });
  }

  private setupAutoRefresh() {
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }
    
    this.refreshInterval = setInterval(() => {
      this.loadSelectedFile();
    }, 30000); // 30 seconds
  }

  private async loadFiles() {
    this.currentSessionId = this.sessionInput.value.trim();
    if (!this.currentSessionId) return;

    // Reset ETag when loading new files
    this.currentETag = '';
    
    if (this.refreshInterval) {
      clearInterval(this.refreshInterval);
    }

    // Remember the currently selected file before loading new files
    const previouslySelectedFile = this.fileSelect.value;

    try {
      const response = await fetch(
        `sessions/${this.currentSessionId}`
      );
      const data: FileListResponse = await response.json();

      // Store session ID in localStorage after successful file list retrieval
      localStorage.setItem('lastSessionId', this.currentSessionId);

      this.fileSelect.innerHTML = data.files
        .map((file) => `<option value="${file}">${file}</option>`)
        .join('');

      if (data.files.length > 0) {
        // Check if previously selected file still exists in the new file list
        if (previouslySelectedFile && data.files.includes(previouslySelectedFile)) {
          this.fileSelect.value = previouslySelectedFile;
        }
        this.loadSelectedFile();
        this.setupAutoRefresh();
      }
    } catch (error) {
      console.error('Error loading files:', error);
      this.contentDisplay.textContent = 'Error loading files';
    }
  }

  private async loadSelectedFile() {
    const selectedFile = this.fileSelect.value;
    if (!selectedFile || !this.currentSessionId) return;

    try {
      const headers: HeadersInit = {};
      if (this.currentETag) {
        headers['If-None-Match'] = this.currentETag;
      }

      const response = await fetch(
        `sessions/${
          this.currentSessionId
        }/file?file=${encodeURIComponent(selectedFile)}`,
        { headers }
      );

      if (response.status === 304) {
        // Content hasn't changed
        return;
      }

      const newETag = response.headers.get('ETag');
      if (newETag) {
        this.currentETag = newETag;
      }

      const data: FileContentResponse = await response.json();

      const code = this.contentDisplay.querySelector('code')!;
      code.textContent = data.content;
      code.removeAttribute('data-highlighted');
      hljs.highlightAll();
      hljs.initLineNumbersOnLoad();
    } catch (error) {
      console.error('Error loading file content:', error);
      this.contentDisplay.textContent = 'Error loading file content';
    }
  }
}

// Initialize the application
new FileViewer();
