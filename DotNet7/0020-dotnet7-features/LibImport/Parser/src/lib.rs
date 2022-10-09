#[no_mangle]
pub extern "C" fn Parse(input: *const u8, length: usize) -> i32 {
    let input = unsafe { std::str::from_utf8_unchecked(std::slice::from_raw_parts(input, length)) };
    input.parse().unwrap_or(0)
}
