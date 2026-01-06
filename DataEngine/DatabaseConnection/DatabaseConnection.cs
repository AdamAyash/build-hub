using System.Data;
using Microsoft.Data.SqlClient;

namespace BuildHub.DataEngine.DatabaseConnection
{
	/// <summary>
	/// Represents a connection to a database. Virtual proxy to the actual SqlConnection. 
	/// </summary>
	public class DatabaseConnection : IDisposable
	{
		public SqlConnection InternalConnection { get; private set; }
		public DatabaseSource DatabaseSource { get; private set; }
		public bool IsConnectionPooled { get; internal set; }

		public DatabaseConnection(DatabaseSource databaseSource, string connectionString)
		{
			this.InternalConnection = new SqlConnection(connectionString);
			this.DatabaseSource = databaseSource;
			this.IsConnectionPooled = true;
		}

		~DatabaseConnection() => Dispose(false);

		/// <summary>
		/// Returns a boolean indicating whether the connection is open.
		/// </summary>
		/// <returns>boolean</returns>
		public bool IsConnectionOpen() => this.InternalConnection.State == ConnectionState.Open;

		/// <summary>
		/// Opens the database connection.
		/// </summary>
		public void OpenConnection() => this.InternalConnection.Open();
	
		/// <summary>
		/// Closes the database connection.
		/// </summary>
		public void CloseConnection() => this.InternalConnection.Close();

		/// <summary>
		/// Implements the Dispose pattern to release the database connection back to the pool.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Implements the Dispose pattern to release the database connection back to the pool.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.IsConnectionPooled)
			{
				DatabaseConnectionPool.GetInstance().ReleaseDatabaseConnection(this);
				this.IsConnectionPooled = true;
			}
		}
	}
}
