# Changelog

All notable changes to MERQO VERTEXA are documented here.
Format follows [Keep a Changelog](https://keepachangelog.com/), versioning follows
Semantic Versioning.

## [1.0.0] - Unreleased

### Added
- Initial project scaffolding: repository structure, governance docs, planning docs.
- PRD with traceable requirement IDs (docs/PRD.md).
- Architecture, technology decision, diagnostic capability matrix, design system,
  testing strategy, development plan, release checklist.
- Core diagnostic engine (`Merqo.Vertexa.Core`): provider framework with
  multi-method fallback, diagnostic result model with provenance and status,
  WMI/CIM-based hardware discovery (CPU, GPU, RAM, motherboard, storage, battery,
  Windows, drivers), settings (SQLite), logging, report engine (JSON/HTML/CSV).
- WPF application shell (`Merqo.Vertexa.App`): sidebar navigation, dashboard,
  hardware pages, settings, reports.
- GitHub Actions CI (build + test on windows-latest) and release workflow
  (self-contained EXE, SHA256SUMS.txt).
