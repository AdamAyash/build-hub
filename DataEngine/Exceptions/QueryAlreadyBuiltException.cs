namespace BuildHub.DataEngine.Exceptions
{
	public class QueryAlreadyBuiltException : Exception
	{
		public QueryAlreadyBuiltException()
			: base("The specified query has already been built.")
		{
		}
	}
}
