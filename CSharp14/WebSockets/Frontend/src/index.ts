import './styles.css';

declare const __SERVICE_BASE__: string;

let websocket: WebSocket | null = null;

const serverUrlInput = document.getElementById('serverUrl') as HTMLInputElement;
const connectBtn = document.getElementById('connectBtn') as HTMLButtonElement;
const disconnectBtn = document.getElementById('disconnectBtn') as HTMLButtonElement;
const messageInput = document.getElementById('messageInput') as HTMLInputElement;
const sendBtn = document.getElementById('sendBtn') as HTMLButtonElement;
const statusDiv = document.getElementById('status') as HTMLDivElement;
const messagesDiv = document.getElementById('messages') as HTMLDivElement;

// Set default server URL
const serviceBase = __SERVICE_BASE__.replace(/^https?:\/\//, '');
serverUrlInput.value = `wss://${serviceBase}/ws`;

function addMessage(message: string, type: 'sent' | 'received' | 'system') {
    const messageEl = document.createElement('div');
    messageEl.className = `message ${type}`;
    messageEl.textContent = `[${new Date().toLocaleTimeString()}] ${message}`;
    messagesDiv.appendChild(messageEl);
    messagesDiv.scrollTop = messagesDiv.scrollHeight;
}

function updateStatus(status: string, connected: boolean) {
    statusDiv.textContent = status;
    statusDiv.className = connected ? 'status connected' : 'status';
    connectBtn.disabled = connected;
    disconnectBtn.disabled = !connected;
    messageInput.disabled = !connected;
    sendBtn.disabled = !connected;
}

connectBtn.addEventListener('click', () => {
    const url = serverUrlInput.value;
    if (!url) {
        addMessage('Please enter a server URL', 'system');
        return;
    }

    try {
        websocket = new WebSocket(url);

        websocket.onopen = () => {
            updateStatus('Connected', true);
            addMessage('Connected to server', 'system');
        };

        websocket.onmessage = (event) => {
            addMessage(`Received: ${event.data}`, 'received');
        };

        websocket.onerror = (error) => {
            addMessage('WebSocket error occurred', 'system');
            console.error('WebSocket error:', error);
        };

        websocket.onclose = () => {
            updateStatus('Disconnected', false);
            addMessage('Disconnected from server', 'system');
            websocket = null;
        };
    } catch (error) {
        addMessage(`Connection error: ${error}`, 'system');
    }
});

disconnectBtn.addEventListener('click', () => {
    if (websocket) {
        websocket.close();
    }
});

sendBtn.addEventListener('click', () => {
    sendMessage();
});

messageInput.addEventListener('keypress', (event) => {
    if (event.key === 'Enter') {
        sendMessage();
    }
});

function sendMessage() {
    if (websocket && websocket.readyState === WebSocket.OPEN) {
        const message = messageInput.value;
        if (message) {
            // Send message with terminating newline
            websocket.send(message + '\n');
            addMessage(`Sent: ${message}`, 'sent');
            messageInput.value = '';
        }
    }
}