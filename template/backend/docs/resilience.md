# Resiliencia

## Tratamento Global

O `GlobalExceptionMiddleware` trata excecoes HTTP e retorna envelope padronizado com `correlationId`.

Mapeamento:

- `ValidationException`: 400
- `DomainException`: 400
- `KeyNotFoundException`: 404
- `UnauthorizedAccessException`: 401
- `InvalidOperationException`: 400
- `OperationCanceledException`: 499
- `TimeoutException`: 408
- `DbUpdateConcurrencyException`: 409
- `DbUpdateException`: 500 com mensagem segura
- `Exception`: 500 com mensagem segura

## CancellationToken

Controllers propagam `CancellationToken` para MediatR. Handlers propagam para repositories. Repositories usam o token nas chamadas async do EF Core.

Cancelamento pelo cliente nao vira erro 500 generico.

## Observabilidade

Falha no Datadog Agent nao derruba a API porque a coleta e feita via stdout/stderr.

Falha no Seq nao derruba a API porque o sink e assíncrono e usado como fallback local.

## Logs Seguros

Erros retornam mensagens seguras fora de Development. Mensagens com termos sensiveis sao mascaradas pelo `SensitiveDataMasker`.
