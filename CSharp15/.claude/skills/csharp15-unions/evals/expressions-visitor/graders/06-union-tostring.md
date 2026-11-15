---
type: llm
focus: trace
weight: 1
---

Notes that a Token union's ToString() prints the union type name (not the case value), so existing 'Unexpected token {token}' messages must use .Value or a ToString override
