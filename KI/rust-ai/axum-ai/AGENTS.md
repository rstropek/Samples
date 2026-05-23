# Repository Guidelines

Use this guide to align contributions with this Axum-based AI sample.

## Build Command

* Whenever you make changes, ensure the project builds successfully using `cargo build`.
* Run `cargo fmt` to format the code according to Rust standards.
* Execute `cargo clippy` to catch common mistakes and improve code quality.

# Coding Style & Naming Conventions

* Follow Rust 2024 defaults: 4‑space indentation, snake_case for functions/modules, UpperCamelCase for types, SCREAMING_SNAKE_CASE for constants.

* Prefer `?` propagation over `unwrap` in library code; reserve `unwrap` for startup wiring (`main.rs` already uses it for clarity).

* Keep handlers small and asynchronous; move blocking work into separate tasks.

* If you add dependencies, do not add them to `Cargo.toml` directly. Use `cargo add <dependency>` to ensure version compatibility.
