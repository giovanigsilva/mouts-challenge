[Back to README](../README.md)

## Project Structure

The backend is located under `template/backend` and is structured as follows:

```
root
├── .doc/
├── template/
│   └── backend/
│       ├── src/
│       │   ├── Ambev.DeveloperEvaluation.WebApi
│       │   ├── Ambev.DeveloperEvaluation.Application
│       │   ├── Ambev.DeveloperEvaluation.Domain
│       │   ├── Ambev.DeveloperEvaluation.ORM
│       │   ├── Ambev.DeveloperEvaluation.Common
│       │   └── Ambev.DeveloperEvaluation.IoC
│       ├── tests/
│       │   ├── Ambev.DeveloperEvaluation.Unit
│       │   ├── Ambev.DeveloperEvaluation.Integration
│       │   └── Ambev.DeveloperEvaluation.Functional
│       ├── docs/
│       ├── docker-compose.yml
│       └── Ambev.DeveloperEvaluation.sln
└── README.md
```

Sales was added following the existing template organization:

- Domain: `Entities/Sale.cs`, `Entities/SaleItem.cs`, `Services/SaleDiscountPolicy.cs`, Sales events and `ISaleRepository`
- Application: `Sales/CreateSale`, `Sales/GetSale`, `Sales/ListSales`, `Sales/UpdateSale`, `Sales/DeleteSale`, `Sales/CancelSale`, `Sales/CancelSaleItem`
- WebApi: `Features/Sales/SalesController.cs` and request validators
- ORM: `SaleRepository`, Sales mappings and Sales migration
- Unit tests: Sales domain and validator tests
