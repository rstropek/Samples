use rand::seq::IndexedRandom;

const PONIES: [&str; 22] = [
    "Pinkie",
    "Pie",
    "Rainbow",
    "Dash",
    "Twilight",
    "Sparkle",
    "Applejack",
    "Rarity",
    "Fluttershy",
    "Spike",
    "Starlight",
    "Glimmer",
    "Trixie",
    "Sunset",
    "Shimmer",
    "Princess",
    "Celestia",
    "Luna",
    "Shining",
    "Armor",
    "Big",
    "McIntosh",
];

pub fn generate_pony_password(min_length: usize) -> String {
    let mut rng = rand::rng();
    let mut password = String::with_capacity((min_length as f32 * 1.25) as usize);

    while password.len() < min_length {
        if let Some(pony) = PONIES.choose(&mut rng) {
            password.push_str(pony);
        }
    }

    password
}
