---
type: llm
focus: trace
weight: 1
---

States that with `closed` the `_ => throw new UnreachableException(...)` arm (and/or the final throw in ToHttpStatus) can be removed because the compiler checks exhaustiveness
