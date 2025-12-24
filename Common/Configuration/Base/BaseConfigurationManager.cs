using Microsoft.Extensions.Configuration;

namespace BuildHub.Common.Configuration.Base
{
	using Application;

	/// <summary>
	/// Base class for the configuration managers.
	/// </summary>
	public class BaseConfigurationManager
	{
		/// <summary>
		/// Constant configuration file name
		/// </summary>
		protected const string _CONFIGURATION_FILE_NAME = "appsettings.json";

		/// <summary>
		/// Whether the configuration should reload on change
		/// </summary>
		protected readonly bool _reloadOnChange;

		/// <summary>
		/// Whether the configuration is optional
		/// </summary>
		protected readonly bool _isOptional;

		/// <summary>
		/// Configuration interface
		/// </summary>
		protected IConfiguration? _configuration;

		protected BaseConfigurationManager()
		{
			this._reloadOnChange = true;
			this._isOptional = false;
		}

		/// <summary>
		/// Initializes the configuration
		/// </summary>
		protected virtual void Initialize()
		{
			try
			{
				var builder = new ConfigurationBuilder()
				.AddEnvironmentVariables()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile(_CONFIGURATION_FILE_NAME, optional: this._isOptional,
				reloadOnChange: this._reloadOnChange);

				_configuration = builder.Build();
			}
			catch (Exception exception)
			{
				Application.ExitWithError(exception, "Failed to initialize configuration manager.");
			}
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
		public IReadOnlyList<ConfigurationModel>? GetConfigurationModels<ConfigurationModel>(string key)
			  where ConfigurationModel : IConfigurationModel
		{
			return _configuration!.GetSection(key).Get<IReadOnlyList<ConfigurationModel>>();
		}
	}
}