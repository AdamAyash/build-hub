namespace BuildHub.DataEngine.Exceptions.DatabaseConnection
{
	/// <summary>
	/// Exception class for the scenario where an empty connection string isn't provided.
	/// </summary>
	public sealed class EmptyConnectionStringException : Exception
	{
		public EmptyConnectionStringException(DatabaseSource databaseSource)
			: base($"The provided connection string for {databaseSource} is empty")
		{
		}
	}
}
