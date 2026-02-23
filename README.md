# clinical-ai

Clinical decision-support backend that:
- models clinical workflows (Patient/Encounter/Observation/Alert/Task)
- uses incremental ingestion (append-only + watermarks)
- serves ML risk predictions + survival analysis
- provides provenance (lineage ledger) for alert explainability
- integrates with FHIR (later milestone)

## Prereqs
- Docker Desktop
- .NET SDK 8.0.404 (pinned via `global.json`)

## Start Postgres
```bash
docker compose -f infra/docker/docker-compose.yml up -d
```

## Apply Migrations
```bash
dotnet tool restore

dotnet dotnet-ef database update \
  --project services/clinical-api/src/Clinical.Infrastructure/Clinical.Infrastructure.csproj \
  --startup-project services/clinical-api/src/Clinical.Api/Clinical.Api.csproj
```

## Run API
```bash
dotnet run --project services/clinical-api/src/Clinical.Api/Clinical.Api.csproj
```

## API Endpoints
- `POST /api/events`
- `POST /api/pipelines/features_hourly/run`
