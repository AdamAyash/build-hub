using Microsoft.Data.SqlClient;

namespace BuildHub.DataEngine.DatabaseConnection
{
	using Common.Logger;
	using Common.Utilities;
	using Common.Application;
	using Common.ConfigurationManager;

	/// <summary>
	/// Database connection pool singleton, initializing and managing a number of database connections.
	/// </summary>
	public sealed class DatabaseConnectionPool
	{
		/// <summary>Singleton instance to the connection pool</summary>
		private static DatabaseConnectionPool? _databaseConnectionPoolInstance = null;
		/// <summary>ConfigurationManager</summary>
		private readonly ConfigurationManager _configurationManager;

		/// <summary>Available connection ready for use</summary>
		private readonly Dictionary<DatabaseSource, List<DatabaseConnection>> _availableDatabaseConnectionsMap;
		/// <summary>Currently used connections</summary>
		private readonly Dictionary<DatabaseSource, List<DatabaseConnection>> _currentlyUsedDatabaseConnectionsMap;

		private DatabaseConnectionPool()
		{
			this._availableDatabaseConnectionsMap = new Dictionary<DatabaseSource, List<DatabaseConnection>>();
			this._currentlyUsedDatabaseConnectionsMap = new Dictionary<DatabaseSource, List<DatabaseConnection>>();
			this._configurationManager = ConfigurationManager.GetConfigurationManager();

			Initialize();
		}

		~DatabaseConnectionPool()
		{
			Cleanup();
		}

		/// <summary>
		/// Returns an instance to the connection pool
		/// </summary>
		/// <returns>DatabaseConnectionPool</returns>
		public static DatabaseConnectionPool GetInstance()
		{
			if (_databaseConnectionPoolInstance == null)
				_databaseConnectionPoolInstance = new DatabaseConnectionPool();

			return _databaseConnectionPoolInstance;
		}

		/// <summary>
		/// Returns the number of currently available database connections.
		/// </summary>
		/// <param name="databaseSource">Source of the database</param>
		/// <returns>int</returns>
		public int GetAvailableDatabaseConnectionsCount(DatabaseSource databaseSource)
			=> this._availableDatabaseConnectionsMap[databaseSource].Count;

		/// <summary>
		/// Returns the number of currently used database connections.
		/// </summary>
		/// <param name="databaseSource">Source of the database</param>
		/// <returns>int</returns>
		public int GetCurrentlyUsedConnectionsCount(DatabaseSource databaseSource)
			=> this._currentlyUsedDatabaseConnectionsMap[databaseSource].Count;

		/// <summary>
		/// Returns a reference to a database connection
		/// </summary>
		/// <returns>DatabaseConnection</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public DatabaseConnection GetDatabaseConnection(DatabaseSource databaseSource)
		{
			List<DatabaseConnection> availableDatabaseConnections = this._availableDatabaseConnectionsMap[databaseSource];
			List<DatabaseConnection> currentlyUsedDatabaseConnections = this._currentlyUsedDatabaseConnectionsMap[databaseSource];

			var databaseConnection = availableDatabaseConnections.FirstOrDefault();

			if (databaseConnection == null)
				databaseConnection = InitializeConnection(databaseSource);

			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);
			if (!databaseConnectionValidator.TestDatabaseConnection())
			{
				Application.ExitWithError($"Database connection test failed for {databaseSource} database.");
			}

			currentlyUsedDatabaseConnections.Add(databaseConnection);
			availableDatabaseConnections.Remove(databaseConnection);

			return databaseConnection;
		}

		/// <summary>
		/// Returns a database connection to the pool
		/// </summary>
		/// <param name="databaseConnection"></param>
		public void ReleaseDatabaseConnection(DatabaseConnection databaseConnection)
		{
			var databaseSource = databaseConnection.DatabaseSource;

			if (databaseConnection.IsConnectionOpen())
				this._availableDatabaseConnectionsMap[databaseSource].Add(databaseConnection);

			this._currentlyUsedDatabaseConnectionsMap[databaseSource].Remove(databaseConnection);
		}

		private string GetConnectionString(DatabaseSource databaseSource)
		{
			string connectionStringKey = Utilities.GetEnumDescription<DatabaseSource>(databaseSource);
			string connectionString = this._configurationManager.GetConnectionString(connectionStringKey);

			if (string.IsNullOrEmpty(connectionString))
				Application.ExitWithError($"Connection string for {databaseSource} database is empty.");

			return connectionString;
		}

		private DatabaseConnection InitializeConnection(DatabaseSource databaseSource)
		{
			var connectionString = GetConnectionString(databaseSource);
			var databaseConnection = new DatabaseConnection(databaseSource, connectionString);

			try
			{
				databaseConnection.OpenConnection();
			}
			catch (SqlException exception)
			{
				Application.ExitWithError(exception, $"Failed to open database connection to {databaseSource} database.");
			}

			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);
			if (!databaseConnectionValidator.TestDatabaseConnection())
			{
				Application.ExitWithError($"Database connection test failed for {databaseSource} database.");
			}

			return databaseConnection;
		}

		/// <summary>
		/// Initializes the connections for a specific database configuration
		/// </summary>
		/// <param name="databaseConfiguration">Datbase configuration model</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		/// <exception cref="InvalidOperationException"></exception>
		private void InitializeConnections(DatabaseConfiguration databaseConfiguration)
		{
			var availableDatabaseConnections = new List<DatabaseConnection>();

			for (var index = 0; index < databaseConfiguration.MaxPoolConnections; index++)
			{
				DatabaseConnection databaseConnection = InitializeConnection(databaseConfiguration.DatabaseSource);
				availableDatabaseConnections.Add(databaseConnection);
			}

			this._availableDatabaseConnectionsMap.Add(databaseConfiguration.DatabaseSource, availableDatabaseConnections);
			this._currentlyUsedDatabaseConnectionsMap.Add(databaseConfiguration.DatabaseSource, new List<DatabaseConnection>());
		}
		
		/// <summary>
		/// Initializes a number of database connections
		/// </summary>
		private void Initialize()
		{
			var databaseConfigurations = _configurationManager.GetConfigurationModels<DatabaseConfiguration>("DatabaseConfigurations");

			if (databaseConfigurations is null)
				Application.ExitWithError("No database configurations found. Application will terminate.");

			databaseConfigurations = databaseConfigurations?.DistinctBy(x => x.DatabaseSource);

			foreach (var databaseConfiguration in databaseConfigurations)
			{
				InitializeConnections(databaseConfiguration);
			}

			Logger.LogInformation("Database connection pool initialized.");
		}

		/// <summary>
		/// Closes all connections.
		/// </summary>
		private void Cleanup()
		{
			foreach (var databaseConnections in this._availableDatabaseConnectionsMap.Values)
			{
				foreach (DatabaseConnection databaseConnection in databaseConnections)
					databaseConnection.CloseConnection();

				databaseConnections.Clear();
			}

			foreach (var databaseConnections in this._currentlyUsedDatabaseConnectionsMap.Values)
			{
				foreach (DatabaseConnection databaseConnection in databaseConnections)
					databaseConnection.CloseConnection();

				databaseConnections.Clear();
			}
		}
	}
}
