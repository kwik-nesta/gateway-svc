![Docker Image Version](https://img.shields.io/docker/v/blueclikk/kwik-nesta.payment.svc?sort=semver&label=version)
![Docker Pulls](https://img.shields.io/docker/pulls/blueclikk/kwik-nesta.payment.svc)


# 💳 Payment Service

The **Payment Service** is responsible for managing and processing all payment-related operations.  
It enables seamless and secure handling of transactions, refunds, and payment methods, and integrates with third-party payment gateways.

---

## 🚀 Features

- 🔐 **Secure Payments**
  - Process one-time or recurring payments
  - PCI-compliant tokenized payment storage

- 💸 **Refund Management**
  - Initiate full or partial refunds
  - Track refund status

- 👛 **Payment Methods**
  - Add and manage credit/debit cards
  - Support for bank transfers, wallets, and third-party gateways

- 📊 **Transaction History**
  - Retrieve transaction logs for audit and reporting
  - Built-in filtering and pagination

- ⚡ **Gateway Integration**
  - Plug-and-play support for providers like **Stripe, Paystack, Flutterwave, PayPal** (extensible)

---

## 📡 API Endpoints

### 💳 Payments
| Method | Endpoint                  | Description |
|--------|---------------------------|-------------|
| POST   | `/payments`               | Initiate a new payment |
| GET    | `/payments/{id}`          | Get details of a specific payment |
| GET    | `/payments`               | List all payments (paginated) |

### 💸 Refunds
| Method | Endpoint                   | Description |
|--------|----------------------------|-------------|
| POST   | `/refunds`                 | Create a refund for a payment |
| GET    | `/refunds/{id}`            | Get refund details |
| GET    | `/refunds`                 | List refunds (paginated) |

### 👛 Payment Methods
| Method | Endpoint                   | Description |
|--------|----------------------------|-------------|
| POST   | `/methods`                 | Add a new payment method |
| GET    | `/methods/{id}`            | Get details of a payment method |
| GET    | `/methods`                 | List all saved payment methods |
| DELETE | `/methods/{id}`            | Remove a payment method |

### 📊 Transactions
| Method | Endpoint                   | Description |
|--------|----------------------------|-------------|
| GET    | `/transactions`            | List all transactions |
| GET    | `/transactions/{id}`       | Get details of a transaction |

---

## 📥 Pagination & Query

All list endpoints support query parameters:

- `pageNumber` *(int, default: 1)*  
- `pageSize` *(int, default: 20)*  
- `status` *(string, optional — e.g., Success, Failed, Pending)*  
- `dateRange` *(optional, start & end dates)*  

---

## 📤 Sample Responses

### ✅ Successful Payment
```json
{
  "id": "pay_7f92...",
  "amount": 5000,
  "currency": "NGN",
  "status": "Success",
  "method": "Card",
  "timestamp": "2025-09-01T10:45:23Z"
}
```

## ⚙️ Tech Stack
- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core (Data Access)
- SQL Database (Persistence)
- Swagger / OpenAPI (API Documentation)

---

## ⚡ gRPC Integration

This service depends on `SystemSupportService` to fetch location and `IndentityService` to fetch user details.

```csharp
// Example registration in Program.cs
builder.Services.AddGrpcClient<LocationService.LocationServiceClient>(o =>
{
    o.Address = new Uri("https://system-support-service:5001"); // Adjust per environment
});

// Example registration in Program.cs
builder.Services.AddGrpcClient<IdentityService.IdentityServiceClient>(o =>
{
    o.Address = new Uri("https://identity-service:5001"); // Adjust per environment
});
```

## 📜 License

MIT License © Kwik Nesta