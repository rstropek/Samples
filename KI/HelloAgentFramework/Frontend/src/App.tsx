import { useMemo, useRef, useState, type FormEvent } from 'react'
import { marked } from 'marked'
import { runAgent, type AgentMessage } from './aguiClient'

// The agent's base URL is injected by the Aspire AppHost as VITE_AGENT_URL.
// The fallback is only used when running the client outside of Aspire.
const AGENT_URL = import.meta.env.VITE_AGENT_URL ?? 'http://localhost:5180'

// A chat timeline is a mix of text messages and tool-call cards, shown in arrival order.
type MessageItem = { kind: 'message'; id: string; role: 'user' | 'assistant'; content: string }
type ToolItem = { kind: 'tool'; id: string; name: string; args: string; result: string; done: boolean }
type ChatItem = MessageItem | ToolItem

function renderMarkdown(text: string): string {
  // marked.parse is synchronous with the default options. We render the result as raw
  // HTML, which is fine for this trusted, local-only demo (no sanitizer like DOMPurify).
  return marked.parse(text) as string
}

function formatArgs(args: string): string {
  try {
    return JSON.stringify(JSON.parse(args))
  } catch {
    return args
  }
}

export default function App() {
  const [items, setItems] = useState<ChatItem[]>([])
  const [input, setInput] = useState('')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)

  // One stable thread id per browser session keeps the conversation's context together.
  const threadId = useMemo(() => crypto.randomUUID(), [])
  const scrollRef = useRef<HTMLDivElement>(null)

  function scrollToEnd() {
    queueMicrotask(() => {
      const el = scrollRef.current
      if (el) el.scrollTo({ top: el.scrollHeight })
    })
  }

  // --- timeline mutations (create-if-missing, so we tolerate events with no/late ids) ---
  function appendAssistantText(id: string, delta: string) {
    setItems((prev) => {
      if (prev.some((i) => i.kind === 'message' && i.id === id)) {
        return prev.map((i) =>
          i.kind === 'message' && i.id === id ? { ...i, content: i.content + delta } : i,
        )
      }
      return [...prev, { kind: 'message', id, role: 'assistant', content: delta }]
    })
    scrollToEnd()
  }

  function patchTool(id: string, patch: Partial<Omit<ToolItem, 'kind' | 'id'>>, appendArgs?: string) {
    setItems((prev) => {
      if (prev.some((i) => i.kind === 'tool' && i.id === id)) {
        return prev.map((i) =>
          i.kind === 'tool' && i.id === id
            ? { ...i, ...patch, args: appendArgs ? i.args + appendArgs : i.args }
            : i,
        )
      }
      const base: ToolItem = { kind: 'tool', id, name: 'tool', args: appendArgs ?? '', result: '', done: false }
      return [...prev, { ...base, ...patch }]
    })
    scrollToEnd()
  }

  async function send(e: FormEvent) {
    e.preventDefault()
    const text = input.trim()
    if (!text || busy) return

    const userMsg: MessageItem = { kind: 'message', id: crypto.randomUUID(), role: 'user', content: text }

    // The conversation we send is the full text history (client-managed context).
    const history: AgentMessage[] = [
      ...items.filter((i): i is MessageItem => i.kind === 'message'),
      userMsg,
    ].map((m) => ({ id: m.id, role: m.role, content: m.content }))

    setItems((prev) => [...prev, userMsg])
    setInput('')
    setError(null)
    setBusy(true)

    try {
      await runAgent(AGENT_URL, threadId, history, {
        onTextContent: (id, delta) => appendAssistantText(id, delta),
        onToolCallStart: (id, name) => patchTool(id, { name }),
        onToolCallArgs: (id, delta) => patchTool(id, {}, delta),
        onToolCallResult: (id, result) => patchTool(id, { result, done: true }),
        onError: (message) => setError(message),
      })
    } catch (err) {
      setError(err instanceof Error ? err.message : String(err))
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="app">
      <header className="app-header">
        <h1>AG-UI Chat</h1>
        <span className="agent-url" title="Injected by Aspire as VITE_AGENT_URL">
          {AGENT_URL}
        </span>
      </header>

      <div className="timeline" ref={scrollRef}>
        {items.length === 0 && (
          <p className="hint">
            Ask me anything. Try <code>What is 21 + 21?</code> to watch a tool call.
          </p>
        )}

        {items.map((item) =>
          item.kind === 'message' ? (
            <div key={item.id} className={`msg msg-${item.role}`}>
              {item.role === 'assistant' ? (
                <div className="md" dangerouslySetInnerHTML={{ __html: renderMarkdown(item.content) }} />
              ) : (
                <div className="text">{item.content}</div>
              )}
            </div>
          ) : (
            <div key={item.id} className={`tool ${item.done ? 'tool-done' : 'tool-running'}`}>
              <div className="tool-head">
                <span className="tool-icon">🔧</span>
                <span className="tool-name">{item.name}</span>
                <span className="tool-status">{item.done ? 'done' : 'running…'}</span>
              </div>
              {item.args && (
                <div className="tool-row">
                  args: <code>{formatArgs(item.args)}</code>
                </div>
              )}
              {item.done && (
                <div className="tool-row">
                  result: <code>{item.result}</code>
                </div>
              )}
            </div>
          ),
        )}
      </div>

      {error && <div className="error">⚠️ {error}</div>}

      <form className="composer" onSubmit={send}>
        <input
          type="text"
          value={input}
          placeholder={busy ? 'Waiting for the agent…' : 'Type a message…'}
          onChange={(e) => setInput(e.target.value)}
          disabled={busy}
          autoFocus
        />
        <button type="submit" disabled={busy || !input.trim()}>
          Send
        </button>
      </form>
    </div>
  )
}
