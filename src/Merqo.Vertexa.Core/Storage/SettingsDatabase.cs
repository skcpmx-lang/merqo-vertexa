using Microsoft.Data.Sqlite;

namespace Merqo.Vertexa.Core.Storage;

/// <summary>
/// SQLite-backed local settings store with schema versioning and safe recovery
/// from corrupted databases (backed up and recreated, never crashing).
/// </summary>
public sealed class SettingsDatabase : IDisposable
{
    private const int CurrentSchemaVersion = 1;
    private readonly string _dbPath;
    private SqliteConnection? _connection;

    public SettingsDatabase(string dbPath)
    {
        _dbPath = dbPath;
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        OpenAndMigrate();
    }

    private void OpenAndMigrate()
    {
        try
        {
            _connection = new SqliteConnection($"Data Source={_dbPath}");
            _connection.Open();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = """
                CREATE TABLE IF NOT EXISTS AppSettings (
                    Key TEXT PRIMARY KEY,
                    Value TEXT NOT NULL
                );
                CREATE TABLE IF NOT EXISTS SchemaVersion (
                    Version INTEGER PRIMARY KEY
                );
                INSERT OR IGNORE INTO SchemaVersion (Version) VALUES (1);
                """;
            cmd.ExecuteNonQuery();
        }
        catch (SqliteException)
        {
            // Corrupted DB: back it up and start fresh rather than crash.
            try
            {
                _connection?.Dispose();
                var backup = _dbPath + ".corrupt-" + DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                if (File.Exists(_dbPath)) File.Move(_dbPath, backup);
            }
            catch { /* best effort */ }
            _connection = new SqliteConnection($"Data Source={_dbPath}");
            _connection.Open();
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS AppSettings (Key TEXT PRIMARY KEY, Value TEXT NOT NULL);";
            cmd.ExecuteNonQuery();
        }
    }

    public string? Get(string key)
    {
        using var cmd = _connection!.CreateCommand();
        cmd.CommandText = "SELECT Value FROM AppSettings WHERE Key = $key";
        cmd.Parameters.AddWithValue("$key", key);
        return cmd.ExecuteScalar() as string;
    }

    public void Set(string key, string value)
    {
        using var cmd = _connection!.CreateCommand();
        cmd.CommandText = "INSERT INTO AppSettings (Key, Value) VALUES ($key, $value) ON CONFLICT(Key) DO UPDATE SET Value = $value";
        cmd.Parameters.AddWithValue("$key", key);
        cmd.Parameters.AddWithValue("$value", value);
        cmd.ExecuteNonQuery();
    }

    public void Dispose() => _connection?.Dispose();
}
