using BuildHub.Common.Utilities;

namespace BuildHub.DataEngine.DatabaseConnection
{
	/// <summary>
	/// Manages thread-local database connections for the current async/thread context
	/// </summary>
	public sealed class DatabaseConnectionContext : IDisposable
	{
		private static readonly AsyncLocal<DatabaseConnectionContext>? _currentAsyncLocalDatabaseConnection;

		private readonly DatabaseConnectionPool _databaseConnectionPool = DatabaseConnectionPool.GetInstance();
		private readonly Dictionary<DatabaseSource, DatabaseConnection> _contextDatabaseConnections;
		private bool _isDisposed = false;

		private DatabaseConnectionContext()
		{
			this._databaseConnectionPool = DatabaseConnectionPool.GetInstance();
			this._contextDatabaseConnections = new Dictionary<DatabaseSource, DatabaseConnection>();
		}

		~DatabaseConnectionContext() => Dispose(false);
		
		/// <summary>
		/// Gets the current connection context for this async flow
		/// </summary>
		public static DatabaseConnectionContext GetCurrentContext 
			=> _currentAsyncLocalDatabaseConnection?.Value ?? new DatabaseConnectionContext();

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
			if(_isDisposed)
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(DatabaseConnection)));

			if (this._contextDatabaseConnections.TryGetValue(databaseSource, out var existingConnection))
				return existingConnection;

			var newConnection = this._databaseConnectionPool.GetDatabaseConnection(databaseSource);
			this._contextDatabaseConnections[databaseSource] = newConnection;

			return newConnection;
		}

		private void ClearContext()
		{
			foreach(var databaseConnection in this._contextDatabaseConnections.Values)
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