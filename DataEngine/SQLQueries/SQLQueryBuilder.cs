#region
using BuildHub.Common.Logger;
using BuildHub.Common.Utilities;
using BuildHub.DataEngine.Entities;
using BuildHub.DataEngine.Exceptions;
using System.Data;
using System.Text;
#endregion

namespace BuildHub.DataEngine.SQLQueries
{
	using WhereCondition = Tuple<string, CompareTypes, object?>;

	/// <summary>
	/// Provides a builder for constructing SQL SELECT and INSERT queries in a fluent, elegant manner.
	/// </summary>
	/// <remarks>The SQLQueryBuilder enables the creation of parameterized SQL queries by chaining method calls to
	/// specify the table, columns, WHERE conditions, locking behavior, and result limits. It is designed for scenarios
	/// where dynamic query generation is required, such as data access layers or repository implementations. The builder
	/// enforces correct usage by throwing exceptions if a query is built more than once without resetting. This class is
	/// not thread-safe; each instance should be used by a single thread at a time.</remarks>
	public sealed class SQLQueryBuilder : IQueryBuilder
	{
		private readonly List<WhereCondition> _whereStatements = new List<WhereCondition>();

		private string _tableName = string.Empty;
		private string _query = string.Empty;
		private LockTypes _lockType;
		private int _topStatementCount;

		private bool _isQueryBuilt;

		public SQLQueryBuilder() => this.Reset();

		private string ProcessValue(object? value)
		{
			if(value is string)
				return Utilities.Stringify(value);

			if (value is DateTime)
				return Utilities.Stringify(Utilities.FormatDateTime((DateTime)(value)));

			if(value is Guid)
				return Utilities.Stringify(value);

			return value?.ToString() ?? string.Empty;
		}

