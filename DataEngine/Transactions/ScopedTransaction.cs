namespace BuildHub.DataEngine.Transactions
{
	using BuildHub.Common.Logger;
	using BuildHub.Common.Utilities;
	using Microsoft.Data.SqlClient;
	using DatabaseConnection;

	/// <summary>
	/// A transaction wrapper class that will rollback automatically out of scope.
	/// </summary>
	public sealed class ScopedTransaction : ITransactionContext, IDisposable
	{
		private readonly DatabaseContext _databaseContext;
		private readonly DatabaseConnection _databaseConnection;
		private readonly SqlTransaction _internalTransaction;
		private readonly DatabaseSource _databaseSource;
		private bool _isTransactionFinished;
		private bool _isDisposed;

		public SqlTransaction InternalTransaction {  get { return _internalTransaction; } }

		public ScopedTransaction(DatabaseSource databaseSource = DatabaseSource.Core)
		{
			this._databaseContext = DatabaseContext.GetCurrentContext;
			this._databaseContext.TransactionContext = this;
			this._databaseConnection = _databaseContext.GetConnection(databaseSource);
			this._internalTransaction = this.StartTransaction();
			this._databaseSource = databaseSource;
			this._isTransactionFinished = false;
			this._isDisposed = false;

			Logger.LogDebug($"Transaction was successfully started for database {databaseSource}");
		}

		/// <summary>
		/// Starts a new database transaction on the underlying connection.
		/// </summary>
		/// <returns>A <see cref="SqlTransaction"/> object representing the newly started transaction. The caller is responsible for
		/// committing or rolling back the transaction as appropriate.</returns>
		/// <exception cref="ObjectDisposedException">Thrown if the current instance has been disposed.</exception>
		private SqlTransaction StartTransaction()
		{
			if (_isDisposed)
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(ScopedTransaction)));

			return this._databaseConnection.InternalConnection.BeginTransaction();
		}

		/// <summary>
		/// Commits the current transaction and releases associated resources.
		/// </summary>
		/// <remarks>After calling this method, the transaction is finalized and the underlying database context is
		/// disposed.  This method should be called once per transaction scope. Subsequent calls after disposal will result in
		/// an exception.</remarks>
		/// <returns><see langword="true"/> if the transaction is committed successfully; otherwise, <see langword="false"/> if an
		/// error occurs during commit.</returns>
		/// <exception cref="ObjectDisposedException">Thrown if the transaction has already been disposed.</exception>
		public bool Commit()
		{ 
			if (_isDisposed) 
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(ScopedTransaction)));

			try
			{
				this._internalTransaction.Commit();
			}
			catch(Exception exception)
			{
				Logger.LogError(exception, $"Commit transaction failed.");
				return false;
			}
			finally
			{
				this._databaseContext.ClearContext();
			}

			this._isTransactionFinished = true;
			return true;
		}

		/// <summary>
		/// Rolls back the current transaction and releases associated resources.
		/// </summary>
		/// <remarks>After calling this method, the transaction and its underlying resources are disposed and cannot
		/// be used again.</remarks>
		/// <returns><see langword="true"/> if the transaction was successfully rolled back; otherwise, <see langword="false"/>.</returns>
		/// <exception cref="ObjectDisposedException">Thrown if the transaction has already been disposed.</exception>
		public bool Rollback()
		{
			if (_isDisposed)
				throw new ObjectDisposedException(Utilities.GetTypeName(typeof(ScopedTransaction)));

			try
			{
				this._internalTransaction.Rollback();
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Commit transaction failed.");
				return false;
			}
			finally
			{
				this._databaseContext.ClearContext();
			}

			this._isTransactionFinished = true;

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
					if(!this._isTransactionFinished && !this.Rollback())
					{
						throw new InvalidOperationException();
					}

					this._databaseContext.ClearContext();
				}

				_isDisposed = true;
			}
		}
	}
}
