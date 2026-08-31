# Address Management API

Eine RESTful API zur Verwaltung von Adressen, umgesetzt mit ASP.NET Core, EF Core (InMemory)
und einer klaren Schichtentrennung nach dem Service-Repository-Pattern.

## Architekturüberblick

```
src/
├── AddressManagement.Api             → Controller, Middleware, Startup (Program.cs)
├── AddressManagement.Application     → DTOs, Interfaces, Services, Mapping
├── AddressManagement.Domain          → Entities (keine Abhängigkeiten zu anderen Layern)
└── AddressManagement.Infrastructure  → DbContext, Repository-Implementierung (EF Core)

tests/
├── AddressManagement.UnitTests        → Service-Layer-Tests (Repository gemockt)
└── AddressManagement.IntegrationTests → End-to-End-Tests via WebApplicationFactory
```
**Abhängigkeitsrichtung:** `Api → Infrastructure → Application → Domain`
Domain kennt niemanden. Application kennt nur Domain. Infrastructure implementiert die in
Application definierten Interfaces (Dependency Inversion). Api verdrahtet alles über
Dependency Injection.

## Designentscheidungen

- **Service-Repository-Pattern:** `IAddressRepository` kapselt die Persistenz (EF Core),
  `IAddressService` enthält die Anwendungslogik (Validierung, Mapping, Paging-Defaults).
  Der Controller bleibt dünn und delegiert nur.
- **Manuelles DTO-Mapping** statt AutoMapper: Bei einer einzigen, kleinen Entity bringt eine
  Mapping-Bibliothek mehr Indirektion als Nutzen. Die Mapping-Logik liegt transparent direkt
  in `AddressService`.
- **RFC 7807 Problem Details:** Eine globale `ExceptionHandlingMiddleware` fängt unerwartete
  Exceptions ab und wandelt sie in `ProblemDetails`-Responses um. Für 404/400-Fälle nutzt
  ASP.NET Core (ab .NET 8) bereits automatisch dieses Format.
- **`AsNoTracking()` bei Lesezugriffen:** Da Change-Tracking nur beim Schreiben gebraucht wird,
  verbessert das die Performance und vermeidet unbeabsichtigte Tracking-Konflikte.
- **Bonus-Features:**
  - **PATCH** (`AddressPatchDto`) für Partial Updates – nur gesetzte Felder werden übernommen.
  - **Pagination** (`page`, `pageSize`) auf `GET /api/addresses`, inkl. `TotalCount`/`TotalPages`
    in der Response.
  - **Swagger/OpenAPI** ist unter `/swagger` im Development-Modus verfügbar.
  - **Strikte Trennung zwischen Entity und DTO** über alle Endpunkte hinweg.
  -   - **Optimistic Concurrency** via `RowVersion`-Property. Hinweis: Der InMemory-Provider
    erzwingt Concurrency-Tokens nicht vollständig realistisch wie eine echte relationale
    Datenbank (z. B. SQL Server) — der Code ist korrekt aufgebaut, aber der 409-Conflict-Fall
    lässt sich mit InMemory nicht zuverlässig auslösen/testen.
      - **JWT-Authentifizierung:** Alle `/api/addresses`-Endpunkte sind mit `[Authorize]` geschützt.
    Login über `POST /api/auth/login` (Demo-Nutzer: `admin` / `password123`) liefert einen
    signierten JWT-Token, der im `Authorization: Bearer <token>`-Header mitgeschickt werden muss.
    Getestet über die Integrationstests (`CreateAuthenticatedClientAsync`), die den echten
    Login-Flow durchlaufen.
  
## Ausführung

Voraussetzung: .NET 8 SDK (oder neuer)

```bash
# Restore & Build
dotnet restore
dotnet build

# API starten (Swagger öffnet sich automatisch im Development-Profil)
dotnet run --project src/AddressManagement.Api

# Alle Tests ausführen
dotnet test
```

Die API läuft danach lokal (Port wird beim Start im Terminal angezeigt, z. B. `http://localhost:5157`),
Swagger UI ist unter `/swagger` erreichbar.

## API-Endpunkte

| Methode | Route                     | Beschreibung                                   |
|---------|----------------------------|------------------------------------------------|
| POST    | `/api/addresses`          | Adresse anlegen (201 + Location-Header)        |
| GET     | `/api/addresses/{id}`     | Adresse abrufen (200 / 404)                    |
| GET     | `/api/addresses`          | Liste mit Filter (`city`, `postalCode`) & Paging (`page`, `pageSize`) |
| PUT     | `/api/addresses/{id}`     | Vollständiges Update (200 / 404)               |
| PATCH   | `/api/addresses/{id}`     | Partielles Update (200 / 404) — Bonus          |
| DELETE  | `/api/addresses/{id}`     | Löschen (204 / 404)                            |

## Testing-Ansatz (TDD)

Der Service-Layer wurde testgetrieben entwickelt: für jede Methode zuerst ein fehlschlagender
Test (Repository-Interface gemockt mit NSubstitute), dann die minimale Implementierung, danach
Refactoring. Die Integrationstests prüfen den kompletten Request/Response-Zyklus inklusive
Statuscodes und Fehlerformat über `WebApplicationFactory` gegen eine echte (in-memory)
EF-Core-Instanz.