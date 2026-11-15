---
type: llm
focus: trace
weight: 1
---

Warns that a shorthand union would box Option<int>/Option<decimal> on the hot path, and either keeps the struct or proposes a custom [Union] struct using HasValue/TryGetValue
