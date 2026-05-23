
import { marked } from "https://cdn.jsdelivr.net/npm/marked/lib/marked.esm.js"

const baseUrl = "http://localhost:3000";

const longRunningRegular = document.querySelector("#long-running-regular");
const longRunningRegularSpinner = document.querySelector("#long-running-regular-spinner");
const longRunningRegularOutput = document.querySelector("#long-running-regular-output");

const longRunningStreaming = document.querySelector("#long-running-streaming");
const longRunningStreamingSpinner = document.querySelector("#long-running-streaming-spinner");
const longRunningStreamingOutput = document.querySelector("#long-running-streaming-output");

const sseDataOnly = document.querySelector("#sse-data-only");
const sseDataOnlySpinner = document.querySelector("#sse-data-only-spinner");
const sseDataOnlyOutput = document.querySelector("#sse-data-only-output");

const sseCustomEvents = document.querySelector("#sse-custom-events");
const sseCustomEventsSpinner = document.querySelector("#sse-custom-events-spinner");
const sseCustomEventsOutput = document.querySelector("#sse-custom-events-output");

const sseCustomEventsId = document.querySelector("#sse-custom-events-id");
const sseCustomEventsIdSpinner = document.querySelector("#sse-custom-events-id-spinner");
const sseCustomEventsIdOutput = document.querySelector("#sse-custom-events-id-output");

const openai = document.querySelector("#openai");
const openaiOutput = document.querySelector("#openai-output");

longRunningRegular.addEventListener("click", async () => {
  longRunningRegularSpinner.style.display = "block";
  longRunningRegularOutput.innerHTML = "";

  const startTime = performance.now();
  const response = await fetch(`${baseUrl}/long-running/regular`, { method: "POST", });
  const endTime = performance.now();

  const data = await response.text();
  longRunningRegularOutput.innerHTML = `${data}\n(Duration: ${endTime - startTime}ms)`;
  longRunningRegularSpinner.style.display = "";
});

longRunningStreaming.addEventListener("click", async () => {
  longRunningStreamingSpinner.style.display = "block";
  longRunningStreamingOutput.innerHTML = "";

  const startTime = performance.now();
  const response = await fetch(`${baseUrl}/long-running/streaming`, { method: "POST", });
  const endTime = performance.now();
  longRunningStreamingOutput.innerHTML = `(Duration: ${endTime - startTime}ms)\n`;

  const reader = response.body.getReader();
  while (true) {
    const { done, value } = await reader.read();
    if (done) break;
    longRunningStreamingOutput.innerHTML += new TextDecoder().decode(value);
  }

  longRunningStreamingSpinner.style.display = "";
});

sseDataOnly.addEventListener("click", async () => {
  sseDataOnlySpinner.style.display = "block";
  sseDataOnlyOutput.innerHTML = "";

  const eventSource = new EventSource(`${baseUrl}/sse/data-only`);
  eventSource.onmessage = (event) => {
    // Close the connection when the server sends and empty message
    if (!event.data) {
      eventSource.close();
      sseDataOnlySpinner.style.display = "";
    }

    sseDataOnlyOutput.innerHTML += `${event.data}\n`;
  };
  eventSource.onerror = () => { sseDataOnlyOutput.innerHTML += `Error in SSE\n`; };
  eventSource.onopen = () => { sseDataOnlyOutput.innerHTML += `SSE connection opened\n`; }
});

sseCustomEvents.addEventListener("click", async () => {
  sseCustomEventsSpinner.style.display = "block";
  sseCustomEventsOutput.innerHTML = "";

  const eventSource = new EventSource(`${baseUrl}/sse/custom-events`);

  // Note that we are using the addEventListener method here instead of onmessage.
  // With this, we can add custom event listeners for different event types.
  eventSource.addEventListener("even", (event) => {
    sseCustomEventsOutput.innerHTML += `EVEN: ${JSON.parse(event.data).value}\n`;
  });
  eventSource.addEventListener("odd", (event) => {
    sseCustomEventsOutput.innerHTML += `ODD: ${JSON.parse(event.data).value}\n`;
  });
  eventSource.addEventListener("eom", () => {
    eventSource.close();
    sseCustomEventsSpinner.style.display = "";
  });
});

sseCustomEventsId.addEventListener("click", async () => {
  sseCustomEventsIdSpinner.style.display = "block";
  sseCustomEventsIdOutput.innerHTML = "";

  const eventSource = new EventSource(`${baseUrl}/sse/custom-events-with-id`);

  eventSource.addEventListener("even", (event) => {
    sseCustomEventsIdOutput.innerHTML += `EVEN: ${JSON.parse(event.data).value}\n`;
  });
  eventSource.addEventListener("odd", (event) => {
    sseCustomEventsIdOutput.innerHTML += `ODD: ${JSON.parse(event.data).value}\n`;
  });
  eventSource.addEventListener("eom", () => {
    console.log("da");
    eventSource.close();
    sseCustomEventsIdSpinner.style.display = "";
  });
});

openai.addEventListener("click", async () => {
  let markdown = "";
  openaiOutput.innerHTML = "";
  openaiOutput.style.display = "block";

  const eventSource = new EventSource(`${baseUrl}/openai/why-wad`);
  eventSource.onmessage = (event) => {
    if (!event.data) {
      eventSource.close();
      return;
    }

    markdown += `${JSON.parse(event.data)}`;
    openaiOutput.innerHTML = marked(markdown);

    // Scroll to the bottom of the document
    window.scrollTo(0, document.body.scrollHeight);
  };
});
