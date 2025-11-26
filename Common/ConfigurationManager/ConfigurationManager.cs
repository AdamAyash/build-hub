using Microsoft.Extensions.Configuration;

namespace BuildHub.Common.ConfigurationManager
{
	using Base;

	/// <summary>
	/// Configuration manager class
	/// </summary>
	public sealed class ConfigurationManager : BaseConfigurationManager
	{
		private const string _CONFIGURATION_FILE_NAME = "appsettings.json";

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
				.AddEnvironmentVariables()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile(_CONFIGURATION_FILE_NAME, optional: this._isOptional,
				reloadOnChange: this._reloadOnChange);

			_configuration = builder.Build();
		}

		/// <summary>
		/// Gets a connection string
		/// </summary>
		public string GetConnectionString(string key)
		{
			var connectionString = _configuration?.GetConnectionString(key);
			return connectionString ?? string.Empty;
		}

		/// <summary>
		/// Retrieves a configuration model
		/// </summaryConfigurationModel
		/// <typeparam name="ConfugurationSettingsModel">Type of the configuration</typeparam>
		/// <param name="key">key of the configuration</param>
		/// <returns>ConfugurationSettingsModel</returns>
		public ConfigurationModel? GetConfigurationModel<ConfigurationModel>(string key)
			where ConfigurationModel : IConfigurationModel
		{
			return _configuration!.GetSection(key).Get<ConfigurationModel>();
		}

		/// <summary>
		/// Retrieves a collection of configuration models
		/// </summary>
		/// <typeparam name="ConfigurationModelConfigurationModel>Type of the configuration</typeparam>
		/// <param name="key"></param>
		/// <returns>IEnumerable<ConfugurationSettingsModel></returns>
		public IEnumerable<ConfigurationModel>? GetConfigurationModels<ConfigurationModel>(string key)
			  where ConfigurationModel : IConfigurationModel
		{
			return _configuration!.GetSection(key).Get<IEnumerable<ConfigurationModel>>();
		}
	}
}
