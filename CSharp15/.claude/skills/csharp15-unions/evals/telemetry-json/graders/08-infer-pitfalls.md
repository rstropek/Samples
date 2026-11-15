---
type: llm
focus: trace
weight: 1
---

Warns about at least one InferClosedTypePolymorphism pitfall: a closed base without the option serialises derived types as {} (data loss), or mixing it with [JsonDerivedType] disables inference (SYSLIB1244)
