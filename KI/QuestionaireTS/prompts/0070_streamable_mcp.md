Add a Streamable HTTP server in analogy to the existing STDIO server (CLI). The Streamable HTTP server must be in the Next.js app. Functionality should be identical.

Note that tool definition must not be duplicated. As it is used in STDIO and Streamable HTTP server, it should be defined in packages/lib.

Add an integration test for the Streamable HTTP server that mirrors the existing integration test for the STDIO server, but tests the HTTP transport.

As this is a demo, there is no need for authentication. However, we will test the MCP server using MCP Inspector. It runs in the browser, so we need CORS (allow everything is fine).

At the end, don't forget to update docs in `./docs`.
