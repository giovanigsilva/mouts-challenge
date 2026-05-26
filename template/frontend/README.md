# DeveloperStore Frontend

Frontend React para demonstrar a API DeveloperStore Sales.

## Stack

- React
- TypeScript
- Vite
- React Router
- TanStack Query
- React Hook Form
- Zod
- Tailwind CSS
- shadcn/ui style components com Radix UI
- Motion for React
- Axios
- Lucide React
- Sonner

## Configuracao

Crie o arquivo `.env` a partir do exemplo:

```powershell
Copy-Item .env.example .env
```

Conteudo esperado:

```text
VITE_API_BASE_URL=http://localhost:8080
VITE_APP_NAME=DeveloperStore Frontend
VITE_APP_ENV=development
VITE_RECAPTCHA_ENABLED=false
VITE_RECAPTCHA_PROVIDER=simulated
VITE_RECAPTCHA_LOGIN_ACTION=login
VITE_RECAPTCHA_CREATE_USER_ACTION=create_user
```

## Rodar localmente

Suba o backend primeiro:

```powershell
cd ..\backend
docker compose up --build -d
```

Rode o frontend:

```powershell
cd ..\frontend
npm install
npm run dev
```

Abra:

```text
http://localhost:5173
```

## Login rápido

Para entrar na aplicação usando as credenciais de demonstração:

1. Acesse `http://localhost:5173/`.
2. Clique em `Usar demo`.
3. O formulário será preenchido com:

```text
E-mail: admin@developerstore.com
Senha:  Senha@123456
```

4. Clique em `Entrar`.

Após o login, o frontend salva o JWT localmente e envia `Authorization: Bearer {token}` automaticamente nas chamadas da API.

## Rodar com Docker

O compose fica no backend e tambem carrega o frontend:

```powershell
cd ..\backend
docker compose up --build -d frontend
```

Ou subir tudo:

```powershell
cd ..\backend
docker compose up --build -d
```

Frontend:

```text
http://localhost:5173
```

API usada pelo build Docker:

```text
http://localhost:8080
```

## Comandos

```powershell
npm install
npm run dev
npm run lint
npm run build
npm run preview
```

## Fluxo de teste

1. Acesse `http://localhost:5173`.
2. Clique em `Usar demo`.
3. Clique em `Entrar`.
4. Acesse `Vendas`.
5. Crie uma venda em `Nova venda`.
6. Consulte detalhes da venda.
7. Edite uma venda ativa.
8. Cancele item da venda.
9. Cancele a venda.
10. Remova uma venda.
11. Acesse `Saude da API`.

## Regras de desconto no frontend

O frontend exibe preview local:

- 1 a 3 unidades: 0%.
- 4 a 9 unidades: 10%.
- 10 a 20 unidades: 20%.
- Produto duplicado e bloqueado pelo formulario.

O backend continua sendo a fonte da verdade para regras, descontos e totais.

## Arquitetura

```text
src/app
src/shared/api
src/shared/components
src/shared/config
src/shared/hooks
src/shared/lib
src/shared/types
src/features/auth
src/features/users
src/features/sales
src/features/dashboard
src/features/health
```

Camadas:

- `app`: bootstrap, providers e rotas.
- `shared/api`: client HTTP, token, correlationId e erros.
- `shared/components`: UI reutilizavel, layout, feedback e glassmorphism.
- `features`: Auth, Users, Sales, Dashboard e Health.

## Autenticacao

- Login consome `POST /api/Auth`.
- Token JWT e salvo em `localStorage` para simplicidade da prova.
- O client envia `Authorization: Bearer {token}` automaticamente.
- O client envia `X-Correlation-Id` automaticamente.
- Resposta 401 limpa o token e redireciona para `/login`.

## reCAPTCHA v3 simulado

O frontend possui provider simulado para proteger login e criacao de usuario sem Google real.

Para habilitar:

```powershell
$env:VITE_RECAPTCHA_ENABLED="true"
npm run dev
```

No Docker, defina tambem no backend:

```powershell
$env:RECAPTCHA_ENABLED="true"
$env:VITE_RECAPTCHA_ENABLED="true"
cd ..\backend
docker compose up --build -d ambev.developerevaluation.webapi frontend
```

O token e gerado no submit no formato `simulated:{action}:{timestamp}:{nonce}` e descartado apos o envio.

## Observacoes

- Nao existe token hardcoded.
- Nao existe segredo no frontend.
- O Docker do frontend usa build estatico com nginx.
