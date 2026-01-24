using System.Linq.Expressions;

namespace BuildHub.DataEngine.Queries.Base
{
	using Entities;

	/// <summary>
	/// Defines an interface for building SQL queries in a fluent and elegant manner.
	/// </summary>
	/// <remarks>This interface provides methods to construct SQL queries step-by-step, allowing for flexibility 
	/// and readability in query generation. It supports specifying the SELECT clause, FROM clause,  WHERE conditions, and
	/// locking mechanisms, as well as resetting the query builder to its initial state.</remarks>
	public interface IQueryBuilder<TQueryBuilder>
	{
		/// <summary>
		/// Resets the query builder to its initial state, clearing any previously applied configurations.
		/// </summary>
		/// <returns>The current instance of the query builder, allowing for method chaining.</returns>
		TQueryBuilder Reset();

		/// <summary>
		/// Add a top clause in the select statement
		/// </summary>
		/// <param name="count"></param>
		/// <returns></returns>
		TQueryBuilder Top(int count);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="compareType"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		///
		TQueryBuilder Where(string columnName, CompareTypes compareType, object? value);

		/// <summary>
		/// Adds a condition to the query that filters results based on the specified column and value.
		/// </summary>
		/// <param name="columnName">The name of the column to apply the condition to. Cannot be null or empty.</param>
		/// <param name="value">The value to compare against the specified column. Typically used for equality checks.</param>
		/// <returns>An instance of <see cref="IQueryBuilder"/> with the condition applied, allowing for further query customization.</returns>
		TQueryBuilder Where(string columnName, object? value);

		/// <summary>
		/// Adds a filtering condition to the query based on the specified entity property and comparison type.
		/// </summary>
		/// <remarks>Use this method to restrict query results to entities that satisfy the specified comparison on a
		/// property. Multiple calls to <c>Where</c> can be chained to combine conditions.</remarks>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="entity">The entity instance whose property will be used for filtering. Cannot be <c>null</c>.</param>
		/// <param name="condition">An expression that specifies the property of <typeparamref name="TEntity"/> to compare. Cannot be <c>null</c>.</param>
		/// <param name="compareType">The type of comparison to apply between the property and its value. Defaults to <see cref="CompareTypes.Equal"/>.</param>
		/// <returns>The current <c>TQueryBuilder</c> instance with the applied filter condition, enabling further query composition.</returns>
		TQueryBuilder Where<TEntity>(TEntity entity, Expression<Func<TEntity, object>> condition, CompareTypes compareType = CompareTypes.Equal)
			where TEntity : IEntity;

		/// <summary>
		/// Specifies the locking behavior to be applied to the query.
		/// </summary>
		/// <remarks>Use this method to configure the locking strategy for the query, such as applying a shared or
		/// exclusive lock. The exact behavior of the lock depends on the database provider and the specified <paramref
		/// name="lockType"/>.</remarks>
		/// <param name="lockType">The type of lock to apply, represented by a value from the <see cref="LockTypes"/> enumeration.</param>
		/// <returns>An instance of <see cref="IQueryBuilder"/> with the specified locking behavior applied.</returns>
		TQueryBuilder Lock(LockTypes lockType);
	}
}
