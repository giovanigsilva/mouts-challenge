# Queue And Reprocessing

Outbox statuses:

- `Pending`
- `Processing`
- `Published`
- `Failed`
- `DeadLettered`

Administrative endpoints:

```http
GET    /api/admin/outbox/messages
GET    /api/admin/outbox/messages/{id}
POST   /api/admin/outbox/messages/{id}/retry
POST   /api/admin/outbox/messages/{id}/deadletter
POST   /api/admin/outbox/messages/{id}/reset
POST   /api/admin/outbox/retry-failed
POST   /api/admin/outbox/retry-deadlettered
GET    /api/admin/outbox/stats
```

Security:

- Requires JWT.
- Requires `Queue.Admin` policy.
- Uses role `Admin` from the template.
- Records administrative action in `OutboxAdminActions`.
- Does not expose full payload in listing/detail responses.

Retry behavior:

- `retry` moves one Failed/DeadLettered message to Pending.
- `retry-failed` moves a limited batch of Failed messages to Pending.
- `retry-deadlettered` moves a limited batch of DeadLettered messages to Pending.
- `deadletter` marks a message manually as DeadLettered.
- `reset` clears lock and moves the message to Pending.

