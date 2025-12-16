using BuildHub.Common.Logger;
using BuildHub.Common.Utilities;
using System.Text;

namespace BuildHub.DataEngine.SQLQueries
{
	using WhereCondition = Tuple<string, CompareTypes, object>;
	public sealed class SQLQueryBuilder : IQueryBuilder
	{
		private readonly List<WhereCondition> _whereStatements = new List<WhereCondition>();

		private string _tableName;
		private string _buildedQuery;
		private LockTypes _lockType;
		private int _topStatementCount;

		public SQLQueryBuilder() => this.Reset();

		private string ProcessValue(object value)
		{
			if(value is string)
				return Utilities.Stringify(value);

			if (value is DateTime)
				return Utilities.Stringify(Utilities.FormatDateTime((DateTime)(value)));

			if(value is Guid)
				return Utilities.Stringify(value);

			return value.ToString() ?? string.Empty;
		}

		private bool ValidateQueryParameters(object value)
		{
			Type type = value.GetType();

			if (type != typeof(Int16)		&&
				type != typeof(Int32) 		&&
				type != typeof(Int64) 		&&
				type != typeof(Double)		&&
				type != typeof(String)		&&
				type != typeof(DateTime)	&&
				type != typeof(Guid))
			{
				return false;
			}

			return true;
		}

		public IQueryBuilder BuildSelect()
		{
			StringBuilder queryStringBuilder = new StringBuilder();
			if(_topStatementCount > -1)
				queryStringBuilder.Append($"SELECT TOP {_topStatementCount} * FROM {this._tableName}" );
			else
				queryStringBuilder.Append($"SELECT * FROM {this._tableName} ");

			queryStringBuilder.Append($"WITH({Utilities.GetEnumDescription<LockTypes>(this._lockType)})");

			if(_whereStatements.Count > 0)
			{
				queryStringBuilder.Append(" WHERE ");
				var whereStatements = new List<string>();

				foreach(var statement in this._whereStatements)
				{
					string completedCondition = string.Empty;
					string columnName = statement.Item1;
					string compareOperator = Utilities.GetEnumDescription<CompareTypes>(statement.Item2);
					object value = statement.Item3;

					if (!this.ValidateQueryParameters(value))
					{
						Logger.LogError($"The given query parameter {value} is invalid.");
						throw new ArgumentException();
					}

					completedCondition = $"{columnName} {compareOperator} {this.ProcessValue(value)}";
					whereStatements.Add(completedCondition);
				}

				queryStringBuilder.Append(string.Join(" AND ", whereStatements));
			}

			_buildedQuery = queryStringBuilder.ToString().Trim();

			return this;
		}

		public IQueryBuilder Reset()
		{
			this._tableName = string.Empty;
			this._whereStatements.Clear();
			this._lockType = LockTypes.None;
			this._buildedQuery = string.Empty;
			this._topStatementCount = -1;

			return this;
		}

		public IQueryBuilder From(string tableName)
		{
			this._tableName = tableName.ToUpper();
			return this;
		}

		public IQueryBuilder Top(int count)
		{
			this._topStatementCount = count;
			return this;
		}

		public IQueryBuilder Where(string columnName, CompareTypes compareType, object value)
		{
			this._whereStatements.Add(new WhereCondition(columnName.ToUpper(), compareType, value));
			return this; 
		}

		public IQueryBuilder Where(string columnName, object value)
		{
			this._whereStatements.Add(new WhereCondition(columnName, CompareTypes.Equal, value));
			return this;
		}

		public IQueryBuilder Lock(LockTypes lockType)
		{
			this._lockType = lockType;
			return this;
		}

		public override string ToString()
		{
			return this._buildedQuery;
		}
	}
}
