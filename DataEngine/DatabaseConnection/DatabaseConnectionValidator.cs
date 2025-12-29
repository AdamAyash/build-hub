using BuildHub.Common.Logger;
using BuildHub.DataEngine.Transactions;
using Microsoft.Data.SqlClient;

namespace BuildHub.DataEngine.DatabaseConnection
{
	/// <summary>
	/// A class aiming to test the database connection before use
	/// </summary>
	public sealed class DatabaseConnectionValidator
	{
		/// <summary>
		/// Test query constant
		/// </summary>
		private const string _TEST_SQL_QUERY = "SELECT 1";

		/// <summary>
		/// Database connection member
		/// </summary>
		private readonly DatabaseConnection _databaseConnection;
		private readonly ITransactionContext? _transactionContext;

		public DatabaseConnectionValidator(DatabaseConnection databaseConnection, 
			ITransactionContext? transactionContext = null)
		{
			this._databaseConnection = databaseConnection;
			this._transactionContext = transactionContext;
		}

		/// <summary>
		/// Tests the database connection by performing a simple select statement
		/// </summary>
		/// <returns>bool</returns>
		public bool TestDatabaseConnection()
		{
			bool isSuccessful = false;

			if (!this._databaseConnection.IsConnectionOpen())
				return isSuccessful;

			try
			{
				var testQuery = new SqlCommand(_TEST_SQL_QUERY, _databaseConnection.InternalConnection);
				testQuery.Transaction = _transactionContext?.InternalTransaction;

				isSuccessful = Convert.ToInt32(testQuery.ExecuteScalar()) == 1;
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, "An error occurred while trying to execute a test query.");
			}

			return isSuccessful;
		}
	}
}
