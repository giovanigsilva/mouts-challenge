# AGENTS.md

## Politica De Execucao Para IA

Voce deve atuar com extremo rigor, precisao e conservadorismo tecnico.

## Regras Obrigatorias

### 1. Escopo estrito

- Faca somente o que foi solicitado.
- Nao altere arquivos, fluxos, nomes, contratos, comportamento, estrutura, arquitetura, formatacao ou logica fora do escopo pedido.
- Nao aproveite a tarefa para melhorar, refatorar, padronizar, limpar, modernizar ou organizar partes nao solicitadas.
- Se identificar algo estranho fora do escopo, apenas registre em observacao curta ao final, sem modificar.

### 2. Proibido simular

- Nao simule implementacao.
- Nao simule testes.
- Nao invente classes, metodos, servicos, endpoints, DTOs, tabelas, colunas, migrations, variaveis, arquivos, handlers, eventos, testes ou integracoes que nao existam no projeto.
- Nao assuma comportamento sem confirmar no codigo existente.
- Antes de alterar, localize a implementacao real e trabalhe em cima dela.
- Se algo nao existir, diga explicitamente que nao existe e mostre o impacto disso, sem criar solucao ficticia por conta propria.

### 3. Reaproveitamento obrigatorio

- Sempre reutilize o padrao ja adotado no projeto.
- Sempre reutilize codigo ja existente antes de criar algo novo.
- Se ja existir rota, servico, repositorio, teste, helper, mapper, validador ou padrao semelhante, siga exatamente a convencao existente.
- Evite duplicacao.
- Nao crie nova abordagem se ja houver uma consolidada no projeto.

### 4. Separacao de responsabilidades

- Todo codigo deve manter separacao clara de responsabilidades por classes, pastas e camadas relacionadas.
- Program.cs deve permanecer limpo e restrito ao bootstrap da aplicacao.
- Configuracoes de servicos, pipeline HTTP, Swagger, health checks, middlewares, features, validacoes, persistencia e cross-cutting concerns devem ficar em classes ou extensoes proprias nas pastas correspondentes.
- Nao concentrar regras, configuracoes extensas ou fluxos de negocio em arquivos generalistas quando ja existir uma pasta ou camada relacionada.
- Ao criar ou mover codigo, preservar o padrao real existente e fazer a menor alteracao necessaria.

### 5. Testes

- Sempre verifique primeiro se ja existem testes para a rota, servico, caso de uso, handler ou componente especifico relacionado a alteracao.
- Se existirem testes, reaproveite o mesmo padrao, estrutura, fixtures, builders, mocks, nomenclatura e organizacao.
- Prefira ajustar ou complementar testes existentes em vez de criar testes paralelos desnecessarios.
- Nao invente cenario de teste que contradiga o comportamento real do codigo.
- Se nao houver teste para aquele ponto, crie somente se for realmente necessario para validar a alteracao pedida.
- Nunca declare testes como executados sem executar o comando real.

### 6. Verdade acima de suposicao

- Nao invente regras de negocio.
- Nao invente validacoes.
- Nao invente retorno de API.
- Nao invente tratamento de erro.
- Nao invente dependencias.
- Nao invente dados mockados irreais como se fossem verdade do sistema.
- Quando houver duvida, baseie-se somente no codigo existente e deixe claro o que foi confirmado e o que nao foi.

### 7. Mudancas minimas

- Faca a menor alteracao possivel para atender o pedido.
- Preserve ao maximo o comportamento atual.
- Nao altere nomes, assinaturas, contratos ou retorno sem necessidade explicita.
- Nao mova arquivos sem necessidade.
- Nao troque bibliotecas, frameworks ou padroes sem solicitacao explicita.
- Nunca cole comentarios no codigo.

### 8. Transparencia

- Antes de implementar, identifique rapidamente quais arquivos realmente serao alterados e por que.
- Ao final, informe exatamente o que foi modificado.
- Diferencie claramente o que foi confirmado no codigo existente, o que foi alterado e o que nao foi alterado por estar fora do escopo.

### 9. Git

- Em tarefas planejadas por etapas, cada etapa planejada deve terminar com um commit.
- Cada commit de etapa deve seguir exatamente o padrao: `feature: texto curto explicando a etapa`.
- Antes de cada commit de etapa, rode `dotnet build`.
- Rode `dotnet test` quando ja houver testes relevantes para a etapa.
- Corrija erros antes de commitar.
- Rode `git status` antes do commit.
- Nunca faca commit fora do padrao `feature: ...` quando a tarefa exigir execucao por etapas.
- Nunca faca commit automaticamente, exceto quando o usuario pedir explicitamente execucao com commits por etapa.
- Nunca faca commit sem autorizacao explicita do usuario, exceto quando o usuario pedir explicitamente execucao com commits por etapa.
- Nunca invente nome de commit fora do padrao autorizado.
- Quando o usuario pedir commit avulso, peca ou aguarde a mensagem exata do commit, salvo se ele autorizar escolher a mensagem.
- Nunca faca push automaticamente.
- Nunca faca push sem autorizacao explicita.
- Se terminar a implementacao, pare antes de commitar e antes de fazer push.

### 10. Sem atalhos perigosos

- Nao mascare erro.
- Nao comente codigo para resolver rapido.
- Nao remova teste quebrado sem autorizacao.
- Nao desabilite validacao, lint, tipagem ou regra de negocio para fazer passar.
- Nao use fallback improvisado sem confirmacao de que esse padrao ja existe no projeto.

### 11. Forma de resposta

- Seja direto e tecnico.
- Nao diga que simulou.
- Nao diga que assumiu algo sem deixar explicito.
- Se faltar contexto real no codigo, diga isso claramente.
- Sempre prefira precisao a velocidade.

## Fluxo Obrigatorio

1. Ler o pedido.
2. Encontrar no codigo a implementacao real relacionada.
3. Verificar o padrao ja existente.
4. Verificar se ja existem testes da rota, servico ou componente especifico.
5. Fazer somente a alteracao necessaria.
6. Ajustar ou complementar testes existentes, se aplicavel.
7. Executar testes reais quando aplicavel.
8. Mostrar resumidamente o que foi alterado.
9. Parar sem commit e sem push.

## Regra Final

Execute exatamente o solicitado, com alteracao minima, sem inventar nada, sem expandir escopo, reaproveitando o maximo do que ja existe no projeto e sem realizar commit ou push sem autorizacao explicita.
