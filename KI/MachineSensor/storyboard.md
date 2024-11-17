No WSL
Could not make devcontainers work
Let's use Codeium to setup a container environment for our development.

Rust in Windsurf and Docker
Angular in VSCode and WSL

Create a Dockerfile for me. It should be based on the official rust image, but also have the latest Node LTS version installed.

- Insert in Terminal
- Read and write mode
- File need review

Looks good. Remove the steps to verify installations. Also, do not start bash. This container will run in the background and should simply wait until a connection comes in.

Can you add ssh to this Dockerfile?

Add a dockerignore file that includes everything except the files in the .vscode folder. Copy the .vscode folder into /app.

Add /usr/local/cargo/bin to PATH in the root's user .bashrc file.

Note:
> C:\Users\RainerStropek\.ssh\known_hosts
> export PATH="/usr/local/cargo/bin:${PATH}"
  rustup default stable

cargo new backend

I want to add the axum framework. What do I need to do?

https://github.com/tokio-rs/axum/blob/main/examples/hello-world/src/main.rs

```
@host = http://localhost:3000

###
GET {{host}}/
```

Explain get_tail

Implement an iterator for RingBuffer. It should iterate starting with get_tail

ng new --directory frontend --routing --skip-tests --standalone --strict --style css --view-encapsulation ShadowDom --ssr false frontend

Take a look at @app.component.html . You see the div structure there. I want to have a CSS grid where the header is on the top (100% vw). The content should take up the entire other space (vertically and horizontally). I want to have a vertical scroll bar only in the content area.

Add a simple CSS reset

Look at @navbar.component.html . I want to see the menu items side by side, not one below the other

Copy Rust Measurement -> Strg + Alt + v -> Smart paste

By calling BASEURL/measurements, I get a list of measurements. The service should query this endpoint every 500ms. The result must be published using an RxJS observable. Suggest an implementation.

Look at @measurement-history.component.html . Create a CSS grid for dx and dy

How can I implement zebra striping

As you can see in @measurement-retrieval.service.ts , I have a service that returns a collection of measurements from a service. The measurements represent the measurements in the last n seconds. I need a component for Angular that displays the data in a simple line chart. The x axis is each measurement. The y axis is -min value..+max value from the measurements. The component must use SVG to render the chart. Use Angular data binding. You see a sample for component that retrieves the measurement value and shows them in a table in @measurement-history.component.html and @measurement-history.component.ts 

