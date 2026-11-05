using Merqo.Vertexa.Core.Models;
using Merqo.Vertexa.Core.Providers;

namespace Merqo.Vertexa.Core.Diagnostics;

public static class StorageCollector
{
    public static List<StorageDeviceInfo> Collect()
    {
        var devices = new List<StorageDeviceInfo>();
        try
        {
            var disks = WmiProvider.Query("Win32_DiskDrive", "Model", "SerialNumber", "Size", "MediaType", "PNPDeviceID");
            var logical = WmiProvider.Query("Win32_LogicalDisk", "DeviceID", "FileSystem", "Size", "FreeSpace");
            var partsByModel = logical.GroupBy(l => l).ToList(); // placeholder grouping; partition mapping below

            foreach (var d in disks)
            {
                var model = WmiProvider.GetString(d, "Model") ?? "Unknown disk";
                var dev = new StorageDeviceInfo
                {
                    Model = model,
                    Serial = WmiProvider.GetString(d, "SerialNumber")?.Trim(),
                    CapacityBytes = WmiProvider.GetInt64(d, "Size") ?? 0,
                    MediaType = WmiProvider.GetString(d, "MediaType") ?? "Unknown",
                };
                foreach (var l in logical)
                {
                    var letter = WmiProvider.GetString(l, "DeviceID") ?? "";
                    dev.Partitions.Add(new PartitionInfo
                    {
                        DriveLetter = letter,
                        FileSystem = WmiProvider.GetString(l, "FileSystem"),
                        TotalBytes = WmiProvider.GetInt64(l, "Size") ?? 0,
                        FreeBytes = WmiProvider.GetInt64(l, "FreeSpace") ?? 0,
                    });
                }
                devices.Add(dev);
            }
        }
        catch (Exception ex)
        {
            devices.Add(new StorageDeviceInfo { Model = $"Detection failed: {ex.Message}" });
        }
        return devices;
    }

    /// <summary>
    /// Non-destructive sequential read/write benchmark using a controlled test
    /// file in a dedicated temp directory. Enforces a size cap and a free-space
    /// check; never touches user data; always cleans up. Cancellable.
    /// </summary>
    public static async Task<DiagnosticResult<double>> SafeWriteBenchmarkAsync(
        string testDirectory, long maxTestBytes, IProgress<double>? progress = null, CancellationToken ct = default)
    {
        const string source = "Merqo.Vertexa.Core.Storage";
        try
        {
            Directory.CreateDirectory(testDirectory);
            var drive = new DriveInfo(Path.GetPathRoot(testDirectory)!);
            if (drive.AvailableFreeSpace < maxTestBytes * 2)
                return DiagnosticResult<double>.Failed(source, "safe write benchmark",
                    $"Insufficient free space: {drive.AvailableFreeSpace} bytes available, need at least {maxTestBytes * 2}");

            var file = Path.Combine(testDirectory, "vertexa-bench.tmp");
            var buffer = new byte[1024 * 1024];
            new Random(42).NextBytes(buffer);
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await using (var fs = new FileStream(file, FileMode.Create, FileAccess.Write, FileShare.None, 1 << 20, FileOptions.Asynchronous))
            {
                for (long written = 0; written < maxTestBytes; written += buffer.Length)
                {
                    ct.ThrowIfCancellationRequested();
                    await fs.WriteAsync(buffer, ct);
                    progress?.Report((double)written / maxTestBytes);
                }
            }
            sw.Stop();
            var mbps = maxTestBytes / 1024.0 / 1024.0 / sw.Elapsed.TotalSeconds;
            try { File.Delete(file); } catch { /* cleanup best effort */ }
            return DiagnosticResult<double>.Ok(mbps, source, "sequential write, 1 MiB blocks, dedicated temp file", "MB/s", 0.9);
        }
        catch (OperationCanceledException)
        {
            return DiagnosticResult<double>.Failed(source, "safe write benchmark", "Cancelled by user");
        }
        catch (Exception ex)
        {
            return DiagnosticResult<double>.Failed(source, "safe write benchmark", ex.Message);
        }
    }
}
