---
type: llm
focus: trace
weight: 1
---

Suggests a union-based Result whose error case is a distinct type (e.g. `record Error(string)`), not `(T, string)` - or explicitly explains why `(T, string)` would be ambiguous
