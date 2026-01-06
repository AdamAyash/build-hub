using BuildHub.DataEngine.Entities;
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
		public QueryBuilderState QueryBuilderState { get; set; }

		public QueryBuilder()
		{
			this.QueryBuilderState = new QueryBuilderState();
			this.Reset();
		}

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

		public QueryBuilder Where(string columnName, CompareTypes compareType, object? value)
		{
			this.QueryBuilderState.WhereStatements.Add(new WhereCondition(columnName.ToUpper(), compareType, value));
			return this;
		}

		public QueryBuilder Where(string columnName, object? value)
		{
			this.QueryBuilderState.WhereStatements.Add(new WhereCondition(columnName, CompareTypes.Equal, value));
			return this;
		}

		public QueryBuilder Where<TEntity>(TEntity entity, Expression<Func<TEntity, object>> condition, CompareTypes compareType = CompareTypes.Equal)
			where TEntity : IEntity
		{
			var columnInfo = EntityDataMapper.GetColumnInfo<TEntity>(condition);
			var value = condition.Compile()(entity);

			this.QueryBuilderState.WhereStatements.Add(new WhereCondition(columnInfo.ColumnName, compareType, value));
			return this;
		}

		public QueryBuilder Lock(LockTypes lockType)
		{
			this.QueryBuilderState.LockType = lockType;
			return this;
		}
	}
}
