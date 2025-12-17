namespace BuildHub.DataEngine.Exceptions
{
	public class QueryAlreadyBuiltException : Exception
	{
		public QueryAlreadyBuiltException()
			: base("The specified qury has already been built.")
		{
		}
	}
}
