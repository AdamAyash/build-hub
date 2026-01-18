namespace BuildHub.DataEngine.Queries
{
	#region
	using BuildHub.Common.Logger;
	using BuildHub.Common.Utilities;
	using BuildHub.DataEngine.Exceptions.Queries;
	using Entities;
	using System.Data;
	using System.Text;
	#endregion

	/// <summary>
	/// Provides internal functionality for building SQL queries, including SELECT, INSERT, UPDATE, and DELETE statements,
	/// with support for entity mapping and query composition.
	/// </summary>
	/// <remarks><para> <b>InternalSQLQueryBuilder</b> is intended for advanced scenarios where direct control over
	/// SQL query generation is required. It extends <see cref="QueryBuilder"/> and implements <see
	/// cref="IInternalQueryBuilder"/>, offering methods to construct queries for entities that implement <see
	/// cref="IEntity"/>. </para> <para> This class is not intended for public use and may change without notice. It
	/// supports chaining methods for fluent query construction and enforces validation of query parameters and entity
	/// mappings. </para> <para> Thread Safety: Instances of <b>InternalSQLQueryBuilder</b> are not guaranteed to be
	/// thread-safe. Each instance should be used by a single thread at a time. </para></remarks>
	internal class InternalQueryBuilder : IInternalQueryBuilder<InternalQueryBuilder>
	{
		private string _query = string.Empty;
		private string _tableName = string.Empty;
		private bool _isQueryBuilt;
		private QueryBuilderState _queryBuilderState;

		public InternalQueryBuilder(QueryBuilder queryBuilder)
		{
			this._queryBuilderState = queryBuilder.QueryBuilderState;
		}

		public InternalQueryBuilder()
		{
			this._queryBuilderState = new QueryBuilderState();
			this.Reset();
		}

		private string ProcessValue(object? value)
		{
			if (value is string)
				return Utilities.Stringify(value);

			if (value is DateTime)
				return Utilities.Stringify(Utilities.FormatDateTime((DateTime)(value)));

			if (value is Guid)
				return Utilities.Stringify(value);

			return value?.ToString() ?? string.Empty;
		}

		private bool ValidateQueryParameters(object? value)
		{
			Type? type = value?.GetType();

			if (type != typeof(Int16) &&
				type != typeof(Int32) &&
				type != typeof(Int64) &&
				type != typeof(Double) &&
				type != typeof(String) &&
				type != typeof(DateTime) &&
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
			if (this._queryBuilderState.WhereStatements.Count > 0)
			{
				var whereStatements = new List<string>();
				queryStringBuilder.Append(" WHERE ");

				foreach (var statement in this._queryBuilderState.WhereStatements)
				{
					string completedCondition = string.Empty;
					string columnName = statement.InternalWhereCondition.Item1;
					string compareOperator = Utilities.GetEnumDescription<CompareTypes>(statement.InternalWhereCondition.Item2);
					object? value = statement?.InternalWhereCondition.Item3;

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

		public InternalQueryBuilder Reset()
		{
			this._queryBuilderState.Reset();
			this._isQueryBuilt = false;
			this._query = string.Empty;

			return this;
		}

		public InternalQueryBuilder Top(int count)
		{
			this._queryBuilderState.TopStatementCount = count;
			return this;
		}

		public InternalQueryBuilder Where(string columnName, CompareTypes compareType, object? value)
		{
			this._queryBuilderState.WhereStatements.Add(new WhereCondition(columnName.ToUpper(), compareType, value));
			return this;
		}

		public InternalQueryBuilder Where(string columnName, object? value)
		{
			this._queryBuilderState.WhereStatements.Add(new WhereCondition(columnName, CompareTypes.Equal, value));
			return this;
		}

		public InternalQueryBuilder Where<TEntity>(TEntity entity, System.Linq.Expressions.Expression<Func<TEntity, object>> condition, CompareTypes compareType)
			where TEntity : IEntity
		{
			var columnInfo = EntityDataMapper.GetColumnInfo<TEntity>(condition);
			var value = condition.Compile()(entity);

			this._queryBuilderState.WhereStatements.Add(new WhereCondition(columnInfo.ColumnName, CompareTypes.Equal, value));
			return this;
		}

		public InternalQueryBuilder Lock(LockTypes lockType)
		{
			this._queryBuilderState.LockType = lockType;
			return this;
		}

		public InternalQueryBuilder BuildSelect()
		{
			if (!this._isQueryBuilt)
			{
				this._query = string.Empty;
				int topStatementCount = this._queryBuilderState.TopStatementCount;

				StringBuilder queryStringBuilder = new StringBuilder();
				if (topStatementCount > -1)
					queryStringBuilder.Append($"SELECT TOP {topStatementCount} * FROM {this._tableName} ");
				else
					queryStringBuilder.Append($"SELECT * FROM {this._tableName} ");

				queryStringBuilder.Append($"WITH({Utilities.GetEnumDescription<LockTypes>(this._queryBuilderState.LockType)})");
				this.GenerateWhereStatements(queryStringBuilder);

				_query = queryStringBuilder.ToString().Trim();
				_isQueryBuilt = true;
			}

			return this;
		}

		public InternalQueryBuilder BuildInsert<Entity>(Entity entity)
			where Entity : IEntity
		{
			if (!this._isQueryBuilt)
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
			}

			return this;
		}

		public InternalQueryBuilder BuildUpdate<Entity>(Entity entity)
			where Entity : IEntity
		{
			if (!this._isQueryBuilt)
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
					updateStatements.Add(columnName + " = " + value);
				}

				queryStringBuilder.AppendJoin(", ", updateStatements);

				if (!hasPrimaryKeyColumn)
					throw new MissingPrimaryKeyException();

				Where(EntityDataMapper.GetPrimaryKeyMappingData<Entity>().ColumnInfo.ColumnName, primaryKeyValue);
				this.GenerateWhereStatements(queryStringBuilder);

				this._query = queryStringBuilder.ToString().Trim();
				this._isQueryBuilt = true;
			}

			return this;
		}

		public InternalQueryBuilder BuildDelete<Entity>(Entity entity)
			where Entity : IEntity
		{
			if (!this._isQueryBuilt)
			{
				this._query = string.Empty;

				StringBuilder queryStringBuilder = new StringBuilder();
				queryStringBuilder.Append($"DELETE FROM {this._tableName}");

				ColumnMappingData columnMappingData = EntityDataMapper.GetPrimaryKeyMappingData<Entity>();

				Where(columnMappingData.ColumnInfo.ColumnName, columnMappingData.PropertyInfo.GetValue(entity));
				this.GenerateWhereStatements(queryStringBuilder);

				this._query = queryStringBuilder.ToString().Trim();
				this._isQueryBuilt = true;
			}

			return this;
		}

		public InternalQueryBuilder From(string tableName)
		{
			this._tableName = tableName.ToUpper();
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
