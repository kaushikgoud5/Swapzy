# Swapzy

A Tinder-style product swapping platform built with .NET 8, PostgreSQL + PostGIS, and AWS services.

## Local Development

### Prerequisites
- [Docker](https://docs.docker.com/get-docker/) & Docker Compose

### Run everything (API + DB + AWS)
```bash
docker compose up
```

This starts:
- **PostgreSQL + PostGIS** on `localhost:5432`
- **LocalStack** (S3, SNS, SQS) on `localhost:4566`
- **Swapzy API** on `http://localhost:8080`

Swagger UI: http://localhost:8080/swagger

### Run only dependencies (for IDE debugging)
```bash
docker compose up db localstack
```

Then run the API from your IDE — it will connect to the Docker services via `appsettings.Development.json`.

### Seed Data
```bash
# Connect to the database and run:
psql -h localhost -U postgres -d swapzydb -f seed_categories.sql
psql -h localhost -U postgres -d swapzydb -f seed_authorization_data.sql
```

### Apply Migrations
```bash
dotnet ef database update --project src/Swapzy.Infrastructure --startup-project src/Swapzy.Api
```
