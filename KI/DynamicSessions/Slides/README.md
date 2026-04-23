# Code Interpreter — Slides

An interactive, presenter-driven slide that visualises the **Code Interpreter** concept for LLMs. Built with Vite + vanilla HTML/CSS/JS; animations are SVG + CSS transitions driven by a state-class machine.

## Running it

```bash
npm install
npm run dev       # → http://localhost:5173
npm run build     # → ./dist (static; runs from USB/local file too)
```

## Controls

- **Space / → / Enter / click** — next step
- **← / Backspace** — previous step
- **R / Home** — reset to step 1
- **End** — jump to final step
- **Deep links**: `#<N>` in the URL jumps to step N (e.g. `…/#17`). The hash updates on every step change.

---

## Content

Two scenarios, presented back-to-back (30 steps total).

### Scenario 1 — Data analysis (steps 1–14)

> *Analyzing large data with a Code Interpreter*

- **Setup**: a warehouse (database) holding `customers.csv` (2.3 GB · 14M rows · PII) and an LLM in the cloud.
- **Approach 01 — Send everything** (fails):
  - Prompt "Analyze this dataset" is built, CSV attached.
  - Everything flies to the LLM.
  - Red error state: *Context window exceeded · Sensitive data leaked*.
- **Rewind → Approach 02 — Ship the code, not the data**:
  - Extract a small metadata sketch (schema, dtypes, pseudonymised PII). Full CSV stays in the warehouse.
  - Prompt morphs to "*Write a script* to analyze this dataset"; metadata attaches.
  - Prompt + metadata fly to LLM → LLM responds with `prog.py`.
  - A disposable **sandbox** spins up. Script and full CSV enter the sandbox.
  - Computation runs inside the box (bar chart: *revenue by region · FY24*).
  - Only the **aggregated insights** card flies back out — "no raw data left the sandbox".

### Scenario 2 — Data generation (steps 15–30)

> *Generating test data with a Code Interpreter*

- **Setup**: the warehouse is *empty*; we need to produce data.
- **Approach 01 — Ask the model to just produce it** (fails):
  - Prompt "Generate a large dataset" flies to the LLM.
  - The LLM begins streaming rows back as tokens.
  - The emitted CSV balloons in size and turns red → *Context window exceeded*.
- **Rewind → Approach 02 — Ship the generator**:
  - Prompt reframed as "*Write a script* to generate test data"; schema spec attaches.
  - Prompt + schema fly to LLM → LLM responds with `prog.py`.
  - Sandbox materialises; the generator script enters.
  - The script runs; a fresh dataset materialises inside the sandbox (in the slot where `prog.py` was).
  - The generated CSV flies out to the warehouse.
  - Success card: **"Data is ready for further processing"**.

---

## The shared lesson

Both scenarios land on the same idea:

> **Ship the code, not the data.** Pseudonymised metadata + a generated script executed inside an ephemeral sandbox solves what direct prompting cannot — regardless of whether the data already exists or still needs to be produced.
