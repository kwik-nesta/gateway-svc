![Docker Image Version](https://img.shields.io/docker/v/blueclikk/kwik-nesta.gateway.svc?sort=semver&label=version)
![Docker Pulls](https://img.shields.io/docker/pulls/blueclikk/kwik-nesta.gateway.svc)


# KwikNesta Gateway Service

The **Gateway Service** acts as the API gateway for the KwikNesta platform. It receives requests from clients, validates and routes them to appropriate downstream microservices using Refit-based HTTP clients. It also applies cross-cutting concerns like authentication, logging, and error handling.

---

## 🚀 Features

- API Gateway for routing client requests
- Refit integration for service-to-service communication
- Global exception handling and response wrapping
---

## 🛠️ Requirements

- .NET 8.0 SDK
- Access to downstream microservices (Identity, Property, Payment, etc.)
- Properly configured Refit clients in `appsettings.json`
- Shared contracts via `KwikNesta.Contracts` package

---

## ⚙️ Configuration

Example `appsettings.json` snippet:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "connection string"
  },
  "HangfireSettings": {
    "UserName": "kn-hf-admin",
    "Password": "P@33w0rd"
  },
  "KwikNestaServers": {
    "IdentityService": "https://localhost:7149",
    "SystemSupportService": "https://localhost:5002",
    "PaymentService": "https://localhost:7112",
    "PropertyService": "https://localhost:7292",
    "ExternalLocationClient": "https://locations-marker.onrender.com"
  },
  "CrossQueueHub": {
    "RabbitMQ": {
      "ConnectionString": "rabbit mq connection string",
      "DefaultExchange": "exchange name",
      "DefaultExchangeType": "direct",
      "Durable": true,
      "PublishRetryCount": 5,
      "PublishRetryDelayMs": 200,
      "ConsumerRetryCount": 5,
      "ConsumerRetryDelayMs": 500,
      "DeadLetterExchange": "dlx"
    }
  },
  "DryMailClient": {
    "ApiKey": "mail jet api key",
    "ApiSecret": "mail jet api secret",
    "Email": "email address",
    "AppName": "Kwik Nesta Inc."
  },
  "ElasticSearch": {
    "Url": "elastic search url"
  },
  "Cors": {
    "Origins": [
      "https://kwik-nesta.com",
      "https://localhost:7055"
    ]
  },
  "Jwt": {
    "IdentityService": "https://localhost:7149",
    "Issuer": "issuer",
    "Audience": "audience",
    "RoleClaim": "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
    "IssuerSigningKey": "signing-key"
  }
}
```

Ensure the enums and DTOs match across all services using the shared contracts package.
---

## 🧰 Tech Stack

- ASP.NET Core 8
- Refit
- Hangfire
- System.Text.Json

---

## 📜 License

MIT License © Kwik Nesta