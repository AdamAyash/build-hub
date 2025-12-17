using Microsoft.Data.SqlClient;

namespace BuildHub.DataEngine.Transactions
{
	/// <summary>
	/// Interface for context transactions
	/// </summary>
	public interface ITransactionContext
	{
		public SqlTransaction InternalTransaction { get;}

		/// <summary>
		/// Commits the transaction. 
		/// </summary>
		/// <returns>Whether the transaction was committed.</returns>
		bool Commit();

		/// <summary>
		/// Rollback the transaction.
		/// </summary>
		/// <returns>Whether the transaction has been rolled back.</returns>
		bool Rollback();
	}
}
