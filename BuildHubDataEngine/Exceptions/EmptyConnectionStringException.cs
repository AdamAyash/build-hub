namespace BuildHubDataEngine.Exceptions
{
    public sealed class EmptyConnectionStringException : Exception
    {
        public EmptyConnectionStringException()
            : base("The provided connection string is empty")
        {
        }
    }
}
