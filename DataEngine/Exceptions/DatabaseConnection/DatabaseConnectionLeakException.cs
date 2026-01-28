namespace BuildHub.DataEngine.Exceptions.DatabaseConnection
{
    /// <summary>
    /// Exception class indicating a conenction has been leaked.
    /// </summary>
    internal class DatabaseConnectionLeakException : Exception
    {
        public DatabaseConnectionLeakException()
            : base("")
        {
        }
    }
}
