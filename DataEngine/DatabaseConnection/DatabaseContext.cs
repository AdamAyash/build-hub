namespace BuildHub.DataEngine.DatabaseConnection
{
	using Transactions;

	/// <summary>
	/// Manages thread-local database connections for the current async/thread context
	/// </summary>
	public sealed class DatabaseContext : IDisposable
	{
		private static readonly ThreadLocal<DatabaseContext> _currentThreadLocalDatabaseConnection
			= new ThreadLocal<DatabaseContext>(() => new DatabaseContext());

		private readonly DatabaseConnectionPool _databaseConnectionPool = DatabaseConnectionPool.GetInstance();
		private readonly Dictionary<DatabaseSource, DatabaseConnection> _contextDatabaseConnections;
		private bool _isDisposed = false;

		public ITransactionContext? TransactionContext { get; set; }

		private DatabaseContext()
		{
			this._databaseConnectionPool = DatabaseConnectionPool.GetInstance();
			this._contextDatabaseConnections = new Dictionary<DatabaseSource, DatabaseConnection>();
			this.TransactionContext = null;
		}

		~DatabaseContext() => Dispose(false);

		/// <summary>
		/// Gets the current connection context for this async flow
		/// </summary>
		public static DatabaseContext GetCurrentContext => _currentThreadLocalDatabaseConnection.Value;

		/// <summary>
		/// Determines whether a connection to the specified database source exists.
		/// </summary>
		/// <param name="databaseSource">The database source to check for an existing connection.</param>
		/// <returns><see langword="true"/> if a connection to the specified database source exists; otherwise</returns>
		public bool HasContextDatabaseConnection(DatabaseSource databaseSource) => this._contextDatabaseConnections.ContainsKey(databaseSource);

		/// <summary>
		/// Gets a connection for the specified database source, reusing if already exists in context
		/// </summary>
		/// <param name="databaseSource">Database source</param>
		/// <returns>DatabaseConnection</returns>
		public DatabaseConnection GetConnection(DatabaseSource databaseSource)
		{
			if (this._contextDatabaseConnections.TryGetValue(databaseSource, out var existingConnection))
			{
				DatabaseConnectionValidator databaseConnectionValidator = new(existingConnection, this.TransactionContext);
				if (databaseConnectionValidator.TestDatabaseConnection())
					return existingConnection;
			}

			var newConnection = this._databaseConnectionPool.GetDatabaseConnection(databaseSource);
			this._contextDatabaseConnections[databaseSource] = newConnection;

			return newConnection;
		}

		private void ClearContext()
		{
			foreach (var databaseConnection in this._contextDatabaseConnections.Values)
				databaseConnection.Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
				this.ClearContext();
		}
	}
}