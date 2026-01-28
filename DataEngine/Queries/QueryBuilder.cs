using BuildHub.DataEngine.Entities;
using BuildHub.DataEngine.Queries.Base;
using System.Linq.Expressions;

namespace BuildHub.DataEngine.Queries
{
	/// <summary>
	/// Provides a builder for constructing SQL query statements with support for WHERE conditions, row limits, and locking
	/// options.
	/// </summary>
	/// <remarks><para> The <see cref="QueryBuilder"/> class enables the fluent construction of SQL queries by
	/// allowing callers to specify WHERE clauses, set a maximum number of rows to return, and apply locking hints. Methods
	/// can be chained to incrementally build up a query definition. </para> <para> This class is intended for use in
	/// scenarios where dynamic SQL query generation is required. It implements the <see cref="IQueryBuilder"/> interface.
	/// </para></remarks>
	public sealed class QueryBuilder : IQueryBuilder<QueryBuilder>
	{
		/// <summary>
		/// Query builder's internal state.
		/// </summary>
		public QueryBuilderState QueryBuilderState { get; set; }

		public QueryBuilder()
		{
			this.QueryBuilderState = new QueryBuilderState();
			this.Reset();
		}

        /// <summary>
        /// Resets the state of the query builder.
        /// </summary>
        /// <returns>Reference to the builder</returns>
        public QueryBuilder Reset()
		{
			QueryBuilderState.Reset();
			return this;
		}

		public QueryBuilder Top(int count)
		{
			this.QueryBuilderState.TopStatementCount = count;
			return this;
		}

        /// <summary>
        ///	Generates a where statement by providing a column and value.
        /// </summary>
        /// <param name="columnName">Name of the column</param>
		/// <param name="compareType">Cmpare type </param>
        /// <param name="value"></param>
        /// <returns>Returns a reference to the query builder</returns>
        public QueryBuilder Where(string columnName, CompareTypes compareType, object? value)
		{
			this.QueryBuilderState.WhereStatements.Add(new WhereCondition(columnName.ToUpper(), compareType, value));
			return this;
		}

        /// <summary>
        ///	Generates a where statement by providing a column and value.
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="value"></param>
        /// <returns>Returns a reference to the query builder</returns>
        public QueryBuilder Where(string columnName, object? value)
		{
			this.QueryBuilderState.WhereStatements.Add(new WhereCondition(columnName, CompareTypes.Equal, value));
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
		public QueryBuilder Where<TEntity>(TEntity entity, Expression<Func<TEntity, object>> condition, CompareTypes compareType = CompareTypes.Equal)
			where TEntity : IEntity
		{
			var columnInfo = EntityDataMapper.GetColumnInfo<TEntity>(condition);
			var value = condition.Compile()(entity);

			this.QueryBuilderState.WhereStatements.Add(new WhereCondition(columnInfo.ColumnName, compareType, value));
			return this;
		}

		/// <summary>
		/// Specifies the locking behavior for the query by setting the lock type.
		/// </summary>
		/// <param name="lockType">The type of lock to apply to the query. Determines how concurrent access to the queried data is managed.</param>
		/// <returns>The current <see cref="InternalQueryBuilder"/> instance with the specified lock type applied, enabling method
		/// chaining.</returns>
		public QueryBuilder Lock(LockTypes lockType)
		{
			this.QueryBuilderState.LockType = lockType;
			return this;
		}
	}
}
