# Swagger

Swagger is available locally at:

```text
http://localhost:5119/swagger
https://localhost:7181/swagger
```

The API uses Bearer JWT authentication in Swagger.

Recommended validation flow:

1. Create a user with `POST /api/Users`.
2. Authenticate with `POST /api/Auth`.
3. Copy the token.
4. Click `Authorize`.
5. Test `Vendas` and `Fila e Reprocessamento`.

Important tags:

- Autenticacao
- Usuarios
- Vendas
- Fila e Reprocessamento
- Saude

