namespace BuildHub.DataEngine.DatabaseConnection
{
	using Common.Logger;

	/// <summary>
	/// Manages thread-local database connections for the current async/thread context
	/// </summary>
	public sealed class DatabaseContext
	{
		private static readonly AsyncLocal<DatabaseContext>? _currentAsyncLocalDatabaseConnection;

		private readonly DatabaseConnectionPool _databaseConnectionPool = DatabaseConnectionPool.GetInstance();
		private readonly Dictionary<DatabaseSource, DatabaseConnection> _contextDatabaseConnections;
		private bool _isDisposed = false;

		private DatabaseContext()
		{
			this._databaseConnectionPool = DatabaseConnectionPool.GetInstance();
			this._contextDatabaseConnections = new Dictionary<DatabaseSource, DatabaseConnection>();
		}

		/// <summary>
		/// Gets the current connection context for this async flow
		/// </summary>
		public static DatabaseContext GetCurrentContext => _currentAsyncLocalDatabaseConnection?.Value ?? new DatabaseContext();

		/// <summary>
		/// Gets a connection for the specified database source, reusing if already exists in context
		/// </summary>
		/// <param name="databaseSource">Database source</param>
		/// <returns>DatabaseConnection</returns>
		public DatabaseConnection GetConnection(DatabaseSource databaseSource)
		{
			if (_contextDatabaseConnections.TryGetValue(databaseSource, out var existingConnection))
				return existingConnection;

			var newConnection = _databaseConnectionPool.GetDatabaseConnection(databaseSource);
			_contextDatabaseConnections[databaseSource] = newConnection;

			return newConnection;
		}
	}
}