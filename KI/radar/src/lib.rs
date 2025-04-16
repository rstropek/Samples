use wasm_bindgen::prelude::*;
use web_sys::console;

mod radar;
mod helpers;
mod planes;

#[wasm_bindgen]
pub fn ping() -> String {
    console::log_1(&JsValue::from_str("ping"));
    "pong".to_string()
}

