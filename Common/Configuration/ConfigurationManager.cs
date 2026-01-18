namespace BuildHub.Common.Configuration
{
	using Base;

	/// <summary>
	/// Configuration manager class
	/// </summary>
	public class ConfigurationManager : BaseConfigurationManager
	{
		/// <summary>
		/// Configuration manager singleton instance
		/// </summary>
		private static ConfigurationManager? _configurationManagerInstance = null;
		private ConfigurationManager()
		{
		}

		/// <summary>
		/// Returns an instance to the configuration manager
		/// </summary>
		/// <returns></returns>
		public static ConfigurationManager GetConfigurationManager()
		{
			if (_configurationManagerInstance is null)
				_configurationManagerInstance = new ConfigurationManager();

			_configurationManagerInstance.Initialize();

			return _configurationManagerInstance;
		}
	}
}
