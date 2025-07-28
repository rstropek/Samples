use crate::ExchangeRate;
use chrono::{DateTime, Utc};
use serde::{Deserialize, Serialize};
use std::collections::HashMap;
use thiserror::Error;

/// Raw API response structure - mirrors the API exactly
#[derive(Debug, Clone, Deserialize, Serialize)]
pub struct ExchangeRatesApiResponse {
    pub success: bool,
    pub timestamp: u64,
    pub base: String,
    pub date: String,
    pub rates: HashMap<String, f64>,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub error: Option<ApiErrorDetails>,
}

/// API error details structure
#[derive(Debug, Clone, Deserialize, Serialize)]
pub struct ApiErrorDetails {
    pub code: u32,
    pub info: String,
}

/// Configuration for API requests
#[derive(Debug, Clone)]
pub struct ApiRequestConfig {
    pub base_currency: Option<String>,
    pub target_currencies: Option<Vec<String>>,
}

impl Default for ApiRequestConfig {
    fn default() -> Self {
        Self {
            base_currency: Some("EUR".to_string()),
            target_currencies: None,
        }
    }
}

/// HTTP client for the Exchange Rates API
#[derive(Debug, Clone)]
pub struct ExchangeRatesApiClient {
    client: reqwest::Client,
    api_key: String,
    base_url: String,
}

#[derive(Error, Debug)]
pub enum ExchangeRatesApiError {
    #[error("HTTP request failed: {0}")]
    HttpError(#[from] reqwest::Error),
    #[error("JSON parsing failed: {0}")]
    ParseError(#[from] serde_json::Error),
    #[error("API error {code}: {info}")]
    ApiError { code: u32, info: String },
    #[error("Wrong base currency (must be EUR)")]
    WrongBaseCurrency(),
    #[error("Invalid timestamp: {0}")]
    TimestampError(i64),
}

impl ExchangeRatesApiClient {
    /// Creates a new API client with the provided API key
    pub fn new(api_key: String) -> Self {
        Self {
            client: reqwest::Client::new(),
            api_key,
            base_url: "https://api.exchangeratesapi.io/v1".to_string(),
        }
    }

    /// Sets a custom base URL (useful for testing or different API endpoints)
    pub fn with_base_url(mut self, base_url: String) -> Self {
        self.base_url = base_url;
        self
    }

    /// Fetches the latest exchange rates from the API
    pub async fn fetch_latest_rates(&self, config: &ApiRequestConfig) -> Result<ExchangeRatesApiResponse, ExchangeRatesApiError> {
        let url = self.build_request_url(config);

        let response = self.client.get(&url).send().await?;

        let api_response: ExchangeRatesApiResponse = response.json().await?;

        if !api_response.success {
            return Err(self.handle_api_error(&api_response));
        }

        Ok(api_response)
    }

    /// Builds the request URL with query parameters
    fn build_request_url(&self, config: &ApiRequestConfig) -> String {
        let mut url = format!("{}/latest?access_key={}", self.base_url, self.api_key);

        if let Some(base_currency) = &config.base_currency {
            url.push_str(&format!("&base={}", base_currency.to_uppercase()));
        }

        if let Some(target_currencies) = &config.target_currencies {
            let symbols_str = target_currencies.iter().map(|s| s.to_uppercase()).collect::<Vec<_>>().join(",");
            url.push_str(&format!("&symbols={}", symbols_str));
        }

        url
    }

    /// Handles API error responses
    fn handle_api_error(&self, response: &ExchangeRatesApiResponse) -> ExchangeRatesApiError {
        if let Some(error) = &response.error {
            ExchangeRatesApiError::ApiError {
                code: error.code,
                info: error.info.clone(),
            }
        } else {
            ExchangeRatesApiError::ApiError {
                code: 0,
                info: "API request failed with no error details".to_string(),
            }
        }
    }
}

/// Converter trait to transform API responses into domain models
pub trait IntoExchangeRates {
    fn into_exchange_rates(self, target_base: &str) -> Result<Vec<ExchangeRate>, ExchangeRatesApiError>;
}

impl IntoExchangeRates for ExchangeRatesApiResponse {
    /// Converts API response into domain ExchangeRate objects
    fn into_exchange_rates(self, target_base: &str) -> Result<Vec<ExchangeRate>, ExchangeRatesApiError> {
        let last_updated = DateTime::from_timestamp(self.timestamp as i64, 0).unwrap_or_else(Utc::now);

        if self.base != "EUR" {
            return Err(ExchangeRatesApiError::WrongBaseCurrency());
        }

        let mut exchange_rates = Vec::new();

        for (currency_code, rate) in &self.rates {
            let exchange_rate = ExchangeRate {
                from_currency_code: target_base.to_string(),
                to_currency_code: currency_code.clone(),
                rate: *rate,
                last_updated,
            };

            exchange_rates.push(exchange_rate);
        }

        Ok(exchange_rates)
    }
}

/// Builder for creating API request configurations
pub struct ApiRequestBuilder {
    config: ApiRequestConfig,
}

impl ApiRequestBuilder {
    pub fn new() -> Self {
        Self {
            config: ApiRequestConfig::default(),
        }
    }

    pub fn base_currency(mut self, base: &str) -> Self {
        self.config.base_currency = Some(base.to_uppercase());
        self
    }

    pub fn target_currencies(mut self, currencies: &[&str]) -> Self {
        self.config.target_currencies = Some(currencies.iter().map(|s| s.to_uppercase()).collect());
        self
    }

    pub fn build(self) -> ApiRequestConfig {
        self.config
    }
}

impl Default for ApiRequestBuilder {
    fn default() -> Self {
        Self::new()
    }
}
