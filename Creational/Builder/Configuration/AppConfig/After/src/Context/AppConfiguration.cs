using System;
using System.Collections.Generic;
using System.Linq;

namespace Builder.Configuration.AppConfig.Context
{
    /// <summary>
    /// Product: Immutable application configuration constructed via builder.
    /// Demonstrates: Step-by-step configuration with validation at build time.
    /// </summary>
    public class AppConfiguration
    {
        public string AppName { get; }
        public string Environment { get; } // Development, Staging, Production
        public DatabaseConfig Database { get; }
        public LoggingConfig Logging { get; }
        public SecurityConfig Security { get; }
        public CacheConfig Cache { get; }
        public IReadOnlyDictionary<string, string> CustomSettings { get; }
        public DateTime BuildTime { get; }

        private AppConfiguration(
            string appName,
            string environment,
            DatabaseConfig database,
            LoggingConfig logging,
            SecurityConfig security,
            CacheConfig cache,
            IReadOnlyDictionary<string, string> customSettings)
        {
            AppName = appName;
            Environment = environment;
            Database = database;
            Logging = logging;
            Security = security;
            Cache = cache;
            CustomSettings = customSettings;
            BuildTime = DateTime.UtcNow;
        }

        public static AppConfigBuilder Builder => new AppConfigBuilder();

        public override string ToString()
        {
            return $"AppConfiguration(App={AppName}, Env={Environment}, DB={Database?.Server}, " +
                   $"LogLevel={Logging?.Level}, CacheEnabled={Cache?.Enabled})";
        }

        public class DatabaseConfig
        {
            public string Server { get; }
            public int Port { get; }
            public string Database { get; }
            public string Username { get; }
            public int ConnectionPoolSize { get; }
            public int CommandTimeout { get; }
            public bool EnableLogging { get; }

            public DatabaseConfig(string server, int port, string database, string username,
                int poolSize, int timeout, bool enableLogging)
            {
                Server = server;
                Port = port;
                Database = database;
                Username = username;
                ConnectionPoolSize = poolSize;
                CommandTimeout = timeout;
                EnableLogging = enableLogging;
            }

            public override string ToString() =>
                $"DatabaseConfig(Server={Server}:{Port}, DB={Database}, PoolSize={ConnectionPoolSize})";
        }

        public class LoggingConfig
        {
            public string Level { get; } // Debug, Info, Warning, Error, Critical
            public string Format { get; } // Json, PlainText
            public string Output { get; } // Console, File, Both
            public string FilePath { get; }
            public int RetentionDays { get; }
            public bool IncludeStackTrace { get; }

            public LoggingConfig(string level, string format, string output, string filePath,
                int retentionDays, bool includeStackTrace)
            {
                Level = level;
                Format = format;
                Output = output;
                FilePath = filePath;
                RetentionDays = retentionDays;
                IncludeStackTrace = includeStackTrace;
            }

            public override string ToString() =>
                $"LoggingConfig(Level={Level}, Format={Format}, Output={Output})";
        }

        public class SecurityConfig
        {
            public bool EnableHttps { get; }
            public string JwtSecret { get; }
            public int JwtExpirationMinutes { get; }
            public int MaxLoginAttempts { get; }
            public int LockoutDurationMinutes { get; }
            public IReadOnlyList<string> AllowedOrigins { get; }

            public SecurityConfig(bool enableHttps, string jwtSecret, int jwtExpiration,
                int maxAttempts, int lockoutDuration, IReadOnlyList<string> origins)
            {
                EnableHttps = enableHttps;
                JwtSecret = jwtSecret;
                JwtExpirationMinutes = jwtExpiration;
                MaxLoginAttempts = maxAttempts;
                LockoutDurationMinutes = lockoutDuration;
                AllowedOrigins = origins;
            }

            public override string ToString() =>
                $"SecurityConfig(HTTPS={EnableHttps}, JWT Exp={JwtExpirationMinutes}min, Origins={AllowedOrigins.Count})";
        }

        public class CacheConfig
        {
            public bool Enabled { get; }
            public string Provider { get; } // Redis, InMemory, Memcached
            public int ExpirationMinutes { get; }
            public int MaxSize { get; }
            public string RedisConnection { get; }

