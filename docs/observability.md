# Observability

The project keeps Serilog as the base logger and adds correlation id propagation through `X-Correlation-Id`.

Recommended production setup:

- Datadog as primary observability provider.
- Application Insights as fallback telemetry.
- JSON logs in containers.
- Correlation id in every request and error response.
- Worker logs for publish success and failure.
- Function logs for processed and duplicated events.

Expected environment variables:

```text
DD_SERVICE=developerstore-sales-api
DD_ENV=development
DD_VERSION=1.0.0
DD_API_KEY
DD_SITE
DD_LOGS_INJECTION=true
DD_TRACE_ENABLED=true
APPLICATIONINSIGHTS_CONNECTION_STRING
```

Recommended metrics:

- `sales.created.count`
- `sales.modified.count`
- `sales.cancelled.count`
- `sales.item_cancelled.count`
- `cache.hit.count`
- `cache.miss.count`
- `outbox.pending.count`
- `outbox.failed.count`
- `servicebus.publish.success.count`
- `servicebus.publish.failure.count`

