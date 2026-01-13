#region
using BuildHub.Common.Configuration.Base;
using BuildHub.DataEngine.Exceptions.DatabaseConnection;
using Microsoft.Extensions.Configuration;
#endregion

namespace BuildHub.DataEngine.Configuration
{
	using DatabaseConfigurationsMap = Dictionary<DatabaseSource, DatabaseConfiguration>;

	/// <summary>
	/// Manages database configurations and provides access to database-related settings and connection strings.
	/// </summary>
	/// <remarks>This class is a singleton and provides methods to retrieve database configurations and connection
	/// strings. Use <see cref="GetInstance"/> to obtain the singleton instance of this class. The class ensures that
	/// database configurations are loaded and validated during initialization.</remarks>
	internal class DatabaseConfigurationManager : BaseConfigurationManager
	{
		private const string _DATABASE_CONFIGURATIONS_KEY = "DatabaseConfigurations";

		/// <summary>
		/// Database configuration manager singleton instance
		/// </summary>
		private static DatabaseConfigurationManager? _databaseConfigurationManagerInstance = null;
		private DatabaseConfigurationsMap _databaseConfigurationsMap;

		private DatabaseConfigurationManager()
		{
			this._databaseConfigurationsMap = new DatabaseConfigurationsMap();

		}

		/// <summary>
		/// Returns an instance to the configuration manager
		/// </summary>
		/// <returns></returns>
		public static DatabaseConfigurationManager GetInstance()
		{
			if (_databaseConfigurationManagerInstance is null)
				_databaseConfigurationManagerInstance = new DatabaseConfigurationManager();

			_databaseConfigurationManagerInstance.Initialize();
			return _databaseConfigurationManagerInstance;
		}

		/// <summary>
		/// Gets a connection string
		/// </summary>
		public string GetConnectionString(string key)
		{
			var connectionString = _configuration?.GetConnectionString(key);
			return connectionString ?? string.Empty;
		}

		protected override void Initialize()
		{
			base.Initialize();
			LoadDatabaseConfigurations();
		}

		private void LoadDatabaseConfigurations()
		{
			IReadOnlyList<DatabaseConfiguration>? databaseConfigurations = GetConfigurationModels<DatabaseConfiguration>(_DATABASE_CONFIGURATIONS_KEY);
			if (databaseConfigurations is null)
				throw new MissingDatabaseConfigurationException();

			if (databaseConfigurations.Count() != databaseConfigurations.DistinctBy(config => config.DatabaseSource).Count())
				throw new DuplicateDatabaseConfigurationException();

			this._databaseConfigurationsMap = databaseConfigurations.ToDictionary(config => config.DatabaseSource);
		}

		public DatabaseConfigurationsMap GetDatabaseConfigurations() => _databaseConfigurationsMap;
	}
}
