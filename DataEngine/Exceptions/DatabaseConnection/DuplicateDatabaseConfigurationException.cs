namespace BuildHub.DataEngine.Exceptions.DatabaseConnection
{
	public sealed class DuplicateDatabaseConfigurationException : Exception
	{
		/// <summary>
		/// Represents an exception that is thrown when a duplicate database configuration is detected.
		/// </summary>
		/// <remarks>This exception is typically used to indicate that an attempt to add or use a database
		/// configuration has failed due to the presence of an existing configuration with the same identifier or
		/// settings.</remarks>
		public DuplicateDatabaseConfigurationException()
			: base("There is a duplicate database configuration")
		{
		}
	}
}
