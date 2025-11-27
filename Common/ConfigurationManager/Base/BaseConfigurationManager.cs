using Microsoft.Extensions.Configuration;

namespace BuildHub.Common.ConfigurationManager.Base
{
	/// <summary>
	/// Base class for the configuration managers.
	/// </summary>
	public abstract class BaseConfigurationManager
	{
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
		protected abstract void Initialize();
	}
}