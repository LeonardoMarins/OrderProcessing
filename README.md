# OrderProcessing

Sistema de processamento de pedidos construído com arquitetura orientada a eventos, utilizando .NET 10, RabbitMQ, SQL Server e MongoDB.

## Arquitetura

```
┌─────────────┐        ┌───────────┐        ┌────────────┐
│   Cliente   │ ──────▶│    API    │───────▶│  RabbitMQ  │
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

- **API**: recebe requisições HTTP, publica pedidos na fila e consulta o cache MongoDB
- **Worker**: consome a fila RabbitMQ e persiste os pedidos no SQL Server
- **MongoDB**: cache de pedidos
- **RabbitMQ**: mensageria entre API e Worker
- **SQL Server**: persistência principal

## Tecnologias

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- MediatR (CQRS)
- RabbitMQ
- MongoDB
- Docker / Docker Compose
- Azure Container Apps
- Azure DevOps (CI/CD)

## Pré-requisitos

- [Docker](https://www.docker.com/products/docker-desktop) instalado
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (opcional, para rodar sem Docker)

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

Aguarde todos os containers iniciarem. Na primeira vez pode demorar alguns minutos para o build.

As migrations são aplicadas automaticamente quando a API inicia.

### 3. Acesse a API

```
http://localhost:5000/orders
```

## Como rodar sem Docker

### 1. Suba apenas os serviços de infraestrutura

```bash
docker-compose up sqlserver rabbitmq mongodb
```

### 2. Configure o appsettings

Copie o arquivo de exemplo e preencha as configurações:

```bash
cp OrderProcessing.API/appsettings.Example.json OrderProcessing.API/appsettings.json
```

### 3. Rode a API

```bash
dotnet run --project OrderProcessing.API
```

### 5. Rode o Worker

Em outro terminal:

```bash
dotnet run --project OrderProcessing.Worker
```

## Endpoints

### Criar pedido
```
POST /orders
Content-Type: application/json

{
  "client": "Nome do Cliente",
  "value": 150.00,
  "orderDate": "2024-04-12T00:00:00"
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

## CI/CD

O projeto utiliza Azure DevOps com pipeline configurado em `azure-pipelines.yml`.

A cada push na branch `main`:
1. Build da imagem Docker da API
2. Build da imagem Docker do Worker
3. Push das imagens para o Docker Hub

## Deploy

A aplicação está hospedada no Azure Container Apps:

- **API**: `https://order-api.wittyhill-1ef4ba1e.eastus2.azurecontainerapps.io`

## Serviços em produção

| Serviço | Plataforma |
|---------|-----------|
| API / Worker | Azure Container Apps |
| SQL Server | Azure SQL Database |
| RabbitMQ | CloudAMQP |
| MongoDB | MongoDB Atlas |

## Monitoramento RabbitMQ (local)

Acesse o painel de gerenciamento do RabbitMQ em:
```
http://localhost:15672
```
Usuário: `guest` / Senha: `guest`
