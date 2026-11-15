---
type: llm
focus: trace
weight: 1
---

Suggests replacing SensorValue and its converter with a union (e.g. `union SensorValue(double, string)`) serialised by System.Text.Json's built-in union support
