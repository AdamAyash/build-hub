using BuildHub.DataEngine.DatabaseConnection;

namespace BuildHub.DataEngine.Exceptions.DatabaseConnection
{
	public class DatabaseConnectionValidationException : Exception
	{
		public DatabaseConnectionValidationException(DatabaseSource databaseSource)
			: base($"An error occurred while trying to validate the database connection for {databaseSource}")
		{
		}
	}
}
