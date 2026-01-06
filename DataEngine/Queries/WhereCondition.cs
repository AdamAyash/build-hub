namespace BuildHub.DataEngine.Queries
{
	public class WhereCondition
	{
		private Tuple<string, CompareTypes, object?> _internalWhereCondition;

		public WhereCondition(string columnName, CompareTypes compareType, object? value)
		{
			this._internalWhereCondition = new Tuple<string, CompareTypes, object?>( columnName, compareType, value );
		}

		public Tuple<string, CompareTypes, object?> InternalWhereCondition
		{
			get
			{
				return this._internalWhereCondition;
			}
		}
	}
}
