# Bank More

Projeto de estudo para operacoes bancarias com foco em:

- Conta corrente
- Transferencias entre contas
- Aplicacao de tarifas
- Integracao assincrona entre modulos

## Modulos principais

- Conta Corrente: abertura e movimentacao de conta.
- Transferencia: fluxo de transferencia entre contas.
- Tarifas: processamento de tarifas relacionadas aos eventos do sistema.
- SharedKernel: componentes compartilhados entre os modulos.

## Tecnologias

- .NET
- SQLite
- Docker e Docker Compose
- Kafka
- Swagger (OpenAPI)

## Como executar o projeto

A forma mais simples de executar tudo localmente e via Docker Compose.

### Passos

1. Abra um terminal na raiz do projeto.
2. Execute:

```bash
docker compose up --build
```

3. Aguarde a inicializacao dos servicos.

Para encerrar, use:

```bash
docker compose down
```

## Swagger das APIs

Com os servicos em execucao, acesse:

- Servico de Conta Corrente: http://localhost:5034/swagger/index.html
- Servico de Transferencias: http://localhost:5009/swagger/index.html

## Estrutura resumida

```text
src/
  ContaCorrente/
  Transferencia/
  Tarifas/
  SharedKernel/
```

## Testes

Para executar os testes automatizados:

```bash
dotnet test bank-more.sln
```

## Observacoes

- O ambiente local utiliza bancos SQLite por servico.
- A comunicacao assincrona entre modulos utiliza Kafka.
