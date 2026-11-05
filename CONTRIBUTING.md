# Contributing to MERQO VERTEXA

## Commit convention

Use conventional-commit style prefixes:

`feat:` `fix:` `refactor:` `test:` `docs:` `build:` `ci:` `security:` `perf:`

Never commit a message claiming an unverified feature is complete.

## Branching

- `main` — protected default branch; releases are tagged from here.
- Feature work happens on short-lived branches named `feat/<area>` or the
  session branch assigned by the development environment.

## Pull requests

Every PR must state: what changed, why, how it was tested, and which requirement
IDs (see `docs/PRD.md`) it affects. Update `PROJECT_PROGRESS.md` and
`FEATURE_CHECKLIST.md` when a feature state changes.

## Code standards

- C# / .NET 8, nullable reference types enabled, `TreatWarningsAsErrors` in CI.
- No logic in code-behind beyond view wiring; view models carry behavior.
- All diagnostics run asynchronously with `CancellationToken` support.
- Every diagnostic result must carry source, method, timestamp, status and
  confidence. Never return fabricated values.
- UI must never block on diagnostic work.
