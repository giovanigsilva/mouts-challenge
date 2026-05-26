# Observabilidade Local

A stack local combina logs e metricas sem depender de nuvem.

## Logs

- Serilog escreve console JSON.
- Seq e o fallback local pesquisavel.
- Datadog Agent e opcional e coleta stdout/stderr quando `DD_API_KEY` estiver configurada.

## Metricas

- OpenTelemetry coleta metricas ASP.NET Core, HttpClient e runtime.
- `/metrics` expoe metricas para Prometheus.
- Prometheus coleta API, PostgreSQL exporter e Redis exporter.
- Grafana provisiona dashboards locais.

## URLs

```text
Seq:        http://localhost:5341
Prometheus: http://localhost:9090
Grafana:    http://localhost:3000
```

Credencial local do Grafana:

```text
admin / developerstore
```

## Subir stack enterprise

```powershell
$env:CACHE_ENABLED="true"
docker compose --profile enterprise up --build -d
```

## Validar metricas

```powershell
curl.exe -i http://localhost:8080/metrics
curl.exe -i http://localhost:8080/health/metrics
```
