using Microsoft.Data.SqlClient;
using System.Data;

namespace BuildHub.DataEngine.DatabaseConnection
{
	/// <summary>
	/// A virtual proxy to the SqlConnection class
	/// </summary>
	public class DatabaseConnection
	{
		public DatabaseConnection(DatabaseSource databaseSource, string connectionString)
		{
			this.InternalConnection = new SqlConnection(connectionString);
			this.DatabaseSource = databaseSource;
		}

		/// <summary>
		/// 
		/// </summary>
		public SqlConnection InternalConnection { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public DatabaseSource DatabaseSource { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool IsConnectionOpen() => this.InternalConnection.State == ConnectionState.Open;

		/// <summary>
		/// 
		/// </summary>
		public void OpenConnection() => this.InternalConnection.Open();

		/// <summary>
		/// 
		/// </summary>
		public void CloseConnection() => this.InternalConnection.Close();
	}
}
