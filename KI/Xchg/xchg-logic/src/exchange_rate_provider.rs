use std::collections::HashMap;
use std::fmt;

use chrono::{DateTime, Utc};
use thiserror::Error;

#[derive(Debug, Clone, PartialEq, Eq, Hash)]
pub struct Currency {
    pub code: String,       // ISO 4217 code like "USD", "GBP"
    pub name: String,       // Full name like "US Dollar"
    pub symbol: String,     // Symbol like "$", "£"
    pub decimal_places: u8, // Usually 2, but some currencies use 0 or 3
}

impl Currency {
    pub fn new(code: &str, name: &str, symbol: &str, decimal_places: u8) -> Self {
        Self {
            code: code.to_uppercase(),
            name: name.to_string(),
            symbol: symbol.to_string(),
            decimal_places,
        }
    }
}

impl fmt::Display for Currency {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{} ({})", self.code, self.name)
    }
}

#[derive(Debug, Clone)]
pub struct ExchangeRate {
    pub from_currency_code: String, // Always "EUR" in your case
    pub to_currency_code: String,
    pub rate: f64, // How many units of to_currency per from_currency
    pub last_updated: DateTime<Utc>,
}

impl ExchangeRate {
    pub fn new(to_currency_code: &str, rate: f64) -> Result<Self, ExchangeRateProviderError> {
        if rate <= 0.0 || !rate.is_finite() {
            return Err(ExchangeRateProviderError::InvalidRate("Rate must be positive and finite".to_string()));
        }

        Ok(Self {
            from_currency_code: "EUR".to_string(),
            to_currency_code: to_currency_code.to_uppercase(),
            rate,
            last_updated: Utc::now(),
        })
    }
}

#[derive(Debug)]
pub struct ExchangeRateProvider {
    currencies: HashMap<String, Currency>, // Available currencies by code
    rates: HashMap<String, ExchangeRate>,  // Exchange rates by target currency code
}

#[derive(Error, Debug)]
pub enum ExchangeRateProviderError {
    #[error("Currency not found: {0}")]
    CurrencyNotFound(String),

    #[error("Invalid base currency")]
    InvalidBaseCurrency,

    #[error("Currency already exists: {0}")]
    CurrencyAlreadyExists(String),

    #[error("Exchange rate already exists for currency: {0}")]
    ExchangeRateAlreadyExists(String),

    #[error("Exchange rate not found for currency: {0}")]
    ExchangeRateNotFound(String),

    #[error("Invalid exchange rate: {0}")]
    InvalidRate(String),

    #[error("Cannot remove base currency (EUR)")]
    CannotRemoveBaseCurrency,
}

impl ExchangeRateProvider {
    pub fn new() -> Self {
        let mut currencies = HashMap::new();
        // Always include EUR as the base currency
        currencies.insert("EUR".to_string(), Currency::new("EUR", "Euro", "€", 2));

        Self {
            currencies,
            rates: HashMap::new(),
        }
    }

    pub fn add_currency(&mut self, currency: Currency) -> Result<(), ExchangeRateProviderError> {
        if self.currencies.contains_key(&currency.code) {
            Err(ExchangeRateProviderError::CurrencyAlreadyExists(currency.code))
        } else {
            self.currencies.insert(currency.code.clone(), currency);
            Ok(())
        }
    }

    pub fn add_rate(&mut self, rate: ExchangeRate) -> Result<(), ExchangeRateProviderError> {
        if rate.from_currency_code != "EUR" {
            return Err(ExchangeRateProviderError::InvalidBaseCurrency);
        }

        if !self.currencies.contains_key(&rate.to_currency_code) {
            return Err(ExchangeRateProviderError::CurrencyNotFound(rate.to_currency_code.clone()));
        }

        if self.rates.contains_key(&rate.to_currency_code) {
            return Err(ExchangeRateProviderError::ExchangeRateAlreadyExists(rate.to_currency_code));
        }

        self.rates.insert(rate.to_currency_code.clone(), rate);
        Ok(())
    }

    pub fn set_rate(&mut self, to_currency_code: &str, rate: f64) -> Result<(), ExchangeRateProviderError> {
        if rate <= 0.0 || !rate.is_finite() {
            return Err(ExchangeRateProviderError::InvalidRate("Exchange rate must be positive and finite".to_string()));
        }

        if !self.currencies.contains_key(to_currency_code) {
            return Err(ExchangeRateProviderError::CurrencyNotFound(to_currency_code.to_string()));
        }

        let exchange_rate = ExchangeRate::new(to_currency_code, rate)?;
        self.rates.insert(to_currency_code.to_uppercase(), exchange_rate);
        Ok(())
    }

    pub fn get_currency(&self, code: &str) -> Option<&Currency> {
        self.currencies.get(&code.to_uppercase())
    }

    pub fn get_rate(&self, to_currency_code: &str) -> Option<&ExchangeRate> {
        self.rates.get(&to_currency_code.to_uppercase())
    }

    pub fn update_rate(&mut self, to_currency_code: &str, new_rate: f64) -> Result<(), ExchangeRateProviderError> {
        if new_rate <= 0.0 || !new_rate.is_finite() {
            return Err(ExchangeRateProviderError::InvalidRate("Exchange rate must be positive and finite".to_string()));
        }

        if let Some(rate) = self.rates.get_mut(to_currency_code) {
            rate.rate = new_rate;
            rate.last_updated = Utc::now();
            Ok(())
        } else {
            Err(ExchangeRateProviderError::ExchangeRateNotFound(to_currency_code.to_string()))
        }
    }

    pub fn exchange_rates(&self) -> impl Iterator<Item = (&str, &ExchangeRate)> {
        self.rates.iter().map(|(k, v)| (k.as_str(), v))
    }

    pub fn available_currencies(&self) -> impl Iterator<Item = &Currency> {
        self.currencies.values()
    }

    pub fn has_currency(&self, code: &str) -> bool {
        self.currencies.contains_key(&code.to_uppercase())
    }

    pub fn has_rate(&self, to_currency_code: &str) -> bool {
        self.rates.contains_key(&to_currency_code.to_uppercase())
    }

    pub fn remove_currency(&mut self, code: &str) -> Result<Currency, ExchangeRateProviderError> {
        let code_upper = code.to_uppercase();

        // Don't allow removing EUR
        if code_upper == "EUR" {
            return Err(ExchangeRateProviderError::CannotRemoveBaseCurrency);
        }

        // Remove associated rate if it exists
        self.rates.remove(&code_upper);

        self.currencies
            .remove(&code_upper)
            .ok_or_else(|| ExchangeRateProviderError::CurrencyNotFound(code.to_string()))
    }
}

impl Default for ExchangeRateProvider {
    fn default() -> Self {
        Self::new()
    }
}
