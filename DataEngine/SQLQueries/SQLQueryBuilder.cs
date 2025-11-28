using BuildHub.Common.Utilities;
using System.Text;

namespace BuildHub.DataEngine.SQLQueries
{
	using WhereCondition = Tuple<string, CompareTypes, object>;

	public sealed class SQLQueryBuilder : IQueryBuilder
	{
		private readonly List<WhereCondition> _whereStatements;

		private string _tableName;
		private LockTypes _lockType;

		public string Query { get; private set; }

		public SQLQueryBuilder()
		{
			this._tableName = string.Empty;
			this._whereStatements = new List<WhereCondition>();
			this._lockType = LockTypes.None;
			this.Query = string.Empty;
		}

		private string ProcessValue(object value)
		{
			if(value is string)
				return Utilities.Stringify(value);

			if (value is DateTime)
				return Utilities.Stringify(Utilities.FormatDateTime((DateTime)(value)));

			return value.ToString() ?? string.Empty;
		}

		public IQueryBuilder BuildSelect()
		{
			StringBuilder queryStringBuilder = new StringBuilder();
			queryStringBuilder.Append($"SELECT * FROM {this._tableName} ");
			queryStringBuilder.Append($"WITH({Utilities.GetEnumDescription<LockTypes>(this._lockType)}) ");

			if(_whereStatements.Count > 0)
			{
				queryStringBuilder.Append(" WHERE ");
				var whereStatements = new List<string>();

				foreach(var statement in this._whereStatements)
				{
					string condition = string.Empty;

					string columnName = statement.Item1;
					CompareTypes compareType = statement.Item2;
					object value = statement.Item3;

					string compareOperator = Utilities.GetEnumDescription<CompareTypes>(compareType);

					condition = $"{columnName} {compareOperator} {this.ProcessValue(value)}";

					whereStatements.Add(condition);
				}

				queryStringBuilder.Append(string.Join(" AND ", whereStatements));
			}

			Query = queryStringBuilder.ToString().Trim();

			return this;
		}

		public IQueryBuilder Reset()
		{
			this._tableName = string.Empty;
			this._whereStatements.Clear();
			this._lockType = LockTypes.None;
			this.Query = string.Empty;

			return this;
		}

		public IQueryBuilder From(string tableName)
		{
			this._tableName = tableName;
			return this;
		}

		public IQueryBuilder Where<ValueType>(string columnName, CompareTypes compareType, ValueType value)
			where ValueType : struct
		{
			this._whereStatements.Add(new Tuple<string, CompareTypes, object>(columnName, compareType, value));
			return this; 
		}

		public IQueryBuilder Lock(LockTypes lockType)
		{
			this._lockType = lockType;
			return this;
		}
	}
}
