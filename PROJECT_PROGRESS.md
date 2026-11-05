# PROJECT PROGRESS — MERQO VERTEXA

- **Current stage:** Stage 4 — Hardware Intelligence (core foundation implemented)
- **Version:** 1.0.0 (unreleased)
- **Branch:** arena/ab38dde1-merqo-vertexa
- **Last commit:** see `git log`
- **Last successful build:** none locally (Linux sandbox); CI builds on windows-latest
- **Last successful test run:** none yet (xUnit tests authored; run in CI)
- **Completed:** repository init, governance + planning docs, Core engine (models with
  provenance/status, WMI/CIM + powercfg providers, orchestrator with cancellation and
  error isolation, SQLite settings, logging, JSON/HTML/CSV report engine), WPF shell
  with dashboard/hardware/battery/storage/drivers/reports/settings pages, CI + release
  workflows, unit tests
- **In progress:** physical verification of collectors on real Windows hardware
- **Pending:** display/keyboard/audio/camera labs, benchmarks, health scoring, release
- **Known bugs:** none recorded
- **Blocked:** none
- **Hardware compatibility limitations:** sandbox is Linux; Windows build/test runs
  on GitHub Actions windows-latest runners and physical verification procedures
  documented in docs/TESTING_STRATEGY.md
- **Release readiness:** not ready

Update this file after every milestone.
