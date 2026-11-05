# MERQO VERTEXA — Product Requirements Specification

Every requirement has a unique ID, priority (P1 = release-blocking, P2 = core,
P3 = post-1.0), implementation approach, verification method and acceptance
criteria. Status is tracked in `FEATURE_CHECKLIST.md`.

## SYS — System & shell
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| SYS-001 | App launches on Win10/11, no crash on unelevated run | P1 | WPF .NET 8, single-instance mutex | Manual launch test on Win10/11 |
| SYS-002 | Sidebar + top bar navigation, all entries functional | P1 | WPF Frame navigation, view models | UI test: every nav item opens a working page |
| SYS-003 | Dark and light themes, persisted | P1 | Theme resource dictionaries, settings store | Toggle test + restart persistence |
| SYS-004 | Async diagnostics; UI never blocks | P1 | Task-based orchestrator, CancellationToken | Stress test run while interacting with UI |
| SYS-005 | Global error handling; no unhandled exceptions crash app | P1 | DispatcherUnhandledException + per-module try/catch | Fault-injection test |
| SYS-006 | Settings persisted locally (SQLite) with migrations | P1 | Microsoft.Data.Sqlite, schema versioning | Corrupt-DB recovery test |
| SYS-007 | Structured logging to local file, no PII | P1 | Serilog-style rolling file sink (own impl) | Log inspection |

## CPU
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| CPU-001 | Identify manufacturer/model/architecture/cores/threads | P1 | WMI Win32_Processor + Registry | Compare with msinfo32 on test machines |
| CPU-002 | Base/max/current frequency, per-core load | P1 | WMI + PerformanceCounter (Processor Information) | Sanity check vs Task Manager |
| CPU-003 | Cache, instruction sets, virtualization | P2 | WMI + CPUID via native interop where available | Cross-check with CPU-Z on physical HW |
| CPU-004 | Bounded single/multi-core stress test with thermal stop | P2 | Worker tasks, configurable duration, cancel | Run 60s test, verify stop conditions |

## GPU
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| GPU-001 | Detect all GPUs, driver version/date, VRAM | P1 | WMI Win32_VideoController | Compare with Device Manager |
| GPU-002 | Utilization/temp where exposed | P2 | Performance counters where available | Mark Unsupported when not exposed |
| GPU-003 | Bounded rendering stability test | P3 | Simple D3D workload, bounded | Physical HW verification procedure |

## RAM
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| RAM-001 | Total/available/used, per-module details (DDR3/4/5) | P1 | WMI Win32_PhysicalMemory + Win32_PhysicalMemoryArray | Compare with Task Manager |
| RAM-002 | Bounded allocation read/write verification | P2 | Allocate bounded buffer, verify pattern | Unit test + run test |

## BAT — Battery
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| BAT-001 | Design/full charge capacity, wear %, health %, cycle count | P1 | `powercfg /batteryreport` + WMI BatteryFullChargedCapacity / BatteryStaticData + Win32_Battery | Formula check on laptop; document when firmware hides cycle count |
| BAT-002 | Voltage/current/charge rate/status | P2 | WMI BatteryStatus / Win32_PortableBattery where available | Mark Unsupported when absent |

## STR — Storage
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| STR-001 | Disks, partitions, FS, free/used | P1 | WMI Win32_DiskDrive/LogicalDisk/Volume | Compare with Disk Management |
| STR-002 | SMART/NVMe health via `wmic`/StorageReliabilityCounter or SMART IOCTL (admin) | P2 | Read-only SMART query, non-elevated fallback | Document permission requirements |
| STR-003 | Non-destructive sequential R/W benchmark in temp dir, with free-space check, cancel, cleanup | P1 | Controlled test file in %TEMP%\MerqoVertexa, size-limited | Verify no user data touched; cleanup verified |

## DSP/KEY/AUD/CAM — Labs
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| DSP-001 | Fullscreen color/gradient/pattern tests, Esc exit, multi-monitor | P1 | Borderless fullscreen window | Manual test on multi-monitor setup |
| KEY-001 | Keyboard capture lab with visual feedback, passed/untested lists | P1 | Low-level hook or focused KeyDown capture | Physical keyboard test procedure |
| AUD-001 | Tone playback L/R, mic level meter, local record/playback | P2 | NAudio-free: Windows Multimedia / WASAPI via NAudio-free P/Invoke or System.Windows.Media | Manual audio test |
| CAM-001 | Webcam detection + live preview | P2 | Windows.Media.Capture (WinRT) | Physical camera test procedure |

## DRV — Drivers
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| DRV-001 | Driver inventory, problem codes, unknown/disabled devices | P1 | WMI Win32_PnPSignedDriver + Win32_PnPEntity ConfigManagerErrorCode | Compare with Device Manager |

## RPT — Reporting
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| RPT-001 | Export JSON/HTML/CSV reports with branding, version, provenance, limitations | P1 | Report engine in Core; HTML print-friendly | Open exported files, validate schema |

## SEC — Security
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| SEC-001 | No telemetry, offline-capable, no secrets in repo | P1 | No network calls in Core by default | Network monitor during test run; grep for secrets |
| SEC-002 | Least privilege; elevation only when required | P1 | No requestedExecutionLevel requireAdministrator | UAC prompt absence on normal run |

## REL — Release
| ID | Requirement | Pri | Approach | Verification |
|---|---|---|---|---|
| REL-001 | CI builds unsigned EXE + portable on windows-latest | P1 | GitHub Actions, pinned SDK | Workflow run inspection |
| REL-002 | SHA256SUMS.txt + release notes per release | P1 | Release workflow | Artifact inspection |
