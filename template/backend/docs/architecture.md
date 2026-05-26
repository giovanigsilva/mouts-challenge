# Arquitetura

O backend segue uma Clean Architecture simples:

- WebApi recebe HTTP, valida request, chama MediatR e retorna wrappers padronizados.
- Application orquestra casos de uso e validacoes de comando/query.
- Domain contem o aggregate Sale, SaleItem, politica de desconto e eventos de dominio.
- ORM implementa EF Core, mappings, repositories e migrations PostgreSQL.
- Common contem seguranca, logging, validacao e health checks.
- IoC registra dependencias.

As regras de desconto e cancelamento ficam no Domain.
