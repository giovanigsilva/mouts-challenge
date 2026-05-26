# Testes De Carga Com k6

Os scripts ficam em `tests/load`.

## Scripts

- `k6-smoke.js`: valida health checks.
- `k6-sales-read.js`: executa leituras de Sales.
- `k6-sales-write.js`: cria vendas quando `TOKEN` estiver configurado.
- `k6-sales-mixed.js`: mistura health, leitura e escrita.

## Rodar

```powershell
docker compose --profile loadtest run --rm k6 run /scripts/k6-smoke.js
docker compose --profile loadtest run --rm k6 run /scripts/k6-sales-read.js
```

Com token:

```powershell
$env:K6_TOKEN="<TOKEN_JWT>"
docker compose --profile loadtest run --rm k6 run /scripts/k6-sales-mixed.js
```

## Variaveis

- `K6_BASE_URL`: URL base da API. Padrao dentro do Docker: `http://ambev.developerevaluation.webapi:8080`.
- `K6_TOKEN`: token JWT para endpoints protegidos.
- `K6_SALE_ID`: id de venda para teste de detalhe.
