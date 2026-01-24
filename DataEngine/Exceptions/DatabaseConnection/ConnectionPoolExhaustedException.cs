using BuildHub.DataEngine.DatabaseConnection;

namespace BuildHub.DataEngine.Exceptions.DatabaseConnection
{
	public sealed class ConnectionPoolExhaustedException : Exception
	{
		public ConnectionPoolExhaustedException(DatabaseSource databaseSource)
			: base($"Connection pool exhausted for {databaseSource}.")
		{
		}
	}
}
