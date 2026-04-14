# OrderProcessing

Sistema de processamento de pedidos construído com arquitetura orientada a eventos, utilizando .NET 10, RabbitMQ, SQL Server e MongoDB.

## Arquitetura

```
┌─────────────┐        ┌───────────┐        ┌────────────┐
│   Frontend  │ ──────▶│    API    │───────▶│  RabbitMQ  │
└─────────────┘        └───────────┘        └─────┬──────┘
                             │                     │
                             ▼                     ▼
                       ┌──────────┐         ┌────────────┐
                       │ MongoDB  │         │   Worker   │
                       │ (Cache)  │         └─────┬──────┘
                       └──────────┘               │
                                                   ▼
                                            ┌────────────┐
                                            │ SQL Server │
                                            └────────────┘
```

- **Frontend**: interface web em React/Vite para criação e consulta de pedidos
- **API**: recebe requisições HTTP, publica pedidos na fila e consulta o cache MongoDB
- **Worker**: consome a fila RabbitMQ e persiste os pedidos no SQL Server
- **MongoDB**: cache de pedidos para consultas rápidas
- **RabbitMQ**: mensageria assíncrona entre API e Worker
- **SQL Server**: persistência principal dos pedidos

## Tecnologias

- .NET 10 — ASP.NET Core Web API + Worker Service
- Entity Framework Core — ORM com migrations automáticas
- MediatR — CQRS (Commands e Queries)
- ErrorOr — resultado tipado sem exceções de fluxo
- Serilog — structured logging com output estruturado no console
- OpenTelemetry — tracing e métricas (console em dev, Azure Monitor em produção)
- RabbitMQ — mensageria assíncrona
- MongoDB — cache
- Docker / Docker Compose — ambiente local
- Azure Container Apps — deploy da API e Worker
- Azure Static Web Apps — deploy do frontend
- Azure DevOps — CI/CD pipeline
- xUnit + NSubstitute + FluentAssertions — testes unitários
- TestContainers — testes de integração

## Pré-requisitos

- [Docker](https://www.docker.com/products/docker-desktop) instalado

## Como rodar com Docker (recomendado)

### 1. Clone o repositório

```bash
git clone <url-do-repositorio>
cd OrderProcessing
```

### 2. Suba os serviços

```bash
docker-compose up --build
```

Aguarde todos os containers iniciarem. Na primeira vez pode demorar alguns minutos para o build das imagens.

As migrations são aplicadas automaticamente quando a API inicia.

### 3. Acesse a aplicação

| Serviço | URL |
|---------|-----|
| Frontend | http://localhost:3000 |
| API | http://localhost:5000 |
| RabbitMQ Management | http://localhost:15672 (guest/guest) |

## Como rodar sem Docker

### 1. Suba apenas a infraestrutura

```bash
docker-compose up sqlserver rabbitmq mongodb
```

### 2. Rode a API

```bash
dotnet run --project OrderProcessing.API
```

### 3. Rode o Worker

Em outro terminal:

```bash
dotnet run --project OrderProcessing.Worker
```

### 4. Rode o Frontend

Em outro terminal:

```bash
cd OrderProcessing-front
npm install
npm run dev
```

## Endpoints

### Criar pedido
```
POST /orders
Content-Type: application/json

{
  "client": "Nome do Cliente",
  "value": 150.00
}
```

### Listar pedidos
```
GET /orders
```

### Buscar pedido por ID
```
GET /orders/{id}
```

## Observabilidade

O projeto usa **Serilog** para structured logging e **OpenTelemetry** para tracing e métricas.

| Ambiente | Destino |
|----------|---------|
| Development | Console (stdout) |
| Produção | Azure Application Insights |

Em produção, os logs e traces ficam disponíveis no Azure Portal em **Application Insights → Logs** e **Transaction search**.

## Testes

### Unitários

```bash
dotnet test OrderProcessing.UnitTests
```

Cobrem os handlers de Commands e Queries com mocks via NSubstitute.

### Integração

```bash
dotnet test OrderProcessing.IntegrationTests
```

Sobem containers reais (SQL Server, MongoDB, RabbitMQ) via TestContainers e testam os endpoints HTTP end-to-end.

## CI/CD

O projeto utiliza Azure DevOps com pipeline configurado em `azure-pipelines.yml`.

A cada push na branch `main` o pipeline executa:

1. **Build** da solução .NET
2. **Testes** unitários e de integração
3. **Build & Push** das imagens Docker (API e Worker) para o Docker Hub
4. **Deploy** da API e Worker no Azure Container Apps
5. **Build & Deploy** do frontend no Azure Static Web Apps

## Serviços em produção

| Serviço | Plataforma |
|---------|-----------|
| API / Worker | Azure Container Apps |
| Frontend | Azure Static Web Apps |
| SQL Server | Azure SQL Database |
| RabbitMQ | CloudAMQP |
| MongoDB | MongoDB Atlas |
| Logs | Serilog → Azure Application Insights |
| Tracing / Métricas | OpenTelemetry → Azure Application Insights |
