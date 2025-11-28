namespace BuildHub.DataEngine.SQLQueries
{
	public interface IQueryBuilder
	{
		IQueryBuilder BuildSelect();

		IQueryBuilder Reset();

		IQueryBuilder From(string tableName);

		IQueryBuilder Where<ValueType>(string columnName, CompareTypes compareType, ValueType value)
			where ValueType : struct;

		IQueryBuilder Lock(LockTypes lockType);

		string Query { get; }
	}
}
