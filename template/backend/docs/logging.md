# Logging

## Estrategia Principal E Fallback

Datadog e o coletor principal via Agent Docker. A aplicacao nao depende de sink direto do Datadog.

Seq e o unico fallback local para consulta de logs:

```text
http://localhost:5341
```

Nao ha File Sink nesta etapa.

## Pesquisas No Seq

Por correlationId:

```text
CorrelationId = '11111111-1111-1111-1111-111111111111'
```

Por usuario:

```text
UserId = 'user-id'
```

Por venda:

```text
SaleId = 'sale-id'
```

ou:

```text
SaleNumber = 'SALE-2026-000001'
```

Por caso de uso:

```text
RequestName = 'CreateSaleCommand'
```

## Eventos Positivos

- `HTTP request received`
- `HTTP request finished successfully`
- `UseCase started`
- `UseCase completed`
- `LoginSuccess`
- `TokenIssued`
- `UserCreated`
- `UserRetrieved`
- `UserDeleted`
- `SaleCreated`
- `SaleUpdated`
- `SaleCancelled`
- `SaleItemCancelled`
- `SaleDeleted`
- `SalesListed`

## Eventos Negativos

- `HTTP request finished with client error`
- `HTTP request finished with server error`
- `UseCase failed`
- `LoginFailed`
- `UserNotFound`
- `UserCreateFailed`
- `UserDeleteFailed`
- `SaleGetFailed`
- `SaleCreateFailed`
- `SaleUpdateFailed`
- `SaleCancelFailed`
- `SaleItemCancelFailed`
- `SaleDeleteFailed`

## Sanitizacao

Nunca logar:

- senha
- password
- token
- JWT
- Authorization header
- secret
- connection string
- DD_API_KEY
- API key
- credential

O helper `SensitiveDataMasker` mascara chaves ou texto contendo termos sensiveis.

## Como Validar

1. Execute `docker compose up --build`.
2. Abra `http://localhost:5341`.
3. Chame `/health/live` com `X-Correlation-Id`.
4. Pesquise no Seq pelo valor do correlationId.
5. Crie uma venda e pesquise por `SaleNumber`.
