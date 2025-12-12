using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Protocols.Configuration;

namespace BuildHub.DataEngine.DatabaseConnection
{
	using DataEngine.Configuration;
	using Exceptions;
	using Common.Utilities;
	using Common.Logger;

	using DatabaseConfigurationsMap = Dictionary<DatabaseSource, Configuration.DatabaseConfiguration>;

	/// <summary>
	/// Database connection pool singleton, initializing and managing a number of database connections.
	/// </summary>
	public sealed class DatabaseConnectionPool : IDisposable
	{
		private static readonly Lazy<DatabaseConnectionPool> _databaseConnectionPoolInstance = new Lazy<DatabaseConnectionPool>(() => new DatabaseConnectionPool());
		private DatabaseConfigurationManager _configurationManager;

		/// <summary>
		/// Represents a collection of database connection configurations.
		/// </summary>
		/// <remarks>This field holds the configurations for connecting to one or more databases.  It is intended for
		/// internal use and should not be accessed directly outside of the containing class.</remarks>
		private DatabaseConfigurationsMap _databaseConfigurationsMap;
		/// <summary>Available connection ready for use</summary>
		private readonly Dictionary<DatabaseSource, Queue<DatabaseConnection>> _availableDatabaseConnectionsMap;

		/// <summary>
		/// An object used to synchronize access to shared resources in a multi-threaded environment.
		/// </summary>
		/// <remarks>This field is intended to be used as a locking mechanism to ensure thread safety when accessing
		/// or modifying shared data. Always use this object with a <c>lock</c> statement to avoid race conditions.</remarks>
		private readonly object _mutex = new object();
		private bool _isDisposed;

		private DatabaseConnectionPool()
		{
			this._configurationManager = DatabaseConfigurationManager.GetInstance();
			this._availableDatabaseConnectionsMap = new Dictionary<DatabaseSource, Queue<DatabaseConnection>>();
			this._databaseConfigurationsMap = new Dictionary<DatabaseSource, DatabaseConfiguration>();
			this._isDisposed = false;

			Initialize();
		}

		~DatabaseConnectionPool() => Dispose(false);

		/// <summary>
		/// Returns an instance to the connection pool
		/// </summary>
		/// <returns>DatabaseConnectionPool</returns>
		public static DatabaseConnectionPool GetInstance() => _databaseConnectionPoolInstance.Value;

		/// <summary>
		/// Returns the number of currently available database connections.
		/// </summary>
		/// <param name="databaseSource">Source of the database</param>
		/// <returns>int</returns>
		public int GetAvailableDatabaseConnectionsCount(DatabaseSource databaseSource)
		{
			if (_isDisposed)
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(DatabaseConnection)));

