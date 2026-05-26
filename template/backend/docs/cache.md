# Cache Redis

O cache e opcional e usado apenas para leitura de vendas.

## Estrategia

- Padrao: cache-aside.
- Fonte da verdade: PostgreSQL.
- Cache: Redis local no profile `enterprise`.
- Configuracao principal: `Cache__Enabled=true`.

## Consultas cacheadas

- `GET /api/sales/{id}`
- `GET /api/sales`

## Escritas nao cacheadas

- `POST /api/sales`
- `PUT /api/sales/{id}`
- `DELETE /api/sales/{id}`
- `PATCH /api/sales/{id}/cancel`
- `PATCH /api/sales/{id}/items/{itemId}/cancel`

## Chaves

```text
sales:v1:details:{saleId}
sales:v1:list:version
sales:v1:list:v{version}:page:{page}:size:{pageSize}:saleNumber:{saleNumber}:customer:{customerId}:branch:{branchId}:cancelled:{isCancelled}:from:{fromDate}:to:{toDate}
```

## Invalidacao

Operacoes de escrita removem o detalhe da venda e incrementam a versao da lista.

## Resiliencia

Se o Redis falhar, a API continua consultando PostgreSQL. A falha e registrada como warning seguro.

## Validacao

```powershell
$env:CACHE_ENABLED="true"
docker compose --profile enterprise up --build -d
curl.exe -i http://localhost:8080/health/cache
```
