# Performance

Esta prova adiciona recursos locais para observar e validar performance sem depender de nuvem.

## Recursos

- Redis para cache de leitura de Sales.
- OpenTelemetry para metricas.
- Prometheus para coleta.
- Grafana para dashboards.
- k6 para carga local.

## Cuidados

- PostgreSQL continua sendo a fonte da verdade.
- Redis e otimizacao, nao dependencia obrigatoria do modo simples.
- Testes k6 devem ser usados com volume moderado em maquina local.

## Comandos principais

```powershell
$env:CACHE_ENABLED="true"
docker compose --profile enterprise up --build -d
docker compose --profile loadtest run --rm k6 run /scripts/k6-smoke.js
```
