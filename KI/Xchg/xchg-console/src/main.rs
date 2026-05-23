use std::env;
use std::error::Error;

use xchg_logic::ExchangeRatesApiClient;

#[tokio::main]
async fn main() -> Result<(), Box<dyn Error>> {
    dotenvy::dotenv()?;
    let api_key = env::var("XCHANGE_API_KEY").unwrap();

    let client = ExchangeRatesApiClient::new(api_key);

    let result = client.fetch_latest_rates(&xchg_logic::ApiRequestConfig::default())
        .await
        .unwrap();

    println!("Latest exchange rates: {:?}", result);

    Ok(())
}
