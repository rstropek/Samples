// ---------------------------------------------------------------------------
// A *minimal, hand-written* AG-UI protocol client.
//
// The point of this file is to show what AG-UI actually is, without hiding it
// behind an SDK. AG-UI is just:
//
//   1. An HTTP POST with a JSON "RunAgentInput" body (thread id, run id, the
//      conversation so far).
//   2. A Server-Sent-Events (SSE) response stream where each `data:` line is a
//      JSON event with a `type` field (RUN_STARTED, TEXT_MESSAGE_CONTENT,
//      TOOL_CALL_START, ...).
//
// We parse that stream by hand and call back into the UI as events arrive.
// ---------------------------------------------------------------------------

export type Role = 'user' | 'assistant' | 'system' | 'tool'

/** A chat message as sent to the agent in the request body. */
export interface AgentMessage {
  id: string
  role: Role
  content: string
}

/** Callbacks invoked as protocol events stream in. All are optional. */
export interface RunHandlers {
  onRunStarted?: () => void
  onTextStart?: (messageId: string) => void
  onTextContent?: (messageId: string, delta: string) => void
  onTextEnd?: (messageId: string) => void
  onToolCallStart?: (toolCallId: string, toolName: string) => void
  onToolCallArgs?: (toolCallId: string, argsDelta: string) => void
  onToolCallEnd?: (toolCallId: string) => void
  onToolCallResult?: (toolCallId: string, result: string) => void
  onRunFinished?: () => void
  onError?: (message: string) => void
}

/** Shape of a decoded AG-UI event. We only read the fields we care about. */
interface AgentEvent {
  type: string
  messageId?: string
  delta?: string
  toolCallId?: string
  toolCallName?: string
  toolName?: string
  content?: string
  message?: string
}

function newId(): string {
  return crypto.randomUUID()
}

/**
 * Run one agent turn: POST the conversation and stream the response, dispatching
 * each AG-UI event to the supplied handlers.
 */
export async function runAgent(
  agentBaseUrl: string,
  threadId: string,
  messages: AgentMessage[],
  handlers: RunHandlers,
  signal?: AbortSignal,
): Promise<void> {
  const body = {
    threadId,
    runId: newId(),
    messages,
    tools: [],
    context: [],
    state: {},
    forwardedProps: {},
  }

  const response = await fetch(`${agentBaseUrl.replace(/\/$/, '')}/ag-ui`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Accept: 'text/event-stream',
    },
    body: JSON.stringify(body),
    signal,
  })

  if (!response.ok || !response.body) {
    throw new Error(`Agent request failed: ${response.status} ${response.statusText}`)
  }

  // Ollama-backed responses do not always include a messageId / toolCallId on
  // every event, so we remember the "current" one and synthesize ids when needed.
  let currentMessageId: string | null = null
  let currentToolCallId: string | null = null

  const reader = response.body.getReader()
  const decoder = new TextDecoder()
  let buffer = ''

  // SSE frames are separated by a blank line. We accumulate bytes, split on the
  // frame boundary, and parse the `data:` payload of each complete frame.
  for (;;) {
    const { value, done } = await reader.read()
    if (done) break
    buffer += decoder.decode(value, { stream: true })

    let boundary: number
    while ((boundary = buffer.indexOf('\n\n')) !== -1) {
      const frame = buffer.slice(0, boundary)
      buffer = buffer.slice(boundary + 2)
      dispatchFrame(frame)
    }
  }
  // Flush any trailing frame without a final blank line.
  if (buffer.trim().length > 0) dispatchFrame(buffer)

  function dispatchFrame(frame: string) {
    // A frame may contain several lines; the JSON payload lives on `data:` lines.
    const data = frame
      .split('\n')
      .filter((line) => line.startsWith('data:'))
      .map((line) => line.slice('data:'.length).trim())
      .join('')

    if (!data || data === '[DONE]') return

    let evt: AgentEvent
    try {
      evt = JSON.parse(data) as AgentEvent
    } catch {
      return // ignore non-JSON keep-alive lines
    }

    switch (evt.type) {
      case 'RUN_STARTED':
        handlers.onRunStarted?.()
        break

      case 'TEXT_MESSAGE_START':
        currentMessageId = evt.messageId ?? newId()
        handlers.onTextStart?.(currentMessageId)
        break

      case 'TEXT_MESSAGE_CONTENT': {
        const id = evt.messageId ?? currentMessageId ?? (currentMessageId = newId())
        handlers.onTextContent?.(id, evt.delta ?? '')
        break
      }

      case 'TEXT_MESSAGE_END':
        handlers.onTextEnd?.(evt.messageId ?? currentMessageId ?? newId())
        currentMessageId = null
        break

      case 'TOOL_CALL_START':
        currentToolCallId = evt.toolCallId ?? newId()
        handlers.onToolCallStart?.(currentToolCallId, evt.toolCallName ?? evt.toolName ?? 'tool')
        break

      case 'TOOL_CALL_ARGS':
        handlers.onToolCallArgs?.(evt.toolCallId ?? currentToolCallId ?? newId(), evt.delta ?? '')
        break

      case 'TOOL_CALL_END':
        handlers.onToolCallEnd?.(evt.toolCallId ?? currentToolCallId ?? newId())
        break

      case 'TOOL_CALL_RESULT':
        handlers.onToolCallResult?.(
          evt.toolCallId ?? currentToolCallId ?? newId(),
          evt.content ?? '',
        )
        currentToolCallId = null
        break

      case 'RUN_FINISHED':
        handlers.onRunFinished?.()
        break

      case 'RUN_ERROR':
        handlers.onError?.(evt.message ?? 'Unknown run error')
        break

      default:
        // Other AG-UI events (state snapshots, steps, ...) are ignored in this demo.
        break
    }
  }
}
