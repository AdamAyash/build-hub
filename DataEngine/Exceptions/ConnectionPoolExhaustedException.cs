namespace BuildHub.DataEngine.Exceptions
{
	public sealed class ConnectionPoolExhaustedException : Exception
	{
		public ConnectionPoolExhaustedException(DatabaseSource databaseSource)
			: base($"Connection pool exhausted for {databaseSource}.")
		{
		}
	}
}
