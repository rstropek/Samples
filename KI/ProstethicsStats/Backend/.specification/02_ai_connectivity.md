# Integration of OpenAI Responses API

This specification defines the next iteration of the prosthetics data explorer project. The goal is to extend the `/ask` endpoint so it communicates with the **OpenAI Responses API**, reads its configuration from files, and produces output.

## Purpose

Enhance the current mock `/ask` route to generate real AI responses using the OpenAI Responses API. The system prompt will be read from local files.

## Research

Use the `context7` MCP server to research in the library id `/websites/platform_openai` for "OpenAI Responses API Python" with max. 5000 tokens. The output will give you up-to-date code examples and best practices for using the OpenAI Responses API in Python.

Use context7 MCP server to research `/websites/fastapi_tiangolo` for "lifespan event handlers" with max 2500 tokens.

## Functional Overview

- The `/ask` endpoint calls the **OpenAI Responses API** with a **system prompt** loaded from a Markdown file.
  - Use the OpenAI `gpt-5` model with reasoning _minimal_ (you might not know about this model; trust me, it exists).
  - Limit the output tokens to 8192.
  - If the OpenAI call fails, return an appropriate error message to the frontend.
  - It will always be a single-turn conversation. Instruct OpenAI no not store any conversation history.
- You can assume that the system prompt tells the LLM to answer with a single python script, and that script only without any additional text.
- The generated python script generates files. Make sure a suitable temporary folder is used.
  - The scripts writes HTML output (fragment top-level `div`) to `index.html` and optionally saves plots as images (_plot1.png_, _plot2.png_, etc.). If there are plots, they are referenced in the HTML using `<img src="plot1.png"...`.
- Execute the python script using `exec()` to generate HTML and images.
  - Use basic sandboxing, but remember that the script must be able to import libraries such as `matplotlib` and generate files in the temporary folder.
- Use regex to find all `<img src="...">` tags in the generated HTML, read the corresponding image files, and convert them to base64 strings.
- Return the final HTML with embedded base64 images to the frontend.
- Once the HTML is returned, clean up all temporary files.

## Backend Logic

### System Prompt Loading

**File:** `src/config/system_prompt.md` (relative to project root)

**Implementation Notes:**
- Read this file once at app startup and cache it.
- Use UTF-8 encoding.
- Provide fallback text (e.g., `"No system prompt loaded"`) if missing.

### Sample Prompt

Generate a sample prompt that instructs the LLM to create a dummy HTML report with a very simple plot (using `matplotlib`). Do **not** use the data set in `/data` yet. We will integrate that in the next iteration.

## Components

Add the following components using Poetry:

* openai
* matplotlib

## Acceptance Criteria

1. The /ask endpoint successfully calls the OpenAI Responses API using the API key from .env.
2. The system prompt and schema are read from files on startup.
3. The server returns HTML with:
   * AI-generated text
   * (Optional) image rendered from base64 if provided.
5. All code passes mypy and adheres to AGENTS.md coding standards.

