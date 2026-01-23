namespace BuildHub.DataEngine.Queries
{
	#region
	using BuildHub.Common.Logger;
	using BuildHub.Common.Utilities;
	using BuildHub.DataEngine.Exceptions.Queries;
	using BuildHub.DataEngine.Queries.Base;
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

		/// <summary>
		/// Performs a validation to a query parameter
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Resets the state of the query builder.
		/// </summary>
		/// <returns></returns>
		public InternalQueryBuilder Reset()
		{
			this._queryBuilderState.Reset();
			this._isQueryBuilt = false;
			this._query = string.Empty;

			return this;
		}

		/// <summary>
		///	Creates a top clause
		/// </summary>
		/// <param name="count">count to return</param>
		/// <returns></returns>
		public InternalQueryBuilder Top(int count)
		{
			this._queryBuilderState.TopStatementCount = count;
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="compareType"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public InternalQueryBuilder Where(string columnName, CompareTypes compareType, object? value)
		{
			this._queryBuilderState.WhereStatements.Add(new WhereCondition(columnName.ToUpper(), compareType, value));
			return this;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public InternalQueryBuilder Where(string columnName, object? value)
		{
			this._queryBuilderState.WhereStatements.Add(new WhereCondition(columnName, CompareTypes.Equal, value));
			return this;
		}

		/// <summary>
		/// Adds a WHERE condition to the query using the specified entity property, comparison type, and value.
		/// </summary>
		/// <remarks>This method enables building queries by specifying conditions based on entity properties.
		/// Multiple calls to <c>Where</c> will add additional conditions to the query.</remarks>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity">The entity instance whose property value will be used in the condition. Cannot be <see langword="null"/>.</param>
		/// <param name="condition">An expression that specifies the property of <typeparamref name="TEntity"/> to compare. Cannot be <see
		/// langword="null"/>.</param>
		/// <param name="compareType">The comparison operator to use for the condition, such as equal, greater than, or less than.</param>
		/// <returns>The current <see cref="InternalQueryBuilder"/> instance with the added WHERE condition, allowing for method
		/// chaining.</returns>
		public InternalQueryBuilder Where<TEntity>(TEntity entity, System.Linq.Expressions.Expression<Func<TEntity, object>> condition, CompareTypes compareType)
			where TEntity : IEntity
		{
			var columnInfo = EntityDataMapper.GetColumnInfo<TEntity>(condition);
			var value = condition.Compile()(entity);

			this._queryBuilderState.WhereStatements.Add(new WhereCondition(columnInfo.ColumnName, CompareTypes.Equal, value));
			return this;
		}

		/// <summary>
		/// Specifies the locking behavior for the query by setting the lock type.
		/// </summary>
		/// <param name="lockType">The type of lock to apply to the query. Determines how concurrent access to the queried data is managed.</param>
		/// <returns>The current <see cref="InternalQueryBuilder"/> instance with the specified lock type applied, enabling method
		/// chaining.</returns>
		public InternalQueryBuilder Lock(LockTypes lockType)
		{
			this._queryBuilderState.LockType = lockType;
			return this;
		}

		/// <summary>
		/// Builds a SQL SELECT query based on the current query builder state and configuration.
		/// </summary>
		/// <remarks>This method constructs the SELECT statement using the specified table name, optional TOP clause,
		/// locking options, and any configured WHERE conditions.  Subsequent calls will return the same query unless the
		/// builder state is modified.</remarks>
		/// <returns>The current <see cref="InternalQueryBuilder"/> instance with the SELECT query constructed and ready for execution
		/// or further modification.</returns>
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

		/// <summary>
		/// Builds an SQL INSERT query for the specified entity and prepares it for execution.
		/// </summary>
		/// <remarks>Identity columns are automatically excluded from the INSERT statement. The method can only be
		/// called once per query builder instance; subsequent calls will not rebuild the query.</remarks>
		/// <typeparam name="Entity">The type of the entity to insert. Must implement <see cref="IEntity"/>.</typeparam>
		/// <param name="entity">The entity instance containing the data to be inserted into the database. All non-identity properties of the
		/// entity are included in the query.</param>
		/// <returns>The current <see cref="InternalQueryBuilder"/> instance with the INSERT query constructed and ready for execution.</returns>
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

		/// <summary>
		/// Builds an SQL UPDATE query for the specified entity, setting column values based on the entity's properties.
		/// </summary>
		/// <remarks>The generated UPDATE query sets all non-identity, non-primary key columns to the corresponding
		/// values from <paramref name="entity"/>. The WHERE clause is automatically constructed using the entity's primary
		/// key value. This method should be called once per query build cycle; repeated calls will not rebuild the
		/// query.</remarks>
		/// <typeparam name="Entity">The type of the entity, which must implement <see cref="IEntity"/>.</typeparam>
		/// <param name="entity">The entity instance containing the property values to be updated in the database. Must have a defined primary key.</param>
		/// <returns>An <see cref="InternalQueryBuilder"/> instance representing the constructed UPDATE query.</returns>
		/// <exception cref="MissingPrimaryKeyException">Thrown if the entity type does not define a primary key property.</exception>
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

		/// <summary>
		/// Builds a SQL DELETE query targeting the specified entity's primary key.
		/// </summary>
		/// <remarks>The generated query deletes a single row from the table associated with the entity type, using
		/// the entity's primary key value in the WHERE clause. Subsequent calls to this method on the same <see
		/// cref="InternalQueryBuilder"/> instance will not rebuild the query unless the internal state is reset.</remarks>
		/// <typeparam name="Entity">The type of the entity, which must implement <see cref="IEntity"/>.</typeparam>
		/// <param name="entity">The entity instance to delete. The primary key value of this entity is used to identify the row to remove from the
		/// database table.</param>
		/// <returns>An <see cref="InternalQueryBuilder"/> instance containing the constructed DELETE query.</returns>
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

		/// <summary>
		/// Specifies the table to use as the source for the query.
		/// </summary>
		/// <remarks>The table name is converted to uppercase. This method is typically used as part of a fluent API
		/// to build queries.</remarks>
		/// <param name="tableName">The name of the table to query. Cannot be <c>null</c> or empty.</param>
		/// <returns>The current <see cref="InternalQueryBuilder"/> instance, allowing for method chaining.</returns>
		public InternalQueryBuilder From(string tableName)
		{
			this._tableName = tableName.ToUpper();
			return this;
		}

		/// <summary>
		/// Returns the SQL query string that was previously built for the current table.
		/// </summary>
		/// <remarks>This method should be called only after the query has been successfully constructed. Attempting
		/// to retrieve the query before it is built will result in an exception.</remarks>
		/// <returns>The SQL query string associated with the current table.</returns>
		/// <exception cref="NotBuiltQueryException">Thrown if the query has not been built prior to calling this method.</exception>
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
