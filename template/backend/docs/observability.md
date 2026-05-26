# Observabilidade

## Estrategia

- Serilog e o logger principal da aplicacao.
- Console JSON fica sempre ativo.
- Datadog Agent coleta logs dos containers via stdout/stderr.
- Seq e o unico fallback local pesquisavel.
- Nao ha File Sink, Loki, Elasticsearch, OpenSearch, Application Insights, Redis, Service Bus ou Azure real nesta feature.

## Docker

Suba os servicos:

```powershell
docker compose up --build
```

Servicos esperados:

- `ambev.developerevaluation.webapi`
- `ambev.developerevaluation.database`
- `seq`
- `datadog-agent`

Seq:

```text
http://localhost:5341
```

Datadog usa `DD_API_KEY` via `.env`. Se a chave nao existir, a API continua funcionando e o Seq permanece disponivel localmente.

## Health

```text
GET /health/live
GET /health/ready
GET /health/logging
```

`/health/logging` informa apenas flags seguras: Datadog habilitado, Seq habilitado, URL do Seq configurada, console logging ativo, ambiente e service name. Nao retorna API key nem segredo.

## Campos De Rastreabilidade

Logs HTTP incluem:

- `CorrelationId`
- `TraceId`
- `SpanId`
- `Environment`
- `RequestPath`
- `RequestMethod`
- `RouteName`
- `StatusCode`
- `ElapsedMilliseconds`
- `ResultStatus`
- `UserId`
- `UserName`
- `UserEmail`
- `UserRole`
- `RemoteIpAddress`
- `UserAgent`
- `RequestId`

Logs de Sales incluem, quando aplicavel:

- `SaleId`
- `SaleNumber`
- `CustomerExternalId`
- `BranchExternalId`
- `TotalAmount`

## Falhas De Observabilidade

- Datadog Agent coleta stdout; se o Agent estiver fora, a API continua escrevendo no console.
- Seq usa sink assíncrono; se estiver fora, a API continua atendendo requests.
- Serilog SelfLog fica habilitado apenas em Development quando configurado.
