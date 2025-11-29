namespace BuildHub.DataEngine.SQLQueries
{
	public interface IQueryBuilder
	{
		IQueryBuilder BuildSelect();

		IQueryBuilder Reset();

		IQueryBuilder From(string tableName);

		IQueryBuilder Where(string columnName, CompareTypes compareType, object value);

		IQueryBuilder Lock(LockTypes lockType);

		string Query { get; }
	}
}
