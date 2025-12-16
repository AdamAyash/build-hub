namespace BuildHub.DataEngine.Transactions
{
	using BuildHub.Common.Logger;
	using DatabaseConnection;
	using Microsoft.Data.SqlClient;

	/// <summary>
	/// 
	/// </summary>
	public sealed class ScopedTransaction : ITransactionContext, IDisposable
	{
		private readonly DatabaseConnectionContext _databaseConnectionContext;
		private readonly DatabaseConnection _databaseConnection;
		private readonly SqlTransaction _internalTransaction;
		private readonly DatabaseSource _databaseSource;
		private bool _isDisposed;

		public ScopedTransaction(DatabaseSource databaseSource = DatabaseSource.Core)
		{
			this._databaseConnectionContext = DatabaseConnectionContext.GetCurrentContext;
			this._databaseConnection = _databaseConnectionContext.GetConnection(databaseSource);
			this._internalTransaction = this.StartTransaction();
			this._databaseSource = databaseSource;
			this._isDisposed = false;

			Logger.LogDebug($"Transaction was successfully started for database {databaseSource}");
		}
		private SqlTransaction StartTransaction()
		{
			return this._databaseConnection.InternalConnection.BeginTransaction();
		}

		public bool Commit()
		{
			try
			{
				this._internalTransaction.Commit();
			}
			catch(Exception exception)
			{
				Logger.LogError(exception, $"Commit transaction failed.");
				return false;
			}

			return true;
		}

		public bool Rollback()
		{
			try
			{
				this._internalTransaction.Rollback();
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Commit transaction failed.");
				return false;
			}

			return true;
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
				{
					if(!this.Rollback())
					{
						//TODO throw
					}
					this._databaseConnectionContext.Dispose();
				}

				_isDisposed = true;
			}
		}
	}
}
