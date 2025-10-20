using Microsoft.Extensions.Configuration;

namespace BuildHubCommon.ConfigurationManager
{
    /// <summary>
    /// Configuration manager class
    /// </summary>
    public sealed class ConfigurationManager
    {
        /// <summary>
        /// Configuration manager singleton instance
        /// </summary>
        private static ConfigurationManager? _configurationManager = null;

        private IConfiguration? _configuration;

        private bool _reloadOnChange = true;
        private bool _isOptional = true;

        private ConfigurationManager()
        {
            Initialize();
        }

        /// <summary>
        /// Returns an instance to the configuration manager
        /// </summary>
        /// <returns></returns>
        public static ConfigurationManager GetConfigurationManager()
        {
            if (_configurationManager == null)
                _configurationManager = new ConfigurationManager();

            return _configurationManager;
        }

        /// <summary>
        /// Initializes the configuration manager
        /// </summary>
        private void Initialize()
        {
            var builder = new ConfigurationBuilder();
            builder.AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        /// <summary>
        /// Gets a configuration value by key
        /// </summary>
        public string? GetValue(string key, string? defaultValue = null) => _configuration?[key] ?? defaultValue;

        /// <summary>
        /// Gets a connection string
        /// </summary>
        public string? GetConnectionString(string key)
        {
            var connectionString = _configuration?.GetConnectionString(key);
            return connectionString;
        }
    }
}
