# Getting Started With the Web Application

This specification defines the first implementation milestone of the AI-assisted prosthetics data exploration web application.  
The goal of this milestone is to set up a **FastAPI** server with an **HTMX-based** user interface and basic routing.

## Purpose

The system provides a web interface for users to interact with the AI agent via natural-language prompts about the prosthetics dataset.  
In this initial milestone, the app does **not** yet process prompts — it serves static, placeholder content.

## Functional Overview

### FastAPI Application

- Create a FastAPI app instance
- Configure Jinja2 templates for HTML rendering
- Serve static files (CSS, JS, images) from a `/static` directory.
- Define at least two routes:
  - `GET /` → Renders the start page with the input field and placeholder content.
  - `POST /ask` → Returns HTML fragments via HTMX, currently returning Lorem Ipsum text.

## Frontend (HTML + HTMX)

### `/` – Start Page Layout

#### Components

1. **Header / Input Field**

- Positioned at the top of the page.
- Contains:
    - A short title, e.g., “AI-Powered Prosthetics Data Explorer”.
    - A single text input (multi-line, resizeable) where users can type prompts.
    - A _Submit_ button or automatic submission on Enter.
- Cannot be submitted if empty.
- Disabled state while waiting for a response.
- The form uses HTMX to post to `/ask`.

2. **Sample Prompts Section**

- Displayed under the input field.
- Provides 3–5 visually distinct “sample prompts” as cards or buttons.
  - Current placeholders: _Sample Prompt 1_, _Sample Prompt 2_, _Sample Prompt 3_.
  - Only display a short title of the prompt suggestions.
  - If the user clicks on a sample prompt, it populates the input field with that prompt. For now, add a _Lorem Ipsum_ placeholder text if clicked.
  - In later stages, these will be clickable and auto-fill the input field.

3. **Output Area**

- The main section of the page.
- `<div id="ai-output">` will hold responses returned by the AI API.

4. **Visual Style** 

- Use a basic CSS stylesheet to create a clean, professional layout:
  - Centered content with max-width (≈ 800px).
  - Soft background, rounded input fields, and consistent font.
  - Distinct section headers (Input / Samples / Output).
  - The goal is readability, not a polished brand design.

## Backend Routes

### GET /

**Purpose:** Render the main page template.

Response:
- HTML page containing:
  - Header with form and sample prompts
  - Empty output area

Implementation Notes:
- Use Jinja2 template: index.html.
- Pass no dynamic data yet.

### POST /ask

**Purpose:** Process a user prompt (via HTMX). For now, this is mocked.

Input:
  - Form field: prompt (string)

Output:
- Returns a ready-to-render HTML fragment (not JSON).
- Content: Static HTML with a <p> containing Lorem Ipsum.

## Components

Add the following components using Poetry:

* fastapi
* uvicorn
* jinja2
* python-multipart

Add HTMX via CDN as follows (taken from HTMX documentation):
`<script src="https://cdn.jsdelivr.net/npm/htmx.org@2.0.8/dist/htmx.min.js" integrity="sha384-/TgkGk7p307TH7EXJDuUlgG3Ce1UVolAOFopFekQkkXihi5u/6OCvVKyz1W+idaz" crossorigin="anonymous"></script>`

## Unit Tests

For now, we do not need unit tests. Future iterations will add tests for the AI processing logic.

## Project Structure

```txt
project_root/
│
├── src/
│   ├── main.py              # FastAPI entrypoint
│   ├── templates/
│   │   └── index.html       # Jinja2 template for start page
│   ├── static/
│   │   └── style.css        # Basic styling
│   └── routers/
│       └── ask.py           # (Optional) future modularization
│
├── tests/
│   └── test_routes.py       # Unit tests for endpoints
│
├── pyproject.toml           # Poetry configuration (already existing)
└── AGENTS.md                # Existing project guidelines
```

Note: you can remove/replace the existing `backend` folder.

Modify the _start_ command in `pyproject.toml` to starting the FastAPI app with uvicorn.

## Acceptance Criteria

1.	Running uvicorn src.main:app starts a web server.
2.	Visiting http://localhost:8000/ shows:
    - A text field and button at the top.
    - Placeholder sample prompts below.
    - A main output section.
3.	Submitting the form (via HTMX) updates the output area dynamically.
4.	No AI or pandas logic yet — only static placeholder content.
5.	Code passes mypy and black checks per AGENTS.md guidelines.
