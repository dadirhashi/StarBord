# StarBord – Backend (API)

StarBord samlar ett företags recensioner från olika plattformar (Trustpilot, Google) på ett ställe och låter ägaren läsa och svara på dem. Det här repot innehåller REST-API:t. Frontend finns i [StarBord-Client](https://github.com/dadirhashi/StarBord-Client).

## Teknik

- **.NET 9** / ASP.NET Core Web API
- **Entity Framework Core 9** med **SQL Server**
- **MediatR** (CQRS – commands och queries under `Application/`)
- **JWT Bearer**-autentisering, lösenord hashas med **BCrypt**
- **Swagger / OpenAPI** för dokumentation och test av endpoints

## Projektstruktur

```
Application/     Commands & queries (MediatR) per domän: Businesses, Reviews, Responses, Users
Controllers/     API-endpoints
Data/            StarBordDbContext
DTOS/            Request/response-objekt
Integrations/    DTO:er för externa API:er (Trustpilot)
Middleware/      GlobalExceptionMiddleware – gemensam felhantering
Migrations/      EF Core-migrationer
Models/          Entiteter: User, Business, Review, Response, PlatformToken
Services/        Auth, PlatformToken, Trustpilot, (mock) Google Reviews
```

### Datamodell

```
User 1──* Business 1──* Review 1──0..1 Response
              │
              └──* PlatformToken   (OAuth-tokens per plattform, t.ex. Trustpilot)
```

## Kom igång

### Förutsättningar

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (t.ex. SQL Server Express eller LocalDB)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### 1. Klona och återställ paket

```bash
git clone https://github.com/dadirhashi/StarBord.git
cd StarBord
dotnet restore
```

### 2. Konfiguration

`appsettings.json` innehåller ingen hemlig data – JWT-nyckel och Trustpilot-uppgifter läggs i **User Secrets** (projektet har redan ett `UserSecretsId`):

```bash
# Databas (ändra till din egen SQL Server-instans)
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost\\SQLEXPRESS;Database=StarBoardDb;Trusted_Connection=True;TrustServerCertificate=True;"

# JWT – minst 32 tecken
dotnet user-secrets set "Jwt:Key" "<en-lång-slumpmässig-hemlig-nyckel>"

# Trustpilot – mot den inbyggda mocken vid lokal utveckling
dotnet user-secrets set "Trustpilot:BaseUrl"      "http://localhost:5050/api/mock/trustpilot"
dotnet user-secrets set "Trustpilot:ClientId"     "mock-client-id"
dotnet user-secrets set "Trustpilot:ClientSecret" "mock-client-secret"
dotnet user-secrets set "Trustpilot:RedirectUri"  "http://localhost:5050/api/trustpilot/callback"
```

> Trustpilot-värdena krävs – `TrustpilotService` kastar ett fel om någon av dem saknas.

Övriga inställningar i `appsettings.json`:

| Nyckel | Standard | Beskrivning |
|---|---|---|
| `Jwt:Issuer` / `Jwt:Audience` | `StarBord` / `StarBordUsers` | Värden som valideras i token |
| `Jwt:ExpiresInMinutes` | `60` | Tokenens livslängd |
| `Frontend:BaseUrl` | `http://localhost:5173` | Dit användaren skickas efter Trustpilot-OAuth |

### 3. Skapa databasen

```bash
dotnet ef database update
```

### 4. Starta API:t

```bash
dotnet run --launch-profile http     # http://localhost:5050
# eller
dotnet run --launch-profile https    # https://localhost:7239
```

Swagger UI finns på `/swagger` i Development-läge. Klicka på **Authorize** och ange `Bearer <token>` för att testa skyddade endpoints.

CORS tillåter frontend på `http://localhost:5173`.

## API-översikt

Alla endpoints utom de markerade kräver JWT (`Authorization: Bearer <token>`).

| Område | Metod & route | Beskrivning |
|---|---|---|
| **Auth** | `POST /api/auth/login` *(öppen)* | Logga in, returnerar JWT |
| **Users** | `POST /api/user` *(öppen)* | Registrera användare |
| | `GET /api/user` · `GET /api/user/{email}` | Lista / hämta användare (Admin) |
| **Businesses** | `GET /api/businesses` · `GET /api/businesses/{id}` | Lista / hämta företag |
| | `POST` · `PUT /{id}` · `DELETE /{id}` | Skapa / uppdatera / ta bort |
| **Reviews** | `GET /api/reviews/business/{businessId}` | Recensioner för ett företag |
| | `GET` · `PUT` · `DELETE /api/reviews/{id}` · `POST /api/reviews` | CRUD |
| **Responses** | `POST /api/responses` | Svara på en recension |
| | `GET /api/responses/review/{reviewId}` | Svar för en recension |
| | `GET` · `PUT` · `DELETE /api/responses/{id}` | CRUD |
| **Platform tokens** | `/api/platformtokens` (`POST`, `GET /{id}`, `GET /business/{businessId}`, `PUT /{id}`, `DELETE /{id}`) | Hantera plattformstokens |
| **Trustpilot** | `GET /api/trustpilot/connect/{businessId}` | Returnerar URL för att starta OAuth |
| | `GET /api/trustpilot/callback` *(öppen)* | OAuth-callback, sparar tokens och redirectar till frontend |
| | `POST /api/trustpilot/sync/{businessId}` | Hämtar recensioner från Trustpilot |
| **Google** | `POST /api/googlereviews/fetch/{businessId}` | Hämtar recensioner (mockad tjänst) |
| **Mock** | `/api/mock/trustpilot/...` *(öppen)* | Fejkad Trustpilot för lokal utveckling |

Fel returneras i ett gemensamt format via `GlobalExceptionMiddleware`:

```json
{ "StatusCode": 404, "Message": "..." }
```

`KeyNotFoundException` → 404, `UnauthorizedAccessException` → 401, `ConflictException` → 409, övrigt → 500.

### Trustpilot-flödet

1. Frontend anropar `GET /api/trustpilot/connect/{businessId}` och skickar användaren till `authorizationUrl`.
2. Trustpilot (eller mocken) redirectar tillbaka till `/api/trustpilot/callback?code=…&state={businessId}`.
3. API:t byter koden mot access/refresh-token, sparar dem som `PlatformToken` och redirectar till `{Frontend:BaseUrl}/dashboard?trustpilot=connected` (eller `=error`).
4. `POST /api/trustpilot/sync/{businessId}` hämtar recensioner; tokenen förnyas automatiskt när den håller på att gå ut.

## Migrationer

```bash
dotnet ef migrations add <Namn>
dotnet ef database update
```

## CI

GitHub Actions (`.github/workflows/ci.yml`) kör restore, build och test vid push och pull requests mot `main` och `dev`.

## Kända begränsningar / TODO

- OAuth-`state` är bara `businessId` – bör signeras/krypteras mot CSRF före produktion.
- `connect` kontrollerar inte att inloggad användare äger företaget.
- `/api/user` (GET) kräver rollen `Admin`, men JWT:n innehåller ännu ingen roll-claim.
- Google-integrationen är mockad (`MockGoogleReviewService`).
- Inga testprojekt finns ännu.
