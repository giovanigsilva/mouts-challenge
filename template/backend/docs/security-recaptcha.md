# reCAPTCHA v3 simulado

Esta prova usa uma protecao anti-bot simulada para login e criacao de usuario sem depender de Google, internet, site key ou secret key.

## Endpoints protegidos

- `POST /api/Auth`
- `POST /api/Users`

Sales, health checks e Swagger nao usam reCAPTCHA nesta etapa.

## Configuracao

Backend:

```text
Recaptcha__Enabled=true
Recaptcha__Provider=Simulated
Recaptcha__MinimumScore=0.5
Recaptcha__Simulated__Enabled=true
Recaptcha__Simulated__AcceptedTokenPrefix=simulated
Recaptcha__Simulated__TokenLifetimeSeconds=120
Recaptcha__Simulated__DefaultScore=0.9
Recaptcha__Simulated__ForceFailure=false
Recaptcha__Actions__Login=login
Recaptcha__Actions__CreateUser=create_user
```

Frontend:

```text
VITE_RECAPTCHA_ENABLED=true
VITE_RECAPTCHA_PROVIDER=simulated
VITE_RECAPTCHA_LOGIN_ACTION=login
VITE_RECAPTCHA_CREATE_USER_ACTION=create_user
```

## Token simulado

Formato:

```text
simulated:{action}:{unixTimestampSeconds}:{nonce}
```

Exemplos:

```text
simulated:login:1779800000:0f5c4c8e
simulated:create_user:1779800000:0f5c4c8e
```

## Validacoes do backend

Quando `Recaptcha:Enabled=true`, o backend valida:

- token presente;
- prefixo `simulated`;
- action esperada;
- timestamp valido;
- token dentro da janela configurada;
- score simulado maior ou igual ao threshold;
- `ForceFailure=false`.

Quando `Recaptcha:Enabled=false`, o fluxo existente continua funcionando sem token.

## Falha simulada

```powershell
$env:RECAPTCHA_ENABLED="true"
$env:Recaptcha__Simulated__ForceFailure="true"
$env:VITE_RECAPTCHA_ENABLED="true"
docker compose up --build -d ambev.developerevaluation.webapi frontend
```

Login e cadastro devem retornar mensagem segura:

```text
Nao foi possivel validar a protecao anti-robo. Tente novamente.
```

## Logs

Eventos registrados:

- `RecaptchaDisabled`
- `RecaptchaVerificationSucceeded`
- `RecaptchaVerificationFailed`
- `RecaptchaSimulatedForcedFailure`
- `RecaptchaLoginRejected`
- `RecaptchaCreateUserRejected`

O token completo nao e logado.

## Evolucao futura para Google v3

Para producao real:

- implementar `GoogleRecaptchaVerifier`;
- configurar site key no frontend;
- configurar secret key no backend por provedor seguro;
- trocar `Recaptcha:Provider=GoogleV3`;
- manter as actions `login` e `create_user`.
