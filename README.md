# Exchange Rate Fetcher & Printer

An ASP.NET Core solution for fetching and exposing currency exchange rates via a REST API.

## 🚀 Overview

This solution contains two projects:

1. **RateFetcher**: A background service that periodically fetches exchange rates from the [XE API](https://www.xe.com/xecurrencydata/) and writes them to `exchangeRates.json`.
2. **RatePrinter**: A Web API that exposes the exchange rates stored in the JSON file.

### API Endpoints

- `GET /api/rates` → Returns all exchange rates.
- `GET /api/rates/{pair}` → Returns a specific currency pair (e.g., `USDILS`).

---

## 🛠️ Setup Instructions

### ✅ Create your `.env` file


A `.env.example` file is provided inside the `RateFetcher` project folder to show the required environment variables. Create a new `.env` file and replace the placeholder values with your actual XE API credentials.

Example `.env`:

```env
XE_ACCOUNT_ID=-123456789
XE_API_KEY=abcdefg123456
```



The application will automatically load the `.env` file when running.

---

## 📂 Project Structure

```
exchangeRates.json
/RateFetcher
    Program.cs
    Worker.cs
    .env
    .env.example
/RatePrinter
    Program.cs
    /Controllers
        RatesController.cs
/Types
    Constants.cs
    ExchangeRate.cs
```

---

## ✨ Improvements

Future improvements could include:

- Dependency injection for `HttpClient`
- Automated tests (unit & integration)
- Containerization (Docker support)

---

Thank you for reviewing this project!
