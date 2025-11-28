namespace BuildHub.DataEngine.Exceptions
{
	/// <summary>
	/// Exception describing the scenario where a database configuration is missing.
	/// </summary>
	public sealed class MissingDatabaseConfigurationException : Exception
	{
		public MissingDatabaseConfigurationException()
			: base("Database configuration is missing.")
		{

		}
	}
}
