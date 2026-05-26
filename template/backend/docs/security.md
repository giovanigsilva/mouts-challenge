# Seguranca

- JWT Bearer com validacao de issuer, audience, lifetime e signing key.
- Secret JWT fraco rejeitado em Production.
- Policies para Sales: `Sales.Read`, `Sales.Write`, `Sales.Cancel`, `Sales.Delete`.
- CORS configurado por ambiente.
- HSTS e HTTPS redirection controlados por configuracao.
- Headers `X-Content-Type-Options`, `X-Frame-Options`, `Referrer-Policy` e `X-XSS-Protection`.
- CorrelationId por header `X-Correlation-Id`.
- Erros tratados por middleware global.
- Segredos nao devem ser commitados.
