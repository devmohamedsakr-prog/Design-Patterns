using System;
using System.Collections.Generic;

namespace Prototype.Configuration.Settings.Context
{
    /// <summary>
    /// Product: Application settings with undo/redo via prototype cloning.
    /// Demonstrates: Prototype pattern for configuration state management.
    /// </summary>
    public class AppSettings
    {
        public string AppName { get; set; }
        public string Theme { get; set; } // Light, Dark
        public int FontSize { get; set; }
        public string Language { get; set; } // en, fr, de, ar
        public DatabaseSettings Database { get; set; }
        public NetworkSettings Network { get; set; }
        public SecuritySettings Security { get; set; }
        public Dictionary<string, object> CustomSettings { get; set; }
        public DateTime LastModified { get; set; }

        public AppSettings()
        {
            Database = new DatabaseSettings();
            Network = new NetworkSettings();
            Security = new SecuritySettings();
            CustomSettings = new Dictionary<string, object>();
            LastModified = DateTime.UtcNow;
        }

        /// <summary>
        /// Deep copy clone for snapshots/undo.
        /// </summary>
        public AppSettings Clone()
        {
            var clone = new AppSettings
            {
                AppName = this.AppName,
                Theme = this.Theme,
                FontSize = this.FontSize,
                Language = this.Language,
                Database = this.Database?.Clone(),
                Network = this.Network?.Clone(),
                Security = this.Security?.Clone(),
                LastModified = DateTime.UtcNow
            };

            foreach (var kvp in this.CustomSettings)
            {
                clone.CustomSettings[kvp.Key] = DeepCopyValue(kvp.Value);
            }

            return clone;
        }

        private static object DeepCopyValue(object value)
        {
            if (value == null || value is string || value is int || value is bool ||
                value is DateTime || value is decimal)
                return value;

            if (value is Dictionary<string, object> dict)
            {
                var newDict = new Dictionary<string, object>();
                foreach (var kvp in dict)
                    newDict[kvp.Key] = DeepCopyValue(kvp.Value);
                return newDict;
            }

            if (value is List<object> list)
                return new List<object>(list);

            return value;
        }

        public override string ToString()
        {
            return $"AppSettings(App={AppName}, Theme={Theme}, Language={Language}, " +
                   $"Modified={LastModified:G})";
        }
    }

    /// <summary>
    /// Database connection settings.
    /// </summary>
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
        public int PoolSize { get; set; }
        public bool EnableQueryLogging { get; set; }
        public IList<string> ReplicationServers { get; set; }

        public DatabaseSettings()
        {
            ReplicationServers = new List<string>();
            CommandTimeout = 30;
            PoolSize = 10;
        }

        public DatabaseSettings Clone()
        {
            var clone = new DatabaseSettings
            {
                ConnectionString = this.ConnectionString,
                CommandTimeout = this.CommandTimeout,
                PoolSize = this.PoolSize,
                EnableQueryLogging = this.EnableQueryLogging
            };

            foreach (var server in this.ReplicationServers)
                clone.ReplicationServers.Add(server);

            return clone;
        }

