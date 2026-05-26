# Seguranca

- JWT Bearer com validacao de issuer, audience, lifetime e signing key.
- Secret JWT fraco rejeitado em Production.
- Policies para Sales: `Sales.Read`, `Sales.Write`, `Sales.Cancel`, `Sales.Delete`.
- CORS configurado por ambiente.
- HSTS e HTTPS redirection controlados por configuracao.
- Headers `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` e `X-XSS-Protection`.
- CorrelationId por header `X-Correlation-Id`.
- Erros tratados por middleware global.
- Rate limiting nativo do ASP.NET Core com limite global e policies especificas para Auth e Sales.
- Segredos reais nao devem ser commitados.
- Development pode usar HashiCorp Vault local em Docker para segredos de prova.
- UAT usa variaveis de ambiente ou placeholders fortes de simulacao.
- Production rejeita segredo JWT fraco, CORS aberto, detailed errors, token `dev-root-token`, valores `DevOnly`, `UatOnly`, `ReplaceWith` e connection string local.
- Em producao real, usar Azure Key Vault References ou variaveis seguras do ambiente hospedado.

## Vault Local

Suba o Vault local:

```powershell
docker compose up -d vault
```

Popule os segredos:

```powershell
$env:VAULT_ADDR="http://localhost:8200"
$env:VAULT_TOKEN="dev-root-token"
.\scripts\vault-init-dev.ps1
```

Os scripts gravam somente valores de desenvolvimento marcados como `DevOnly`.