            public CacheConfig(bool enabled, string provider, int expiration, int maxSize, string redisConnection)
            {
                Enabled = enabled;
                Provider = provider;
                ExpirationMinutes = expiration;
                MaxSize = maxSize;
                RedisConnection = redisConnection;
            }

            public override string ToString() =>
                $"CacheConfig(Enabled={Enabled}, Provider={Provider}, Expiration={ExpirationMinutes}min)";
        }

        /// <summary>
        /// Builder class: Fluent API for constructing AppConfiguration.
        /// </summary>
        public class AppConfigBuilder
        {
            private string _appName;
            private string _environment = "Production";
            private string _dbServer;
            private int _dbPort = 5432;
            private string _dbName;
            private string _dbUsername;
            private int _dbPoolSize = 10;
            private int _dbTimeout = 30;
            private bool _dbLogging = false;
            private string _logLevel = "Info";
            private string _logFormat = "Json";
            private string _logOutput = "Console";
            private string _logFilePath = "logs/app.log";
            private int _logRetentionDays = 7;
            private bool _logStackTrace = true;
            private bool _securityHttps = true;
            private string _jwtSecret;
            private int _jwtExpiration = 60;
            private int _maxLoginAttempts = 5;
            private int _lockoutDuration = 15;
            private readonly List<string> _allowedOrigins = new();
            private bool _cacheEnabled = true;
            private string _cacheProvider = "InMemory";
            private int _cacheExpiration = 60;
            private int _cacheMaxSize = 1000;
            private string _redisConnection;
            private readonly Dictionary<string, string> _customSettings = new();

            public AppConfigBuilder AppName(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("App name cannot be null or empty", nameof(name));
                _appName = name;
                return this;
            }

            public AppConfigBuilder Environment(string env)
            {
                if (!new[] { "Development", "Staging", "Production" }.Contains(env))
                    throw new ArgumentException("Environment must be Development, Staging, or Production", nameof(env));
                _environment = env;
                return this;
            }

            // Database configuration
            public AppConfigBuilder WithDatabase(string server, int port, string database, string username)
            {
                if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(database) || 
                    string.IsNullOrWhiteSpace(username))
                    throw new ArgumentException("Server, database, and username are required");
                _dbServer = server;
                _dbPort = port;
                _dbName = database;
                _dbUsername = username;
                return this;
            }

            public AppConfigBuilder DatabasePoolSize(int poolSize)
            {
                if (poolSize <= 0)
                    throw new ArgumentException("PoolSize must be greater than 0", nameof(poolSize));
                _dbPoolSize = poolSize;
                return this;
            }

            public AppConfigBuilder DatabaseCommandTimeout(int seconds)
            {
                if (seconds <= 0)
                    throw new ArgumentException("Timeout must be greater than 0", nameof(seconds));
                _dbTimeout = seconds;
                return this;
            }

            public AppConfigBuilder EnableDatabaseLogging()
            {
                _dbLogging = true;
                return this;
            }

            // Logging configuration
            public AppConfigBuilder WithLogging(string level = "Info", string format = "Json")
            {
                if (!new[] { "Debug", "Info", "Warning", "Error", "Critical" }.Contains(level))
                    throw new ArgumentException("Invalid log level", nameof(level));
                if (!new[] { "Json", "PlainText" }.Contains(format))
                    throw new ArgumentException("Invalid log format", nameof(format));
                _logLevel = level;
                _logFormat = format;
                return this;
            }

            public AppConfigBuilder LogOutput(string output, string filePath = null)
            {
                if (!new[] { "Console", "File", "Both" }.Contains(output))
                    throw new ArgumentException("Output must be Console, File, or Both", nameof(output));
                _logOutput = output;
                if (output == "File" || output == "Both")
                {
                    if (string.IsNullOrWhiteSpace(filePath))
                        throw new ArgumentException("FilePath is required when logging to file", nameof(filePath));
                    _logFilePath = filePath;
                }
                return this;
            }