			lock (_mutex)
			{
				if (!_availableDatabaseConnectionsMap.TryGetValue(databaseSource, out var databaseConnections))
					throw new MissingDatabaseConfigurationException(databaseSource);

				return databaseConnections.Count;
			}
		}
		/// <summary>
		/// Returns the number of currently used database connections.
		/// </summary>
		/// <param name="databaseSource">Source of the database</param>
		/// <returns>int</returns>
		public int GetCurrentlyUsedConnectionsCount(DatabaseSource databaseSource)
		{
			if (_isDisposed)
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(DatabaseConnection)));

			lock (_mutex)
			{
				var databaseConfiguration = _databaseConfigurationsMap[databaseSource];
				if (databaseConfiguration is null)
					throw new MissingDatabaseConfigurationException(databaseSource);

				return databaseConfiguration.MaxPoolConnections - this._availableDatabaseConnectionsMap[databaseSource].Count;
			}
		}

		/// <summary>
		/// Returns a reference to a database connection
		/// </summary>
		/// <returns>DatabaseConnection</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public DatabaseConnection GetDatabaseConnection(DatabaseSource databaseSource)
		{
			if (_isDisposed)
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(DatabaseConnection)));

			lock (_mutex)
			{
				int retryCount = 0;
				DatabaseConnection? databaseConnection = null;
				DatabaseConfiguration databaseConfiguration = _databaseConfigurationsMap[databaseSource];

				while (retryCount < databaseConfiguration.RetrieveConnectionRetryCount)
				{
					if (!_availableDatabaseConnectionsMap.TryGetValue(databaseSource, out var availableDatabaseConnections))
						throw new MissingDatabaseConfigurationException(databaseSource);

					if (availableDatabaseConnections.Count > 0)
					{
						databaseConnection = availableDatabaseConnections.Dequeue();
						break;
					}

					Thread.Sleep(databaseConfiguration.RetrieveConnectionTimeout);
					retryCount++;
				}

				if(databaseConnection is null)
				{
					Logger.LogWarning($"Connection pool exhausted for {databaseSource}.");
					throw new ConnectionPoolExhaustedException(databaseSource);
				}

				DatabaseConnectionValidator databaseConnectionValidator = new(databaseConnection);
				if (!databaseConnectionValidator.TestDatabaseConnection())
				{
					//TODO EXCEPTION
				}

				return databaseConnection;
			}
		}

		/// <summary>
		/// Returns a database connection to the pool
		/// </summary>
		/// <param name="databaseConnection"></param>
		public void ReleaseDatabaseConnection(DatabaseConnection databaseConnection)
		{
			if (_isDisposed)
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(DatabaseConnection)));

			lock (_mutex)
			{
				var databaseSource = databaseConnection.DatabaseSource;
				if (!this._availableDatabaseConnectionsMap.TryGetValue(databaseSource, out var availableDatabaseConnections))
					throw new MissingDatabaseConfigurationException(databaseSource);

				if (databaseConnection.IsConnectionOpen())
				{
					var databaseConfiguration = this._databaseConfigurationsMap[databaseSource];
					if (databaseConfiguration is null)
						throw new MissingDatabaseConfigurationException(databaseSource);

					if (availableDatabaseConnections.Count < databaseConfiguration.MaxPoolConnections)
					{
						if(!availableDatabaseConnections.Contains(databaseConnection))
							availableDatabaseConnections.Enqueue(databaseConnection);
					}
					else
					{
						databaseConnection.CloseConnection();
						Logger.LogInformation($"Closing excess connection for database: {databaseSource}");
					}
				}
			}
		}

		private string GetConnectionString(DatabaseSource databaseSource)
		{
			string connectionStringKey = Utilities.GetEnumDescription<DatabaseSource>(databaseSource);
			string connectionString = this._configurationManager.GetConnectionString(connectionStringKey);

			if (string.IsNullOrEmpty(connectionString))
				throw new EmptyConnectionStringException(databaseSource);

			return connectionString;
		}

		/// <summary>
		/// Initializes and opens a connection to the specified database source.
		/// </summary>
		/// <remarks>This method retrieves the connection string for the specified database source, attempts to open
		/// the connection,  and validates the connection. If the connection cannot be opened or fails validation, the
		/// connection is closed  and an exception is thrown.</remarks>
		/// <param name="databaseSource">The database source to connect to.</param>
		/// <returns>A <see cref="DatabaseConnection"/> instance representing the established connection.</returns>
		/// <exception cref="DatabaseConnectionValidationException">Thrown if the connection to the specified database source fails validation.</exception>
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
				databaseConnection.CloseConnection();
				Logger.LogError(exception, $"An error occured while trying to open connection for {databaseSource}");
				throw;
			}

			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);
			if (!databaseConnectionValidator.TestDatabaseConnection())
			{
				databaseConnection.CloseConnection();
				throw new DatabaseConnectionValidationException(databaseSource);
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
			if (databaseConfiguration.MaxPoolConnections < 1)
				throw new InvalidConfigurationException("MaxPoolConnections must be at least 1");

			var availableDatabaseConnections = new Queue<DatabaseConnection>();

			for (var index = 0; index < databaseConfiguration.MinPoolConnections; index++)
			{
				DatabaseSource databaseSource = databaseConfiguration.DatabaseSource;

				DatabaseConnection databaseConnection = InitializeConnection(databaseConfiguration.DatabaseSource);
				availableDatabaseConnections.Enqueue(databaseConnection);

				Logger.LogInformation($"Connection for database {databaseSource} was successfully initialized.");
			}

			this._availableDatabaseConnectionsMap.Add(databaseConfiguration.DatabaseSource, availableDatabaseConnections);
		}

		/// <summary>
		/// Initializes a number of database connections
		/// </summary>
		private void Initialize()
		{
			_databaseConfigurationsMap = _configurationManager.GetDatabaseConfigurations();

			var databaseSources = Utilities.GetEnumValues<DatabaseSource>();
			if (databaseSources.Count() != _databaseConfigurationsMap.Count())
				throw new MissingDatabaseConfigurationException();

			foreach (DatabaseConfiguration databaseConfiguration in _databaseConfigurationsMap.Values)
				InitializeConnections(databaseConfiguration);

			Logger.LogInformation("Database connection pool initialized.");
		}

		/// <summary>
		/// Closes all connections.
		/// </summary>
		private void Cleanup()
		{
			lock (_mutex)
			{
				foreach (var databaseConnections in this._availableDatabaseConnectionsMap.Values)
				{
					foreach (DatabaseConnection databaseConnection in databaseConnections)
						databaseConnection.CloseConnection();

					databaseConnections.Clear();
				}
			}
		}

		/// <summary>
		/// Releases the resources used by the current instance of the class.
		/// </summary>
		/// <remarks>This method should be called when the instance is no longer needed to free up resources.  It
		/// suppresses finalization to prevent the garbage collector from calling the finalizer.</remarks>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the resources used by the current instance of the class.
		/// </summary>
		/// <remarks>This method should be called when the instance is no longer needed to ensure that all resources 
		/// are properly released. Once disposed, the instance should not be used further.</remarks>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
					Cleanup();

				_isDisposed = true;
			}
		}
	}
}
