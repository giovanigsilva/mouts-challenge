[Back to README](../README.md)

### Sales

The Sales API implements the DeveloperStore sales challenge using DDD and the External Identities pattern. Customer, Branch and Product are not modeled as independent domains. Their external identifiers and display names are stored as denormalized snapshots in the Sale aggregate.

#### Business Rules

- Quantity from 1 to 3 identical items receives 0% discount.
- Quantity from 4 to 9 identical items receives 10% discount.
- Quantity from 10 to 20 identical items receives 20% discount.
- Quantity above 20 identical items is invalid.
- The same `productExternalId` cannot be repeated in the same sale.
- Cancelled items do not compose the sale total.
- Cancelled sales cannot be modified.
- Totals and discounts are calculated by the Domain layer.

#### Events

The implementation logs the following events in the application log:

- SaleCreated
- SaleModified
- SaleCancelled
- ItemCancelled

No message broker is required for this simplified challenge scope.

#### POST /api/sales

- Description: Create a new sale.
- Authentication: Bearer token required.
- Authorization policy: `Sales.Write`.
- Response: `201 Created`.

Request body:

```json
{
  "saleNumber": "SALE-2026-000001",
  "saleDate": "2026-05-26T12:00:00Z",
  "customerExternalId": "11111111-1111-1111-1111-111111111111",
  "customerName": "Cliente Exemplo",
  "branchExternalId": "22222222-2222-2222-2222-222222222222",
  "branchName": "Filial Centro",
  "items": [
    {
      "productExternalId": "33333333-3333-3333-3333-333333333333",
      "productName": "Produto Exemplo",
      "quantity": 10,
      "unitPrice": 100
    }
  ]
}
```

Response:

```json
{
  "success": true,
  "message": "Venda criada com sucesso.",
  "data": {
    "id": "uuid",
    "saleNumber": "SALE-2026-000001",
    "totalAmount": 800,
    "isCancelled": false,
    "items": [
      {
        "id": "uuid",
        "quantity": 10,
        "unitPrice": 100,
        "discountPercentage": 20,
        "discountAmount": 200,
        "totalAmount": 800,
        "isCancelled": false
      }
    ]
  }
}
```

#### GET /api/sales

- Description: List sales with pagination and optional filters.
- Authentication: Bearer token required.
- Authorization policy: `Sales.Read`.

Query parameters:

- `page`
- `pageSize`
- `saleNumber`
- `customerId`
- `branchId`
- `isCancelled`
- `fromDate`
- `toDate`

Example:

```
GET /api/sales?page=1&pageSize=20&isCancelled=false
```

#### GET /api/sales/{id}

- Description: Retrieve one sale by ID.
- Authentication: Bearer token required.
- Authorization policy: `Sales.Read`.

#### PUT /api/sales/{id}

- Description: Update sale data and replace its items.
- Authentication: Bearer token required.
- Authorization policy: `Sales.Write`.
- Rule: cancelled sales cannot be updated.

#### DELETE /api/sales/{id}

- Description: Physically remove a sale.
- Authentication: Bearer token required.
- Authorization policy: `Sales.Delete`.

This is different from cancellation. Sale cancellation uses `PATCH /api/sales/{id}/cancel`.

#### PATCH /api/sales/{id}/cancel

- Description: Cancel a sale.
- Authentication: Bearer token required.
- Authorization policy: `Sales.Cancel`.
- Event logged: SaleCancelled.

#### PATCH /api/sales/{id}/items/{itemId}/cancel

- Description: Cancel one sale item and recalculate the sale total.
- Authentication: Bearer token required.
- Authorization policy: `Sales.Cancel`.
- Event logged: ItemCancelled.

<br/>
<div style="display: flex; justify-content: space-between;">
  <a href="./general-api.md">Previous: General API</a>
  <a href="./products-api.md">Next: Products API</a>
</div>
