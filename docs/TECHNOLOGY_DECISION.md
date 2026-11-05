# Technology Selection — MERQO VERTEXA

## Decision: C# / .NET 8 / WPF (Windows Presentation Foundation)

### Why not the alternatives
- **WinUI 3 / Windows App SDK:** modern, but heavier packaging, less mature
  tooling for unsigned distribution, and harder to build reproducibly on CI
  runners. WPF covers all required UI needs with far lower risk.
- **C++/Win32 or Rust:** maximum control, but WMI/CIM/PnP integration and rapid
  UI iteration are far slower; not justified for this product.
- **Electron/Tauri:** cross-platform web shells add a large runtime, weaker
  native Windows API access, and larger attack surface — poor fit for a
  diagnostics tool that must read hardware deeply.

### Why .NET 8 + WPF wins
1. **Native Windows integration:** direct access to WMI/CIM
   (`System.Management`), performance counters, PnP APIs, powercfg, WinRT
   (`Windows.Media.Capture`) — all first-class.
2. **Performance:** compiled, low overhead for benchmarks and monitoring.
3. **Reproducible Windows builds:** `dotnet publish` on `windows-latest` GitHub
   runners produces a self-contained single-file EXE (portable) plus an
   installer-friendly layout — no paid signing required.
4. **Maintainability & testability:** xUnit, DI-friendly modular Core library.
5. **Single framework:** one desktop framework, no mixed-stack complexity.

### Data storage
SQLite via `Microsoft.Data.Sqlite` for settings, diagnostic history and report
metadata, with schema migrations.

### Packaging
- Portable: self-contained single-file `win-x64` EXE.
- Installer: folder-based EXE published via GitHub Releases (unsigned per
  release policy; SmartScreen warning disclosed in release notes).
