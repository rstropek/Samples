# Cleanup Chat Handler

`chat_handler` currently contains lots of `unwrap` calls. Could you use `thiserror` to define an error type and change the `chat_handler` to error propagation?
