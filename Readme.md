# eTaxesApp

A simple CLI application that calculates taxes for various financial resources, such as income, stocks, dividends and
properties.

## Features

- Importing financial records (from JSON files)
- Exporting financial records (in JSON format)
- Calculating taxes for a specific period of time

## Usage

Importing financial records:

```bash
eTaxesApp import --files=a.json,b.yaml,c.csv
```

### Example import file

```json
[
  {
    "time": "2024-05-19T09:27:52+00:00",
    "type": "Dividend",
    "amount": 100.00,
    "currency": "EUR",
    "description": "BMW dividend",
    "metadata": {
      "dividendCountry": "EU"
    }
  },
  {
    "time": "2024-05-19T09:27:52+00:00",
    "type": "Stock",
    "amount": 1000.00,
    "currency": "EUR",
    "description": "Tesla stock",
    "metadata": {
      "soldAt": "",
      "boughtAt": ""
    }
  },
  {
    "time": "2024-05-19T09:27:52+00:00",
    "type": "Income",
    "amount": 50000.04,
    "currency": "EUR",
    "description": "Job"
  },
  {
    "time": "2024-05-19T09:27:52+00:00",
    "type": "Property",
    "amount": 200000.00,
    "currency": "EUR",
    "description": "House"
  }
]
```

Exporting financial records:

```bash
eTaxesApp export --from=2024-01-1T09:27:52+00:00 --to=2024-05-19T09:27:52+00:00 --types=Income,Stock --output=export.json
```

Performing tax calculation:

```bash
eTaxesApp calculate --types=Income ./taxes.json
```

### Example taxes output file

```json
{
  "records": [
    {
      "type": "Income",
      "amount": 15000.04
    },
    {
      "type": "Stock",
      "amount": 200.0
    },
    {
      "type": "Dividend",
      "amount": 20.0
    },
    {
      "type": "Property",
      "amount": 10000.0
    }
  ]
}
```

## Tech

- C# with .NET framework
- [CLI library](https://github.com/commandlineparser/commandline)
- SQLite
- NLog logger (JSON logging)

## Structure

Using Clean Architecture. Could've also used Clean Architecture with CQRS, but it would've been an overkill for this
simple CLI project. CQRS makes a lot of sense as this CLI application follows command-query separation principle.

I've used async programming in the application, but it's not really necessary for this simple CLI application.

## Considerations

- The app is using SQLite for storing the financial records. It's not the best choice for a production application, but
  as this is a simple CLI application, it's good enough. For a production application, I would use a proper database
  with transaction mechanisms and robust security.
- Missing application configuration. I would add a configuration file for the application, where I would store the
  database connection string, logging level, etc.
- Missing unit tests. I would add unit tests for the application, especially for the core business logic.