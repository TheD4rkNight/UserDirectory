# User Directory — .NET 8 + React + SQLite + Microsoft Entra ID

A copy-paste-ready full-stack User Directory reference project using Clean Architecture.

## Stack

- Backend: .NET 8 Web API, C# 12, EF Core 8, SQLite
- Frontend: React 19, TypeScript, Vite
- Authentication: Microsoft Entra ID, OAuth 2.0 / OpenID Connect, MSAL Browser/React
- API security: Microsoft.Identity.Web, JWT bearer validation, `access_as_user` scope policy
- API docs: Swagger/OpenAPI
- Tests: xUnit + Moq + ASP.NET Core integration tests; Vitest + React Testing Library
- Containers: Docker Compose + Nginx reverse proxy

## Architecture

```text
React SPA
   |
   | MSAL / OAuth 2.0 Authorization Code + PKCE
   v
Microsoft Entra ID
   |
   | access token
   v
Nginx / .NET API
   |
   v
UserDirectory.Api
   |
   v
UserDirectory.Application
   |
   v
UserDirectory.Domain
   ^
   |
UserDirectory.Infrastructure
   |
   v
EF Core -> SQLite /data/app.db
```

The API has two public read endpoints and protected mutation endpoints:

| Endpoint | Auth |
|---|---|
| GET `/api/users` | Public |
| GET `/api/users/{id}` | Public |
| POST `/api/users` | `access_as_user` required |
| PUT `/api/users/{id}` | `access_as_user` required |
| DELETE `/api/users/{id}` | `access_as_user` required |

## Repository layout

```text
src/
  UserDirectory.Domain/
    Entities/User.cs
  UserDirectory.Application/
    Common/IUnitOfWork.cs
    Users/IUserRepository.cs
    Users/IUserService.cs
    Users/UserDtos.cs
    Users/UserService.cs
  UserDirectory.Infrastructure/
    Persistence/AppDbContext.cs
    Persistence/Migrations/*
    Repositories/UserRepository.cs
    DependencyInjection.cs
  UserDirectory.Api/
    Authorization/ScopeAuthorization.cs
    Controllers/UsersController.cs
    Middleware/GlobalExceptionHandler.cs
    Program.cs
    appsettings*.json

tests/
  UserDirectory.Application.Tests/
  UserDirectory.Api.Tests/

frontend/
  src/
    api/
    auth/
    components/
    pages/
    types/
    __tests__/
  Dockerfile
  package.json
  vite.config.ts

deploy/nginx/nginx.conf
docker-compose.yml
UserDirectory.sln
```

## Prerequisites

Install:

- .NET 8 SDK
- Node.js 22+
- npm
- Docker Desktop (optional)
- Microsoft Entra tenant if authentication is required

The repository uses .NET 8 through `global.json` with `latestFeature` roll-forward, so an installed .NET 8 SDK can normally satisfy it.

## 1. Configure Microsoft Entra ID

Create two App Registrations in the Microsoft Entra admin centre.

### API registration

Create an app named something like `UserDirectory.Api`.

Record:

- Directory (tenant) ID
- Application (client) ID

Under **Expose an API**:

1. Set the Application ID URI to the default `api://<API_CLIENT_ID>` or another URI you control.
2. Add a delegated scope named:
   - `access_as_user`
3. Allow the intended users/groups to consent as appropriate for your tenant.

The resulting scope should look like:

```text
api://<API_CLIENT_ID>/access_as_user
```

### React SPA registration

Create a second app named something like `UserDirectory.Web`.

Under **Authentication**:

1. Add a Single-page application platform.
2. Add redirect URI:

```text
http://localhost:5173
```

For Docker/Nginx local development also add:

```text
http://localhost:3000
```

Under **API permissions**:

1. Add permission.
2. My APIs.
3. Select `UserDirectory.Api`.
4. Select delegated permission `access_as_user`.
5. Grant admin consent if your tenant requires it.

For the API, configure the expected audience as:

```text
api://<API_CLIENT_ID>
```

### Environment variables

Copy:

```text
frontend/.env.example
```

to:

```text
frontend/.env
```

and replace the placeholders:

```text
VITE_ENTRA_TENANT_ID=<tenant-id>
VITE_ENTRA_CLIENT_ID=<react-spa-client-id>
VITE_API_CLIENT_ID=<api-client-id>
VITE_API_SCOPE=api://<api-client-id>/access_as_user
VITE_API_BASE_URL=/api
```

For a single-tenant corporate application, use your real tenant ID rather than `common`.

The backend values can be set in `appsettings.Development.json` or through environment variables:

```text
AzureAd__TenantId=<tenant-id>
AzureAd__ClientId=<api-client-id>
AzureAd__Audience=api://<api-client-id>
```

Do not commit real credentials, client secrets, certificates, or `.env` files.

## 2. Run locally

### Backend

From the repository root:

```bash
dotnet restore UserDirectory.sln
dotnet build UserDirectory.sln
dotnet dev-certs https --trust
dotnet run --project src/UserDirectory.Api --launch-profile https
```