        public override string ToString() =>
            $"Database(Timeout={CommandTimeout}s, Pool={PoolSize}, Logging={EnableQueryLogging})";
    }

    /// <summary>
    /// Network and API settings.
    /// </summary>
    public class NetworkSettings
    {
        public string ProxyUrl { get; set; }
        public int ConnectionTimeout { get; set; }
        public int MaxRetries { get; set; }
        public bool EnableCompression { get; set; }
        public IList<string> AllowedDomains { get; set; }

        public NetworkSettings()
        {
            AllowedDomains = new List<string>();
            ConnectionTimeout = 30;
            MaxRetries = 3;
        }

        public NetworkSettings Clone()
        {
            var clone = new NetworkSettings
            {
                ProxyUrl = this.ProxyUrl,
                ConnectionTimeout = this.ConnectionTimeout,
                MaxRetries = this.MaxRetries,
                EnableCompression = this.EnableCompression
            };

            foreach (var domain in this.AllowedDomains)
                clone.AllowedDomains.Add(domain);

            return clone;
        }

        public override string ToString() =>
            $"Network(Timeout={ConnectionTimeout}s, Retries={MaxRetries}, Compression={EnableCompression})";
    }

    /// <summary>
    /// Security and authentication settings.
    /// </summary>
    public class SecuritySettings
    {
        public bool EnableEncryption { get; set; }
        public string EncryptionAlgorithm { get; set; } // AES, RSA
        public int PasswordMinLength { get; set; }
        public bool RequireMFA { get; set; }
        public int SessionTimeout { get; set; } // minutes
        public IList<string> DisabledFeatures { get; set; }

        public SecuritySettings()
        {
            DisabledFeatures = new List<string>();
            PasswordMinLength = 8;
            SessionTimeout = 60;
        }

        public SecuritySettings Clone()
        {
            var clone = new SecuritySettings
            {
                EnableEncryption = this.EnableEncryption,
                EncryptionAlgorithm = this.EncryptionAlgorithm,
                PasswordMinLength = this.PasswordMinLength,
                RequireMFA = this.RequireMFA,
                SessionTimeout = this.SessionTimeout
            };

            foreach (var feature in this.DisabledFeatures)
                clone.DisabledFeatures.Add(feature);

            return clone;
        }

        public override string ToString() =>
            $"Security(Encrypted={EnableEncryption}, MFA={RequireMFA}, SessionTimeout={SessionTimeout}min)";
    }

    /// <summary>
    /// Settings manager with undo/redo support via snapshots.
    /// </summary>
    public class SettingsManager
    {
        private AppSettings _current;
        private readonly Stack<AppSettings> _undoStack = new Stack<AppSettings>();
        private readonly Stack<AppSettings> _redoStack = new Stack<AppSettings>();
        private int _maxHistorySize = 20;

        public SettingsManager(AppSettings initialSettings)
        {
            if (initialSettings == null)
                throw new ArgumentNullException(nameof(initialSettings));

            _current = initialSettings.Clone();
        }

        public AppSettings Current => _current;

        public void ApplySettings(AppSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            // Push current state to undo stack
            _undoStack.Push(_current.Clone());
            
            // Clear redo stack on new change
            _redoStack.Clear();

            // Apply new settings
            _current = settings.Clone();

            // Limit history size
            if (_undoStack.Count > _maxHistorySize)
            {
                var list = new List<AppSettings>(_undoStack);
                _undoStack.Clear();
                for (int i = 0; i < _maxHistorySize; i++)
                    _undoStack.Push(list[i]);
            }
        }

        public void Undo()
        {
            if (_undoStack.Count == 0)
                throw new InvalidOperationException("Nothing to undo");

            _redoStack.Push(_current.Clone());
            _current = _undoStack.Pop();
        }

        public void Redo()
        {
            if (_redoStack.Count == 0)
                throw new InvalidOperationException("Nothing to redo");

            _undoStack.Push(_current.Clone());
            _current = _redoStack.Pop();
        }

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public int UndoStackSize => _undoStack.Count;
        public int RedoStackSize => _redoStack.Count;

        public void SetMaxHistorySize(int size)
        {
            if (size <= 0)
                throw new ArgumentException("Max history size must be greater than 0", nameof(size));

            _maxHistorySize = size;
        }

        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }

    /// <summary>
    /// Configuration presets using prototype pattern.
    /// </summary>
    public class SettingsPreset
    {
        private readonly Dictionary<string, AppSettings> _presets =
            new Dictionary<string, AppSettings>();

        public void RegisterPreset(string name, AppSettings settings)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _presets[name] = settings.Clone();
        }

        public AppSettings GetPreset(string name)
        {
            if (!_presets.ContainsKey(name))
                throw new KeyNotFoundException($"Preset '{name}' not found");

            return _presets[name].Clone();
        }

        public bool HasPreset(string name) => _presets.ContainsKey(name);

        public int PresetCount => _presets.Count;
    }
}
