namespace BuildHub.DataEngine.DatabaseConnection
{
	/// <summary>
	/// Manages thread-local database connections for the current async/thread context
	/// </summary>
	public sealed class DatabaseContext : IDisposable
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

		~DatabaseContext() => Dispose(false);
		

		/// <summary>
		/// Gets the current connection context for this async flow
		/// </summary>
		public static DatabaseContext GetCurrentContext 
			=> _currentAsyncLocalDatabaseConnection?.Value ?? new DatabaseContext();

		/// <summary>
		/// Determines whether a connection to the specified database source exists.
		/// </summary>
		/// <param name="databaseSource">The database source to check for an existing connection.</param>
		/// <returns><see langword="true"/> if a connection to the specified database source exists; otherwise</returns>
		public bool HasContexDatabaseConnection(DatabaseSource databaseSource) => this._contextDatabaseConnections.ContainsKey(databaseSource);

		/// <summary>
		/// Gets a connection for the specified database source, reusing if already exists in context
		/// </summary>
		/// <param name="databaseSource">Database source</param>
		/// <returns>DatabaseConnection</returns>
		public DatabaseConnection GetConnection(DatabaseSource databaseSource)
		{
			if (this._contextDatabaseConnections.TryGetValue(databaseSource, out var existingConnection))
				return existingConnection;

			var newConnection = this._databaseConnectionPool.GetDatabaseConnection(databaseSource);
			this._contextDatabaseConnections[databaseSource] = newConnection;

			return newConnection;
		}

		private void ClearContext()
		{
			foreach(var  databaseConnection in this._contextDatabaseConnections.Values)
				databaseConnection.Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				if (disposing)
					this.ClearContext();

				_isDisposed = true;
			}
		}
	}
}