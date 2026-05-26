# Swagger/OpenAPI

Este guia descreve como usar a documentacao OpenAPI da DeveloperStore Sales API.

## Objetivo

O Swagger e a documentacao principal da API durante a avaliacao. Ele descreve os endpoints reais do backend, os envelopes de resposta, a autenticacao JWT, as regras de vendas, os filtros de consulta, os erros padronizados e os ambientes suportados.

## Como abrir

Com Docker:

```text
http://localhost:8080/swagger
```

Com `dotnet run` local:

```text
http://localhost:5119/swagger
```

O Swagger fica habilitado em Development e UAT. Em Production, ele fica controlado por configuracao e deve permanecer desabilitado por padrao em uso real.

## Como autenticar

1. Crie um usuario em `POST /api/Users`, se ainda nao existir.
2. Autentique em `POST /api/Auth`.
3. Copie o valor `data.token` retornado.
4. Clique em `Authorize`.
5. Informe o token no formato:

```text
Bearer {token}
```

Nao use segredo JWT nem token fixo em arquivos do projeto.

## Tags

Os endpoints estao organizados em tags em portugues:

- `Autenticacao`: login e emissao de token JWT.
- `Usuarios`: criacao, consulta e remocao de usuarios.
- `Vendas`: CRUD de vendas.
- `Cancelamentos`: cancelamento de venda e cancelamento de item.
- `Saude`: descricao documental dos health checks da API.

Os health checks existem em `/health/live` e `/health/ready`. Eles sao registrados pela infraestrutura de health checks e podem nao aparecer como actions de controller no Swagger.

## OperationId

As principais operacoes usam identificadores estaveis:

```text
Auth_AuthenticateUser
Users_CreateUser
Users_GetUserById
Users_DeleteUser
Sales_CreateSale
Sales_ListSales
Sales_GetSaleById
Sales_UpdateSale
Sales_DeleteSale
Sales_CancelSale
Sales_CancelSaleItem
```

## Padrão de resposta

As respostas seguem os envelopes reais do template:

```json
{
  "success": true,
  "message": "Operacao realizada com sucesso.",
  "data": {}
}
```

Listagens paginadas retornam:

```json
{
  "success": true,
  "message": "Vendas listadas com sucesso.",
  "data": [],
  "currentPage": 1,
  "totalPages": 1,
  "totalCount": 1
}
```

Erros tratados pelo middleware global incluem `correlationId` e `timestamp` para rastreabilidade.

## Erros documentados

O Swagger documenta respostas de erro sem expor stack trace, SQL, connection string, JWT ou segredo:

- `400`: validacao invalida ou violacao de regra de negocio.
- `401`: autenticacao ausente ou invalida.
- `403`: usuario autenticado sem permissao suficiente, quando aplicavel.
- `404`: recurso nao encontrado.
- `500`: erro inesperado tratado pelo middleware global.

O projeto nao documenta `ProblemDetails` porque o envelope real e baseado em `ApiResponse`.

## Header X-Correlation-Id

Todas as operacoes documentam o header opcional:

```text
X-Correlation-Id: a94fb91b-63e4-46c6-bd3d-82a89f7e1a4d
```

Se o cliente nao enviar o header, a API gera um identificador automaticamente e usa esse valor nos logs e respostas de erro.

## Regras de desconto

O Swagger documenta as regras reais do dominio Sales:

- 1 a 3 unidades do mesmo produto: sem desconto.
- 4 a 9 unidades do mesmo produto: 10% de desconto.
- 10 a 20 unidades do mesmo produto: 20% de desconto.
- Acima de 20 unidades do mesmo produto: operacao invalida.

O request de venda nao aceita desconto manual. O dominio calcula percentual, valor de desconto, total do item e total da venda.

## External Identities

A API de Sales nao cria dominios reais de cliente, filial ou produto. A venda armazena snapshots denormalizados:

- `customerExternalId`
- `customerName`
- `branchExternalId`
- `branchName`
- `productExternalId`
- `productName`

Esses campos representam a informacao conhecida no momento da venda.

## Eventos logados

Nesta entrega da prova, eventos sao registrados no log da aplicacao:

- `SaleCreated`
- `SaleModified`
- `SaleCancelled`
- `ItemCancelled`

Nao ha publicacao real em broker, fila, Service Bus, Azure Functions, worker ou Outbox nesta versao.

## Como testar Sales pelo Swagger

1. Abra `/swagger`.
2. Crie um usuario em `POST /api/Users`.
3. Autentique em `POST /api/Auth`.
4. Autorize com `Bearer {token}`.
5. Execute `POST /api/sales` usando o exemplo exibido no Swagger.
6. Execute `GET /api/sales` para listar vendas.
7. Execute `GET /api/sales/{id}` para consultar a venda.
8. Execute `PUT /api/sales/{id}` para atualizar a venda.
9. Execute `PATCH /api/sales/{id}/items/{itemId}/cancel` para cancelar um item.
10. Execute `PATCH /api/sales/{id}/cancel` para cancelar a venda.

## Exemplo de venda

```json
{
  "saleNumber": "SALE-2026-000001",
  "saleDate": "2026-05-26T14:30:00Z",
  "customerExternalId": "5c9d7b1e-2a63-4e69-9c55-4c0e8142f8c1",
  "customerName": "Joao da Silva",
  "branchExternalId": "7a2b2c71-6c2e-4f54-8a7e-32159a4d53e2",
  "branchName": "Loja Centro - Sao Paulo",
  "items": [
    {
      "productExternalId": "33a8b4f9-4a6e-49c9-91df-ec7b40b3b1a1",
      "productName": "Camiseta DeveloperStore",
      "quantity": 4,
      "unitPrice": 50.00
    },
    {
      "productExternalId": "e7cb8e84-2c77-4020-bf2a-74cfce2b67cb",
      "productName": "Caneca DeveloperStore",
      "quantity": 10,
      "unitPrice": 25.00
    }
  ]
}
```

Totais esperados:

- Camiseta: `4 * 50.00 = 200.00`, desconto de 10%, total `180.00`.
- Caneca: `10 * 25.00 = 250.00`, desconto de 20%, total `200.00`.
- Total da venda: `380.00`.

## Ambientes

Development:

- Swagger habilitado.
- Detailed errors habilitados.
- CORS local.

UAT:

- Swagger habilitado para avaliacao.
- Detailed errors limitados.
- CORS controlado por configuracao.

Production:

- Swagger controlado por `Swagger:Enabled`.
- Detailed errors desabilitados.
- Segredos via variaveis de ambiente ou mecanismo externo seguro.
- Nenhum segredo real deve existir em `appsettings.Production.json`.

## Troubleshooting

Se o Swagger nao abrir:

1. Confirme o ambiente com `ASPNETCORE_ENVIRONMENT`.
2. Confirme `Swagger__Enabled=true` em Development ou UAT.
3. Verifique se a API esta ouvindo na porta esperada.
4. Consulte `/health/live`.
5. Consulte os logs da aplicacao.

Se endpoints Sales retornarem `401`:

1. Autentique novamente em `POST /api/Auth`.
2. Copie `data.token`.
3. Clique em `Authorize`.
4. Informe `Bearer {token}`.

Se a criacao de venda retornar `400`:

1. Verifique campos obrigatorios.
2. Verifique se `quantity` esta entre 1 e 20.
3. Verifique se nao ha produto duplicado na mesma venda.
4. Verifique se `unitPrice` e maior que zero.
