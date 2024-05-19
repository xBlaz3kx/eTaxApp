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

Exporting financial records:

```bash
eTaxesApp export --from=2024-01-1T09:27:52+00:00 --to=2024-05-19T09:27:52+00:00 --types=Income,Stock --output=export.json
```

Performing tax calculation:

```bash
eTaxesApp calculate --types=Income ./taxes.json
```

## Tech

- C# with .NET framework
- [CLI library](https://github.com/commandlineparser/commandline)
- SQLite
- NLog logger

## Structure

Using Clean Architecture. Could've also used Clean Architecture with CQRS, but it would've been an overkill for this
simple CLI project. CQRS makes a lot of sense as this CLI application follows command-query separation principle.

## Example financial records file structure

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