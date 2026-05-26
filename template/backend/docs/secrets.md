# Segredos e Vault Local

## Estrategia

Development usa HashiCorp Vault em Docker como cofre local opcional. O Vault roda em modo dev e existe apenas para a prova local; ele nao representa producao.

UAT usa variaveis de ambiente ou placeholders fortes de simulacao no `appsettings.Uat.json`.

Production nao usa Vault local. Segredos obrigatorios devem vir de variaveis de ambiente do host ou, em uma implantacao real Azure, de Azure Key Vault References com Managed Identity.

## Subir Vault Local

```powershell
docker compose up -d ambev.developerevaluation.database vault
```

## Popular Vault Local

PowerShell:

```powershell
$env:VAULT_ADDR="http://localhost:8200"
$env:VAULT_TOKEN="dev-root-token"
.\scripts\vault-init-dev.ps1
```

Linux/macOS:

```bash
export VAULT_ADDR=http://localhost:8200
export VAULT_TOKEN=dev-root-token
./scripts/vault-init-dev.sh
```

## Chaves Gravadas

- `Jwt__SecretKey`
- `Jwt__Issuer`
- `Jwt__Audience`
- `ConnectionStrings__DefaultConnection`

## Variaveis Obrigatorias Em Production

- `ConnectionStrings__DefaultConnection`
- `Jwt__SecretKey`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Security__AllowedOrigins__0`

Production rejeita valores `DevOnly`, `UatOnly`, `ReplaceWith`, `localhost`, token `dev-root-token` e senhas locais conhecidas.

## Health Check

`/health/ready` valida PostgreSQL, configuracoes criticas de seguranca e disponibilidade do Vault local quando `Vault:Enabled=true` em Development.