		private bool ValidateQueryParameters(object? value)
		{
			Type? type = value?.GetType();

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

		/// <summary>
		/// Generate the where statements.
		/// </summary>
		/// <param name="queryStringBuilder"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		private void GenerateWhereStatements(StringBuilder queryStringBuilder)
		{
			if (_whereStatements.Count > 0)
			{
				var whereStatements = new List<string>();
				queryStringBuilder.Append(" WHERE ");

				foreach (var statement in this._whereStatements)
				{
					string completedCondition = string.Empty;
					string columnName = statement.Item1;
					string compareOperator = Utilities.GetEnumDescription<CompareTypes>(statement.Item2);
					object? value = statement?.Item3;

					if (!this.ValidateQueryParameters(value))
					{
						Logger.LogError($"The given query parameter {value} is invalid.");
						throw new ArgumentException();
					}

					completedCondition = $"{columnName} {compareOperator} {this.ProcessValue(value)}";
					whereStatements.Add(completedCondition);
				}

				queryStringBuilder.AppendJoin(" AND ", whereStatements);
			}
		}

		public IQueryBuilder BuildSelect()
		{
			this._query = string.Empty;

			StringBuilder queryStringBuilder = new StringBuilder();
			if(_topStatementCount > -1)
				queryStringBuilder.Append($"SELECT TOP {_topStatementCount} * FROM {this._tableName} " );
			else
				queryStringBuilder.Append($"SELECT * FROM {this._tableName} ");

			queryStringBuilder.Append($"WITH({Utilities.GetEnumDescription<LockTypes>(this._lockType)})");
			this.GenerateWhereStatements(queryStringBuilder);

			_query = queryStringBuilder.ToString().Trim();
			_isQueryBuilt = true;

			return this;
		}

		public IQueryBuilder BuildInsert<Entity>(Entity entity) 
			where Entity : IEntity
		{
			this._query = string.Empty;

			StringBuilder queryStringBuilder = new StringBuilder();
			queryStringBuilder.Append($"INSERT INTO {this._tableName} ");
			queryStringBuilder.Append("(");

			var properties = Utilities.GetObjectProperties<Entity>();
			var columnNames = new List<string>();
			var values = new List<object>();

			foreach (var property in properties)
			{
				if (EntityDataMapper.HasIdentityColumn(property))
					continue;

				columnNames.Add(EntityDataMapper.GetColumnInfo(property).ColumnName);
				values.Add(ProcessValue(property.GetValue(entity)));
			}

			queryStringBuilder.AppendJoin(", ", columnNames);
			queryStringBuilder.Append(") ");
			queryStringBuilder.Append("VALUES (");
			queryStringBuilder.AppendJoin(", ", values);
			queryStringBuilder.Append(")");

			this._query = queryStringBuilder.ToString().Trim();
			this._isQueryBuilt = true;

			return this;
		}
		public IQueryBuilder BuildUpdate<Entity>(Entity entity)
			where Entity : IEntity
		{
			this._query = string.Empty;

			StringBuilder queryStringBuilder = new StringBuilder();
			queryStringBuilder.Append($"UPDATE {this._tableName} ");
			queryStringBuilder.Append("SET ");

			var properties = Utilities.GetObjectProperties<Entity>();
			var updateStatements = new List<string>();

			bool hasPrimaryKeyColumn = false;
			object? primaryKeyValue = null;

			foreach (var property in properties)
			{
				if (EntityDataMapper.HasIdentityColumn(property))
				{
					continue;
				}

				if (EntityDataMapper.HasPrimaryKeyColumn(property))
				{
					hasPrimaryKeyColumn = true;
					primaryKeyValue = property.GetValue(entity);
					continue;
				}

				var columnName = EntityDataMapper.GetColumnInfo(property).ColumnName;
				var value = ProcessValue(property.GetValue(entity));
				updateStatements.Add(columnName +  " = " + value);
			}

			queryStringBuilder.AppendJoin(", ", updateStatements);

			if(!hasPrimaryKeyColumn)
				throw new MissingPrimaryKeyException();

			Where(EntityDataMapper.GetPrimaryKeyMappingData<Entity>().ColumnInfo.ColumnName, primaryKeyValue);
			this.GenerateWhereStatements(queryStringBuilder);

			this._query = queryStringBuilder.ToString().Trim();
			this._isQueryBuilt = true;

			return this;
		}

		public IQueryBuilder BuildDelete<Entity>(Entity entity)
			where Entity : IEntity
		{
			this._query = string.Empty;

			StringBuilder queryStringBuilder = new StringBuilder();
			queryStringBuilder.Append($"DELETE FROM {this._tableName} ");

			ColumnMappingData columnMappingData = EntityDataMapper.GetPrimaryKeyMappingData<Entity>();

			Where(columnMappingData.ColumnInfo.ColumnName, columnMappingData.PropertyInfo.GetValue(entity));
			this.GenerateWhereStatements(queryStringBuilder);

			this._query = queryStringBuilder.ToString().Trim();
			this._isQueryBuilt = true;

			return this;
		}

		public IQueryBuilder Reset()
		{
			this._tableName = string.Empty;
			this._whereStatements.Clear();
			this._lockType = LockTypes.None;
			this._query = string.Empty;
			this._topStatementCount = -1;
			this._isQueryBuilt = false;

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

		public IQueryBuilder Where(string columnName, CompareTypes compareType, object? value)
		{
			this._whereStatements.Add(new WhereCondition(columnName.ToUpper(), compareType, value));
			return this; 
		}

		public IQueryBuilder Where(string columnName, object? value)
		{
			this._whereStatements.Add(new WhereCondition(columnName, CompareTypes.Equal, value));
			return this;
		}

		public IQueryBuilder Lock(LockTypes lockType)
		{
			this._lockType = lockType;
			return this;
		}

		public string GetQuery()
		{
			if (!_isQueryBuilt)
			{
				Logger.LogError("Trying to use an non built query");
				throw new NotBuiltQueryException();
			}

			Logger.LogDebug($"Table '{this._tableName}' generated a query '{this._query}'");
			return this._query;
		}
	}
}
