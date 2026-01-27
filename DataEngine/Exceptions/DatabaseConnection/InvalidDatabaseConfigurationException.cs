namespace BuildHub.DataEngine.Exceptions.DatabaseConnection
{
    internal class InvalidDatabaseConfigurationException : Exception
    {
        public InvalidDatabaseConfigurationException(string message)
            : base(message)
        {
        }
    }
}
