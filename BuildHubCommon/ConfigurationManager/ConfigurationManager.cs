using BuildHubCommon.ConfigurationManager.Base;
using Microsoft.Extensions.Configuration;

namespace BuildHubCommon.ConfigurationManager
{
    /// <summary>
    /// Configuration manager class
    /// </summary>
    public class ConfigurationManager : BaseConfigurationManager
    {
        /// <summary>
        /// Configuration manager singleton instance
        /// </summary>
        private static ConfigurationManager? _configurationManager = null;

        private ConfigurationManager()
        {
        }

        /// <summary>
        /// Returns an instance to the configuration manager
        /// </summary>
        /// <returns></returns>
        public static ConfigurationManager GetConfigurationManager()
        {
            if (_configurationManager == null)
                _configurationManager = new ConfigurationManager();

            _configurationManager.Initialize();
            return _configurationManager;
        }

        /// <summary>
        /// Initializes the configuration manager
        /// </summary>
        protected override void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

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
