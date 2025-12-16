namespace BuildHub.DataEngine.Transactions
{
	internal interface ITransactionContext
	{
		bool Commit();
		bool Rollback();
	}
}
