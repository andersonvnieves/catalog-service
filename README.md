# Catalog Service

Serviço de catálogo. Gerencia jogos, a biblioteca do usuário e a criação de pedidos. Persiste dados no SQL Server, publica `order.created` no RabbitMQ e consome `payment.processed` para atualizar os pedidos.

## Componentes

| Projeto | Responsabilidade |
| --- | --- |
| `br.com.fiap.cloudgames.Catalog.Domain` | Regras de negócio, agregados e contratos. |
| `br.com.fiap.cloudgames.Catalog.Application` | Casos de uso e contratos de integração. |
| `br.com.fiap.cloudgames.Catalog.Infrastructure` | SQL Server/EF Core, JWT e RabbitMQ. |
| `br.com.fiap.cloudgames.Catalog.WebAPI` | API REST, Swagger e worker consumidor de eventos. |

## Pré-requisitos

- .NET SDK 10;
- SQL Server na porta `1433`;
- RabbitMQ na porta `5672` (painel: `15672`).

Para iniciar toda a plataforma — incluindo essas dependências — use o [README da orquestração](https://github.com/andersonvnieves/orchestration/blob/main/README.md). Para executar apenas este serviço, deixe SQL Server e RabbitMQ disponíveis localmente.

## Configuração local

O perfil `Development` usa `br.com.fiap.cloudgames.Catalog.WebAPI/appsettings.Development.json`. Ele contém a configuração de desenvolvimento já usada pelo repositório. Para sobrescrever valores sem editar o arquivo, use variáveis de ambiente do ASP.NET Core:

```powershell
$env:ConnectionStrings__Default = "Server=localhost,1433;Database=FGC_Catalog;User Id=sa;Password=<SENHA>;TrustServerCertificate=True;"
$env:Jwt__Issuer = "fgcapi"
$env:Jwt__Audience = "fgcapi"
$env:Jwt__Key = "<CHAVE_JWT>"
$env:RabbitMQ__URI = "amqp://<USUARIO>:<SENHA>@localhost:5672/"
$env:RabbitMQ__OrderCreatedEvent__Exchange = "fgc"
$env:RabbitMQ__OrderCreatedEvent__RoutingKey = "order.created"
$env:RabbitMQ__OrderCreatedEvent__QueueName = "order.created"
$env:RabbitMQ__PaymentProcessedEvent__Exchange = "fgc"
$env:RabbitMQ__PaymentProcessedEvent__RoutingKey = "payment.processed"
$env:RabbitMQ__PaymentProcessedEvent__QueueName = "catalog.payment.processed"
```

## Executar localmente

Na pasta `catalog-service`:

```powershell
dotnet restore .\br.com.fiap.cloudgames.CatalogAPI.sln
dotnet run --project .\br.com.fiap.cloudgames.Catalog.WebAPI\br.com.fiap.cloudgames.Catalog.WebAPI.csproj --launch-profile http
```

A API fica disponível em `http://localhost:5150` e o Swagger em `http://localhost:5150/swagger`. As migrations são aplicadas automaticamente na inicialização.

## Endpoints principais

| Método | Rota | Acesso |
| --- | --- | --- |
| `GET` | `/api/Game?Id={id}` | Público |
| `POST` / `PUT` | `/api/Game` | JWT com role `admin` |
| `GET` | `/api/Library` | JWT válido |
| `POST` | `/api/Order` | JWT válido |

Use o token emitido pelo User Service no cabeçalho `Authorization: Bearer <token>`.

## Testes

```powershell
dotnet test .\br.com.fiap.cloudgames.CatalogAPI.sln
```

## Contêiner

```powershell
docker build -t fgc-catalog-service:latest .
docker run --rm -p 8081:8080 fgc-catalog-service:latest
```

Em contêiner, informe as mesmas configurações da seção anterior como variáveis de ambiente e use os hosts da rede Docker para SQL Server e RabbitMQ.
