namespace BuildHub.DataEngine.Exceptions.Queries
{
	public class QueryAlreadyBuiltException : Exception
	{
		public QueryAlreadyBuiltException()
			: base("The specified query has already been built.")
		{
		}
	}
}
