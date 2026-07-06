# CatalogoApi

> Projeto de estudo e desenvolvimento de habilidades em **.NET**, explorando padrões e recursos do ecossistema ASP.NET Core.

API REST em **.NET 9** para gerenciamento de um catálogo de produtos e categorias, com autenticação via **JWT (Access Token + Refresh Token)**, controle de acesso por *roles*, paginação, cache em memória e documentação via Swagger.

## Sumário

- [Visão geral](#visão-geral)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Pré-requisitos](#pré-requisitos)
- [Como rodar](#como-rodar)
- [Autenticação](#autenticação)
- [Autorização (Policies)](#autorização-policies)
- [Endpoints](#endpoints)
  - [Auth](#auth)
  - [Categorias](#categorias)
  - [Produtos](#produtos)
- [Paginação e filtros](#paginação-e-filtros)
- [Cache](#cache)
- [CORS](#cors)
- [Rate Limiting](#rate-limiting)
- [Logging e tratamento de exceções](#logging-e-tratamento-de-exceções)


## Visão geral

O CatalogoApi expõe operações de CRUD para **Categorias** e **Produtos**, com relacionamento 1:N entre eles. O acesso à API é protegido por autenticação JWT, com suporte a renovação de token via refresh token e controle de permissões baseado em roles (ex: `UserOnly`, `AdminOnly`, `SuperAdminOnly`, `ExclusiveOnly`).

## Tecnologias

- **.NET 9** (`Microsoft.NET.Sdk.Web`)
- **Entity Framework Core 9** + **Pomelo.EntityFrameworkCore.MySql** (MySQL)
- **ASP.NET Core Identity** (`Microsoft.AspNetCore.Identity.EntityFrameworkCore`)
- **JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- **X.PagedList** para paginação
- **Swashbuckle.AspNetCore** (Swagger/OpenAPI)
- **JsonPatch** para atualizações parciais (`PATCH`)
- **IMemoryCache** para cache em memória
- **CORS** configurado por política nomeada (`OrigensComAcessoPermitido`)
- **Rate Limiting** (`Microsoft.AspNetCore.RateLimiting`)
- **Logger customizado** (`CustomLoggerProvider`) e filtro global de exceções (`ApiExceptionsFilter`)

## Arquitetura

```
CatalogoApi/
├── Controllers/         # AuthController, CategoriaController, ProdutosController
├── Models/              # Categoria, Produto, ApplicationUser, ErrorDetails
├── DTOs/                # DTOs e mapeamentos (extension methods)
├── Repositories/        # Repository<T> genérico + implementações específicas
│   └── Interface/       # IRepository, ICategoriaRepository, IProdutoRepository
├── Pagination/          # Parâmetros de paginação e filtros
├── Filters/             # Filtros customizados (ex: ApiLogginFilter)
├── Context/             # AppDbContext (EF Core)
└── appsettings.json     # Configurações (connection string, JWT, etc.)
```

O padrão utilizado é **Repository Pattern** com um repositório genérico (`Repository<T>`) que implementa operações básicas (`GetAllAsync`, `GetAsync`, `CreateAsync`, `UpdateAsync`, `DeleteAsync`), estendido por repositórios específicos (`CategoriaRepository`, `ProdutoRepository`) que adicionam consultas com paginação e filtros.

> **Nota:** os repositórios atualmente simulam uma latência de 3 segundos (`Thread.Sleep(3000)`) em `GetAllAsync`/`GetAsync` —  para fins de teste/didáticos. Vale remover isso antes de qualquer uso em produção.

## Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- MySQL Server (local ou remoto)
- Uma ferramenta de client de API (Postman, Insomnia, ou o próprio Swagger)



## Como rodar

```bash
# Restaurar pacotes
dotnet restore

# Aplicar migrations (se houver)
dotnet ef database update

# Rodar a aplicação
dotnet run
```

A API estará disponível em `http://localhost:5066` (conforme `ValidIssuer` configurado). O **Swagger** (`/swagger`) e o middleware de tratamento de exceções só são habilitados quando `ASPNETCORE_ENVIRONMENT=Development`.

> Se a `JWT:SecretKey` não estiver configurada, a aplicação falha ao iniciar com `ArgumentException("Invalid secret key!!")` — isso é intencional (fail-fast), garanta que ela esteja definida via User Secrets ou variável de ambiente antes de rodar.

## Autenticação

Fluxo de autenticação via JWT:

1. **`POST /api/Auth/register`** — cria um novo usuário.
2. **`POST /api/Auth/login`** — autentica e retorna `Token`, `RefreshToken` e `Expiration`.
3. Envie o token no header: `Authorization: Bearer {token}`.
4. Quando o access token expirar, use **`POST /api/Auth/refresh-token`** com o token expirado e o refresh token para obter um novo par.

Endpoints administrativos adicionais:

- **`POST /api/Auth/revoke/{username}`** — revoga o refresh token de um usuário (`ExclusiveOnly`).
- **`POST /api/Auth/CreateRole`** — cria uma nova role (`SuperAdminOnly`).
- **`POST /api/Auth/AddUserToRole`** — associa um usuário a uma role (`SuperAdminOnly`).

## Autorização (Policies)

A API define policies de autorização customizadas, combinando roles e claims:

| Policy | Requisito |
|---|---|
| `AdminOnly` | Usuário na role `Admin` |
| `SuperAdminOnly` | Usuário na role `Admin` e um claim específico adicional |
| `UserOnly` | Usuário na role `User` |
| `ExclusiveOnly` | Usuário com um claim específico ou em uma role de destaque |

Essas policies ilustram diferentes formas de compor autorização no ASP.NET Core: por role simples (`RequireRole`), por combinação de role + claim (`RequireRole` + `RequireClaim`), e por lógica customizada (`RequireAssertion`).

## Endpoints

### Auth

| Método | Rota | Descrição | Autorização |
|---|---|---|---|
| POST | `/api/Auth/register` | Cria um novo usuário | Público |
| POST | `/api/Auth/login` | Login e emissão de tokens | Público |
| POST | `/api/Auth/refresh-token` | Renova o access token | Público (requer refresh token válido) |
| POST | `/api/Auth/revoke/{username}` | Revoga refresh token | `ExclusiveOnly` |
| POST | `/api/Auth/CreateRole` | Cria uma role | `SuperAdminOnly` |
| POST | `/api/Auth/AddUserToRole` | Adiciona usuário a uma role | `SuperAdminOnly` |

### Categorias

| Método | Rota | Descrição |
|---|---|---|
| GET | `/Categoria` | Lista todas as categorias (com cache) |
| GET | `/Categoria/{id}` | Obtém categoria por Id (com cache) |
| POST | `/Categoria` | Cria uma nova categoria |
| PUT | `/Categoria/id{id}` | Atualiza uma categoria |
| DELETE | `/Categoria/id{id}` | Remove uma categoria |
| GET | `/Categoria/pagination` | Lista categorias com paginação |
| GET | `/Categoria/filter/nome/pagination` | Filtra categorias por nome, com paginação |

Exemplo de payload para criação:

```json
POST /Categoria
{
  "categoriaId": 1,
  "nome": "Bebidas",
  "imagemUrl": "https://exemplo.com/imagem/bebidas.png"
}
```

### Produtos

| Método | Rota | Descrição |
|---|---|---|
| GET | `/api/Produtos` | Lista todos os produtos (`UserOnly`) |
| GET | `/api/Produtos/{id}` | Obtém produto por Id |
| GET | `/api/Produtos/categoria/{categoriaId}` | Lista produtos de uma categoria |
| GET | `/api/Produtos/pagination` | Lista produtos com paginação |
| GET | `/api/Produtos/filter/preco/pagination` | Filtra produtos por preço (`maior`, `menor`, `igual`), com paginação |
| POST | `/api/Produtos` | Cria um novo produto |
| PUT | `/api/Produtos/id{id}` | Atualiza um produto |
| PATCH | `/api/Produtos/{id}/UpdatePartial` | Atualização parcial via JSON Patch |
| DELETE | `/api/Produtos/{id}` | Remove um produto |

## Paginação e filtros

Os endpoints de paginação usam `X.PagedList` e retornam metadados no header `X-Pagination`:

```json
{
  "Count": 10,
  "PageSize": 10,
  "PageCount": 3,
  "TotalItemCount": 25,
  "HasNextPage": true,
  "HasPreviousPage": false
}
```

O filtro de preço de produtos aceita os critérios: `maior`, `menor` e `igual`.

## Cache

O `CategoriaController` usa `IMemoryCache` para reduzir consultas ao banco:

- Cache da lista completa de categorias (`CacheCategorias`).
- Cache individual por categoria (`CacheCategoria_{id}`).
- Expiração absoluta de 30s e deslizante de 15s.
- Invalidado automaticamente em operações de criação, atualização e remoção.

## CORS

A política `OrigensComAcessoPermitido` permite requisições apenas de:

- **Origem:** `https://localhost:7022`
- **Métodos:** `GET`, `POST`
- **Headers:** qualquer um

Isso significa que, hoje, requisições `PUT`, `PATCH` e `DELETE` feitas a partir do front-end configurado como origem permitida seriam bloqueadas pelo CORS. É um bom exemplo prático de como o CORS é restritivo por padrão — ajuste `WithMethods` conforme os verbos usados pelo front-end.

## Rate Limiting

Há **duas configurações de rate limiting** registradas (`AddRateLimiter` é chamado duas vezes), e ambas ficam ativas simultaneamente:

1. **Named policy `"Fixedwindow"`**: 1 requisição a cada 5 segundos, fila de até 2 requisições (atualmente comentada/desabilitada nos controllers via `//[EnableRateLimiting("Fixedwindow")]` e `[DisableRateLimiting]`).
2. **Global limiter**: 2 requisições a cada 10 segundos, particionado por usuário autenticado (ou host, se anônimo), sem fila (`QueueLimit = 0`).

Requisições que excedem o limite recebem **HTTP 429 (Too Many Requests)**.

Esse projeto explora duas abordagens de rate limiting lado a lado: policy nomeada por endpoint vs. limiter global por partição — bom material de estudo para entender as diferenças entre `[EnableRateLimiting]`, `[DisableRateLimiting]` e `GlobalLimiter`, já que este último continua valendo mesmo quando uma policy nomeada é desabilitada em um endpoint específico.

## Logging e tratamento de exceções

- Um **logger customizado** (`CustomLoggerProvider`) é registrado com `LogLevel.Information`, além dos providers padrão do ASP.NET Core.
- Um **filtro global de exceções** (`ApiExceptionsFilter`) é adicionado a todos os controllers via `options.Filters.Add`.
- Em ambiente de **Development**, um middleware adicional de tratamento de exceções é configurado (`app.ConfigureExceptionHandler()`), retornando respostas de erro estruturadas (ver `Models/ErrorDetails`).

