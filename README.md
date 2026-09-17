# MtgBans.Api

The public API behind [MTGBans.info](https://mtgbans.info) — a site that tracks Magic: The
Gathering ban/restriction announcements across formats. It's the only writer to the Postgres
schema it owns, and the only backend [mtg-bans-ui](../mtg-bans-ui) talks to.

## Requirements

- .NET 8 SDK
- A Postgres database (see [mtg-bans-database](../mtg-bans-database) for schema dumps)

## Getting started

```
dotnet build MtgBans.sln
```

Copy a connection string into `src/MtgBans.Api/appsettings.Development.json` (gitignored):

```json
{
  "ConnectionStrings": {
    "AppDb": "User ID=postgres;Password=...;Host=127.0.0.1;Port=5432;Database=mtg-bans"
  }
}
```

Then run the API and apply any pending migrations:

```
dotnet ef database update --project src/MtgBans.Data --startup-project src/MtgBans.Api
dotnet run --project src/MtgBans.Api
```

The API listens on `http://localhost:5082` by default. In development, Swagger UI is available at
`/swagger`.

```
dotnet test
```

## Configuration

Settings come from `appsettings.json` plus an environment-specific override
(`appsettings.Development.json` / `appsettings.Production.json`, both gitignored) and, in
production, environment variables.

| Key | Purpose |
|---|---|
| `ConnectionStrings:AppDb` | Postgres connection string |
| `Scryfall:ApiBaseUrl` | Base URL for the Scryfall client |
| `Security:ApiKey` | Shared secret checked against the `X-Api-Key` header on protected endpoints |

## Architecture

```
MtgBans.Api (controllers, DI wiring, auth filter)
  -> MtgBans.Services (business logic; one service+interface per domain: Card, Format, Announcement, Publication)
       -> MtgBans.Data (EF Core DbContext + entities, Npgsql, snake_case naming convention)
       -> MtgBans.Scryfall (typed client for the external Scryfall API — card/set lookups)
  -> MtgBans.Models (DTOs shared between Api and Services)
MtgBans.Exceptions (custom exception types)
```

- **Controllers** stay thin — they delegate directly to a service interface injected via
  constructor. Their XML doc comments are wired into Swagger and double as the real API
  documentation.
- **Services** hold all business logic, registered in
  `MtgBans.Services.Extensions.MtgBansServiceExtensions.AddMtgBans`.
- **Data** uses EF Core with `UseSnakeCaseNamingConvention()`, so PascalCase C# properties map to
  snake_case Postgres columns. A query that `.Include()`s two or more collection navigations off
  the same entity needs `.AsSplitQuery()` too, or it throws at runtime.
- Auth is a single `ApiKeyAuthenticationFilter` (`src/MtgBans.Api/Filters`), applied per-action via
  `[ApiKeyAuthentication]` and checked against `Security:ApiKey`. Public GET endpoints for the site
  stay unauthenticated; mutating/administrative endpoints get the attribute.

### Domain model

`Card` <-> `Printing` <-> `Expansion`, with `CardAlias` for alternate names. `Format` and
`FormatEvent` track supported formats, `CardLegalityEvent`/`CardLegalityStatus` track ban and
restriction history, and `CardLegalityRationale` (plus `CardLegalityRationaleVote`) hold the
AI-generated "why was this banned" writeups produced by
[mtg-bans-util](../mtg-bans-util)'s `summarize` command.

Routes are grouped by domain (`/cards`, `/formats`, `/announcements`, `/publications`), plus
`/health`. Full request/response shapes are in Swagger (`/swagger` in development) — routes there
come from each controller's XML doc comments, which are the source of truth as endpoints change.

## Database migrations

Migrations live in `src/MtgBans.Data/Migrations`, generated from `MtgBansContext.cs` and
`src/MtgBans.Data/Entities/*`:

```
dotnet ef migrations add <Name> --project src/MtgBans.Data --startup-project src/MtgBans.Api
dotnet ef database update --project src/MtgBans.Data --startup-project src/MtgBans.Api
```

This API owns the schema — it's the only component that should apply migrations.

## Docker

```
docker build -t mtg-bans-api .
```

Multi-stage build targeting `net8.0`; the container listens on port 4000 internally.
`.github/workflows/ecs_deploy.yaml` deploys to ECS on push to the tracked branch.