            public AppConfigBuilder LogRetention(int days)
            {
                if (days <= 0)
                    throw new ArgumentException("RetentionDays must be greater than 0", nameof(days));
                _logRetentionDays = days;
                return this;
            }

            // Security configuration
            public AppConfigBuilder WithSecurity(string jwtSecret)
            {
                if (string.IsNullOrWhiteSpace(jwtSecret))
                    throw new ArgumentException("JWT secret cannot be null or empty", nameof(jwtSecret));
                _jwtSecret = jwtSecret;
                return this;
            }

            public AppConfigBuilder DisableHttps()
            {
                _securityHttps = false;
                return this;
            }

            public AppConfigBuilder JwtExpiration(int minutes)
            {
                if (minutes <= 0)
                    throw new ArgumentException("Expiration must be greater than 0", nameof(minutes));
                _jwtExpiration = minutes;
                return this;
            }

            public AppConfigBuilder LoginSecurity(int maxAttempts, int lockoutMinutes)
            {
                if (maxAttempts <= 0 || lockoutMinutes <= 0)
                    throw new ArgumentException("Values must be greater than 0");
                _maxLoginAttempts = maxAttempts;
                _lockoutDuration = lockoutMinutes;
                return this;
            }

            public AppConfigBuilder AddAllowedOrigin(string origin)
            {
                if (string.IsNullOrWhiteSpace(origin))
                    throw new ArgumentException("Origin cannot be null or empty", nameof(origin));
                _allowedOrigins.Add(origin);
                return this;
            }

            // Cache configuration
            public AppConfigBuilder WithCache(string provider = "InMemory", int expirationMinutes = 60)
            {
                if (!new[] { "Redis", "InMemory", "Memcached" }.Contains(provider))
                    throw new ArgumentException("Provider must be Redis, InMemory, or Memcached", nameof(provider));
                if (expirationMinutes <= 0)
                    throw new ArgumentException("Expiration must be greater than 0", nameof(expirationMinutes));
                _cacheProvider = provider;
                _cacheExpiration = expirationMinutes;
                return this;
            }

            public AppConfigBuilder DisableCache()
            {
                _cacheEnabled = false;
                return this;
            }

            public AppConfigBuilder CacheMaxSize(int size)
            {
                if (size <= 0)
                    throw new ArgumentException("MaxSize must be greater than 0", nameof(size));
                _cacheMaxSize = size;
                return this;
            }

            public AppConfigBuilder RedisConnection(string connection)
            {
                if (string.IsNullOrWhiteSpace(connection))
                    throw new ArgumentException("Connection cannot be null or empty", nameof(connection));
                _redisConnection = connection;
                return this;
            }

            // Custom settings
            public AppConfigBuilder AddCustomSetting(string key, string value)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Key and value cannot be null or empty");
                _customSettings[key] = value;
                return this;
            }

            public AppConfiguration Build()
            {
                if (string.IsNullOrWhiteSpace(_appName))
                    throw new InvalidOperationException("App name is required");
                if (string.IsNullOrWhiteSpace(_dbServer))
                    throw new InvalidOperationException("Database server is required");
                if (string.IsNullOrWhiteSpace(_jwtSecret))
                    throw new InvalidOperationException("JWT secret is required");

                var dbConfig = new DatabaseConfig(_dbServer, _dbPort, _dbName, _dbUsername,
                    _dbPoolSize, _dbTimeout, _dbLogging);
                var logConfig = new LoggingConfig(_logLevel, _logFormat, _logOutput, _logFilePath,
                    _logRetentionDays, _logStackTrace);
                var secConfig = new SecurityConfig(_securityHttps, _jwtSecret, _jwtExpiration,
                    _maxLoginAttempts, _lockoutDuration, _allowedOrigins.AsReadOnly());
                var cacheConfig = new CacheConfig(_cacheEnabled, _cacheProvider, _cacheExpiration,
                    _cacheMaxSize, _redisConnection);

                return new AppConfiguration(
                    _appName,
                    _environment,
                    dbConfig,
                    logConfig,
                    secConfig,
                    cacheConfig,
                    new Dictionary<string, string>(_customSettings)
                );
            }
        }
    }
}