API:

```text
https://localhost:7201
```

Swagger:

```text
https://localhost:7201/swagger
```

Health:

```text
https://localhost:7201/health
```

The development database is:

```text
src/UserDirectory.Api/data/app.db
```

The first startup applies the checked-in EF Core migration automatically.

### Frontend

In a second terminal:

```bash
cd frontend
npm install
npm run dev
```

Open:

```text
http://localhost:5173
```

Vite proxies `/api` to `https://localhost:7201` with certificate verification disabled for the development proxy.

## 3. EF Core migrations

The repository contains the initial migration under:

```text
src/UserDirectory.Infrastructure/Persistence/Migrations/
```

To create a future migration after changing the domain/database model:

```bash
dotnet ef migrations add YourMigrationName \
  --project src/UserDirectory.Infrastructure \
  --startup-project src/UserDirectory.Api \
  --output-dir Persistence/Migrations
```

Apply it manually with:

```bash
dotnet ef database update \
  --project src/UserDirectory.Infrastructure \
  --startup-project src/UserDirectory.Api
```

The API also calls `Database.MigrateAsync()` at startup so the local container can initialise/update the database automatically.

## 4. Tests

Backend unit + integration tests:

```bash
dotnet test UserDirectory.sln --collect:"XPlat Code Coverage"
```

Frontend tests:

```bash
cd frontend
npm test
```

Watch mode:

```bash
npm run test:watch
```

The API tests replace Microsoft Entra authentication with a deterministic test authentication handler. The integration database uses EF Core's in-memory provider, so tests never modify your real SQLite file.

## 5. Docker Compose

Set the API tenant/client values in your shell or `.env` next to `docker-compose.yml`:

```text
ENTRA_TENANT_ID=<tenant-id>
ENTRA_API_CLIENT_ID=<api-client-id>
```

Then:

```bash
docker compose up --build
```

Open:

```text
http://localhost:3000
```

Nginx serves the React SPA and proxies:

```text
/api/* -> http://api:8080/api/*
```

SQLite is persisted in the Docker volume:

```text
user-directory-data:/data
```

The database therefore lives at:

```text
/data/app.db
```
inside the API container.

Stop:

```bash
docker compose down
```

Stop and delete the database volume:

```bash
docker compose down -v
```

## 6. API examples

Public list:

```bash
curl https://localhost:7201/api/users
```

Authenticated create:

```bash
curl -X POST https://localhost:7201/api/users \
  -H "Authorization: Bearer <access-token>" \
  -H "Content-Type: application/json" \
  -d '{"name":"Jane Doe","age":40,"city":"Melbourne","state":"VIC","pincode":"3000"}'
```

Successful create returns HTTP 201 and a `Location` header for the new resource.

## Validation rules

Backend and frontend both validate:

- Name: required, 2–100 characters
- Age: required integer, 0–120
- City: required
- State: required
- Pincode: required, 4–10 characters

Backend validation is authoritative. Frontend validation exists for immediate user feedback and better UX.

## Security model

The API uses `Microsoft.Identity.Web` to validate Microsoft Entra bearer tokens. Mutation endpoints require the delegated `access_as_user` scope.

The React SPA uses MSAL Browser with the OAuth 2.0 Authorization Code Flow with PKCE. The token is acquired silently from the browser cache where possible and sent to the API as:

```http
Authorization: Bearer <access-token>
```

No client secret belongs in the React application.

## Production hardening checklist

Before production deployment, add/configure:

- HTTPS at the reverse proxy/load balancer
- real production allowed CORS origins
- secret/certificate management outside source control
- structured logging and central log aggregation
- distributed tracing / OpenTelemetry
- database backups
- database concurrency strategy if SQLite is replaced by a server database
- stricter Entra app roles/scopes if multiple API consumers exist
- rate limiting appropriate to traffic patterns
- security headers at Nginx/load balancer
- CI/CD pipeline that runs backend/frontend tests before deployment
- container image scanning
- dependency vulnerability scanning

## Design notes

### Why Clean Architecture?

The dependency direction is inward:

```text
API -> Application -> Domain
Infrastructure -> Application + Domain
```

The Application layer does not know about EF Core or SQLite. This lets the service be unit tested with mocks and makes the persistence technology replaceable.

### Why SQLite?

SQLite keeps the exercise self-contained and is suitable for small/local deployments. EF Core migrations are committed so schema changes are explicit and versioned.

For a high-concurrency production system, evaluate SQL Server, Azure SQL or PostgreSQL based on hosting and operational requirements.

### Why Microsoft.Identity.Web?

It removes a large amount of JWT validation boilerplate and provides Microsoft Entra-specific integration for ASP.NET Core APIs.

## Useful commands

```bash
# Build everything
dotnet build UserDirectory.sln

# Run backend
dotnet run --project src/UserDirectory.Api --launch-profile https

# Run backend tests
dotnet test UserDirectory.sln

# Run frontend
cd frontend && npm run dev

# Frontend tests
cd frontend && npm test

# Docker
docker compose up --build
```
