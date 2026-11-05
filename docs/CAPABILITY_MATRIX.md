# Diagnostic Capability Matrix

For each data field: primary → fallback detection methods, privilege needs, and
failure handling. Rule: if no source yields a valid value, status is
`Unsupported`/`Unavailable` — never fabricated.

| Field | Primary | Fallback(s) | Admin? | Failure handling |
|---|---|---|---|---|
| CPU model/cores/threads | WMI Win32_Processor | Registry HKLM\HARDWARE\DESCRIPTION\System\CentralProcessor | No | Unavailable + reason |
| CPU current freq / per-core load | PerformanceCounter `Processor Information` | WMI Win32_Processor (load only) | No | Partial (load only) |
| CPU temperature/power | WMI `MSAcpi_ThermalZoneTemperature` (where exposed) | OEM WMI namespaces | No | Unsupported — never invented |
| GPU list, driver, VRAM | WMI Win32_VideoController | PnP Win32_PnPEntity + driver store | No | Unavailable |
| GPU temp/util | Perf counters (where exposed) | — | No | Unsupported |
| RAM total/available | WMI Win32_OperatingSystem + OS perf counter | GlobalMemoryStatusEx | No | Unavailable |
| RAM modules (DDR gen, speed, part#) | WMI Win32_PhysicalMemory | SMBIOS via WMI MSFT_PhysicalMemory | No | Partial |
| Battery capacities/cycles | `powercfg /batteryreport` XML + WMI BatteryStaticData / Win32_PortableBattery | WMI BatteryFullChargedCapacity | No | Field-level Unsupported; wear/health only when both capacities valid > 0 |
| Disk list/partitions/free | WMI Win32_DiskDrive / Win32_LogicalDisk / Win32_Volume | DiskPart list (read-only) | No | Unavailable |
| SMART / NVMe health | WMI StorageReliabilityCounter / MSFT_Disk + NVMe via `wmic diskdrive` | SMART read via storage IOCTL (admin) | Optional | PermissionRequired, never fake health |
| Motherboard/BIOS | WMI Win32_BaseBoard / Win32_BIOS | Registry | No | Unavailable |
| Windows edition/build/boot/TPM/SecureBoot | WMI Win32_OperatingSystem + `bcdedit`/`Confirm-SecureBootUEFI` (read-only) | Registry DisplayVersion | No | Partial |
| Drivers/problems | WMI Win32_PnPSignedDriver + Win32_PnPEntity error codes | SetupAPI log parse | No | Partial |
| Uptime | PerformanceCounter or GetTickCount64 | WMI LastBootUpTime | No | Verified |

## Statuses
`Verified | Partial | Unavailable | Unsupported | PermissionRequired | Failed | NotTested | Estimated`
(`Estimated` only with a documented calculation method.)
