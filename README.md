# MERQO VERTEXA

**Advanced All-in-One PC & Laptop Hardware Diagnostics, Health Analysis, Inspection & Intelligence Platform**

- **Brand:** MERQO
- **Product:** MERQO VERTEXA
- **Founder:** Shohan Khan
- **Contact:** merqoonline@gmail.com
- **Target Platform:** Microsoft Windows 10 and Windows 11
- **Distribution:** GitHub Releases (unsigned EXE installer + portable EXE)
- **Version:** see [VERSION](VERSION)

> ONE SOFTWARE. COMPLETE PC INTELLIGENCE. ULTIMATE HARDWARE DIAGNOSTICS.

## Core principles

MERQO VERTEXA never fabricates hardware data. Every diagnostic value carries its
source, method, timestamp, status and confidence. Unsupported readings are reported
as `Unsupported`/`Unavailable` — never invented.

## Quick start (Windows)

```powershell
# Build (requires .NET 8 SDK on Windows)
./scripts/build.ps1

# Run the app
./src/Merqo.Vertexa.App/bin/Release/net8.0-windows/Merqo.Vertexa.App.exe
```

## Repository layout

| Path | Purpose |
|---|---|
| `src/Merqo.Vertexa.Core` | Diagnostic engine, providers, models, storage, logging, reports |
| `src/Merqo.Vertexa.App` | WPF desktop shell and UI |
| `tests/` | Unit and integration tests |
| `docs/` | PRD, architecture, capability matrix, design system, testing strategy |
| `.github/workflows/` | CI build/test and release automation |
| `scripts/` | Build/packaging scripts |

## Documentation

- [docs/PRD.md](docs/PRD.md) — Product Requirements Specification (traceable requirement IDs)
- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) — System architecture
- [docs/TECHNOLOGY_DECISION.md](docs/TECHNOLOGY_DECISION.md) — Why C# / .NET 8 / WPF
- [docs/CAPABILITY_MATRIX.md](docs/CAPABILITY_MATRIX.md) — Detection methods per data field
- [docs/DESIGN_SYSTEM.md](docs/DESIGN_SYSTEM.md) — UI design system
- [docs/TESTING_STRATEGY.md](docs/TESTING_STRATEGY.md) — QA strategy
- [docs/RELEASE_CHECKLIST.md](docs/RELEASE_CHECKLIST.md) — Release gates
- [PROJECT_PROGRESS.md](PROJECT_PROGRESS.md) — Live project status
- [FEATURE_CHECKLIST.md](FEATURE_CHECKLIST.md) — Feature states

## Security & privacy

Local-first. No telemetry by default, no mandatory cloud, no destructive storage
operations, no driver modification without consent. See [SECURITY.md](SECURITY.md).

## License

MIT — see [LICENSE](LICENSE).
