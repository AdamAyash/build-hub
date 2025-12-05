namespace BuildHub.DataEngine.Exceptions
{
	public class DatabaseConnectionValidationException : Exception
	{
		public DatabaseConnectionValidationException(DatabaseSource databaseSource)
			: base($"An error occurred while trying to validate the database connection for {databaseSource}")
		{
		}
	}
}
