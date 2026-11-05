# MERQO VERTEXA — Architecture

## Solution layout

```
src/
  MerqoVertexa.sln
  Merqo.Vertexa.Core/          # Diagnostic engine (no UI dependencies)
    Diagnostics/               # Orchestrator + per-module collectors
    Providers/                 # WMI/CIM, PerformanceCounter, powercfg, PnP providers
    Models/                    # DiagnosticResult<T>, enums, hardware models
    Reporting/                 # JSON/HTML/CSV report engine
    Storage/                   # SQLite settings + history, migrations
    Logging/                   # Rolling file logger
  Merqo.Vertexa.App/           # WPF shell (net8.0-windows)
    Views/ ViewModels/ Themes/ Services/
tests/
  Merqo.Vertexa.Core.Tests/    # xUnit unit + integration tests
```

## Layers

1. **Application Shell** — window, navigation, theming, settings binding.
2. **UI/Presentation** — WPF views + MVVM view models; zero hardware logic.
3. **Diagnostic Orchestrator** — runs module collectors asynchronously with
   `CancellationToken`, timeouts, progress reporting, error isolation.
4. **Provider layer** — each field is fetched through a chain of providers
   (primary → secondary → fallback). Results are validated before acceptance.
5. **Models** — `DiagnosticResult<T>` carries Value, Unit, Source, Method,
   Timestamp, Status (`Verified|Partial|Unavailable|Unsupported|PermissionRequired|Failed|NotTested|Estimated`),
   Confidence and Error.
6. **Storage/Logging** — SQLite for settings/history, rolling file log.

## Key patterns

- **Multi-method fallback:** `DiagnosticField<T>` resolves via ordered providers;
  first valid result wins; provenance preserved; conflicts reported.
- **Async everywhere:** UI calls `IAsyncDiagnostic` methods; progress via
  `IProgress<DiagnosticProgress>`.
- **Error isolation:** a failing module is caught, logged, and surfaced as
  `Failed` status — never crashes the app or other modules.
- **No fabrication:** unavailable data returns `Unsupported`/`Unavailable`
  with the reason, never an invented value.
