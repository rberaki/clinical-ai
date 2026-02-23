# ADR 0001: Monorepo structure for clinical-ai

## Decision
Use a monorepo with /services for deployable components and /docs for system design, runbooks, and ADRs.

## Rationale
- Supports multiple services (clinical API, ML service, feature pipeline) in one coherent system.
- Enables consistent local development via Docker Compose.

## Consequences
- CI will run per-service builds.
- Shared contracts live under /packages to avoid tight coupling.
