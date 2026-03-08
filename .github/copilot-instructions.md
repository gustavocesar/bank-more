# Instruções para o GitHub Copilot

Este projeto foi desenvolvido seguindo princípios de **DDD (Domain Driven Design)**, **CQRS**, **Clean Architecture** e **Monolito Modular**, visando organização, baixo acoplamento e facilidade de evolução para microsserviços no futuro.

O Copilot deve respeitar os padrões arquiteturais descritos abaixo ao sugerir código.

---

# Arquitetura do Projeto

A solução está organizada como um **Monolito Modular**, onde cada módulo representa um **Bounded Context** independente.

Módulos principais:

* ContaCorrente
* Transferencia
* Tarifas
* SharedKernel

Cada módulo possui sua própria estrutura interna seguindo **Clean Architecture**:

```
Domain
Application
Infrastructure
API
Tests
```

Dependências permitidas:

```
API → Application → Domain
Infrastructure → Domain
Application → SharedKernel
Domain → SharedKernel
Infrastructure → SharedKernel
API → SharedKernel
```

A camada **Domain nunca deve depender de outras camadas** (com exceção do SharedKernel, para casos pontuais).

O **SharedKernel** pode ser referenciado por múltiplos módulos, desde que contenha apenas componentes genéricos e reutilizáveis.

---

# Shared Kernel

O projeto utiliza o conceito de **Shared Kernel** conforme definido em **Domain Driven Design**.

O **SharedKernel** contém apenas componentes de domínio genéricos reutilizáveis, que podem ser utilizados por múltiplos módulos (**Bounded Contexts**) sem criar acoplamento indevido.

O **SharedKernel** não deve conter lógica de negócio específica de nenhum módulo.

Exemplos apropriados para o **SharedKernel**:

* abstrações comuns
* value objects genéricos
* exceptions base
* contratos compartilhados estritamente necessários

Evitar incluir no **SharedKernel**:

* regras de negócio de ContaCorrente
* regras de negócio de Transferencia
* regras de negócio de Tarifas
* qualquer comportamento específico de um único módulo

---

# Padrões de Desenvolvimento

## DDD

A lógica de negócio deve ficar dentro do **Domain**.

Utilizar:

* Entities
* Interfaces de Repositórios
* ValueObjects

Evitar lógica de negócio em Controllers ou Repositories.

---

## CQRS

Separar operações de **Command** e **Query**.

Estrutura esperada:

```
Application
 ├ Commands
 │   ├ CriarConta
 │   ├ MovimentarConta
 │   └ InativarConta
 │
 ├ Queries
 │   └ ConsultarSaldo
```

Commands devem alterar estado.

Queries devem apenas retornar dados.

Handlers podem ficar na mesma pasta dos Commands/Queries.

---

## MediatR

Todas as operações devem ser executadas via **MediatR**.

Exemplo esperado:

Command:

```
internal sealed record MovimentarContaCommand(
    Guid AccountId,
    decimal Valor,
    TipoMovimento Tipo
) : IRequest;
```

Handler:

```
internal sealed class MovimentarContaHandler
    : IRequestHandler<MovimentarContaCommand>
```

Controllers devem apenas encaminhar requests para o MediatR.

---

# Regras de Visibilidade

Por padrão, utilizar **internal** para classes.

Somente devem ser **public**:

* Controllers
* DTOs expostos na API

Exemplo:

```
internal class ContaCorrente
internal class MovimentarContaHandler
```

---

# Persistência

A persistência deve utilizar **Dapper**.

Evitar ORMs como Entity Framework.

Cada módulo possui seu próprio repositório.

Exemplo esperado:

```
internal interface IContaRepository
internal class ContaRepository : IContaRepository
```

Banco de dados utilizado:

SQLite

---

# Idempotência

Operações críticas devem ser idempotentes.

Exemplos:

* movimentações
* transferências

Utilizar tabela de idempotência contendo:

```
chave_idempotencia
requisicao
resultado
```

---

# Autenticação

Todas as APIs devem utilizar **JWT**.

O token deve conter:

* identificação da conta corrente

Nenhum endpoint deve permitir acesso sem autenticação.

---

# Comunicação entre módulos

Sempre que possível, preferir **eventos Kafka** para comunicação assíncrona.

Biblioteca utilizada:

KafkaFlow

Eventos esperados:

```
TransferenciaRealizadaEvent
TarifaAplicadaEvent
```

---

# Testes

Cada módulo deve possuir testes automatizados.

Tipos de teste:

* Unitários
* Integração

Framework recomendado:

xUnit

---

# Boas práticas

Sempre que possível:

* utilizar async/await
* evitar métodos síncronos
* utilizar CancellationToken
* utilizar injeção de dependência

---

# Estrutura de código esperada

Exemplo de organização:

```
ContaCorrente

Domain
 ├ Entities
 ├ ValueObjects
 └ Enums

Application
 ├ Commands
 └ Queries

Infrastructure
 ├ Persistence
 └ Messaging

API
 ├ Endpoints
 ├ Contracts
 ├ Endpoints
 └ Middlewares
```

---

# Eventos de domínio

Sempre que uma ação relevante ocorrer, considerar gerar um evento.

Exemplo:

```
TransferenciaRealizada
TarifaAplicada
```

Eventos devem ser publicados via Kafka.

---

# Objetivo do projeto

Demonstrar:

* arquitetura modular
* boas práticas de DDD
* uso de CQRS
* comunicação assíncrona via eventos
* APIs seguras
* código organizado e testável

```
