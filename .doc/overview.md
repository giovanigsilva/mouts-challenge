[Back to README](../README.md)

## Overview
This project serves as an evaluation for senior developer candidates. It is designed to assess various skills and competencies required for a senior developer role, including but not limited to:

1. Proficiency in C# and .NET 8.0 development
2. Project Layer Separation
3. Database skills with both PostgreSQL and MongoDB
4. Understanding and implementation of design patterns (e.g., Mediator pattern)
5. Ability to work with object-relational mapping tools (EF Core)
6. Proficiency in writing and maintaining unit tests using xUnit
7. Experience with mocking frameworks like NSubstitute
8. Familiarity with object mapping libraries such as AutoMapper
9. API design and RESTful service implementation
10. Version control with Git 
11. Understanding of both relational and non relational database systems
12. Data generation and management for testing purposes (using Faker)
13. Code organization and project structure
14. Implementation of pagination, filtering, and sorting in APIs
15. Error handling and API response formatting
16. Use of Git Flow and Semantic Commits
17. Performance optimization for database queries and API responses
18. Understanding of asynchronous programming patterns
19. Code quality and adherence to best practices
20. Problem-solving and analytical skills
21. Attention to detail in implementing business logic
22. Ability to work with and integrate multiple technologies and frameworks

For this implementation, the backend was extended with a complete Sales API for the DeveloperStore challenge. The solution now covers:

1. Sales CRUD with sale number, date, customer snapshot, branch snapshot, products, quantities, unit prices, discounts, item totals and cancellation status
2. DDD-style Sales aggregate with discount and cancellation rules in the Domain layer
3. External Identities pattern for Customer, Branch and Product references
4. PostgreSQL persistence through EF Core and Npgsql
5. Environment separation for Development, UAT and Production
6. JWT authentication and authorization policies for Sales
7. Structured application logs for SaleCreated, SaleModified, SaleCancelled and ItemCancelled events
8. Swagger/OpenAPI documentation for Sales in Brazilian Portuguese
9. Docker Compose support for WebApi and PostgreSQL
10. Unit tests for Sales domain rules and validators


This comprehensive evaluation aims to assess both the technical proficiency and the broader software engineering skills necessary for a senior developer role.

<br/>
<div style="display: flex; justify-content: space-between;">
  <a href="../README.md">Previous: Read Me</a>
  <a href="./tech-stack.md">Next: Tech Stack</a>
</div>
