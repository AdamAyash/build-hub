namespace BuildHub.DataEngine.SQLQueries
{
	/// <summary>
	/// Defines an interface for building SQL queries in a fluent and composable manner.
	/// </summary>
	/// <remarks>This interface provides methods to construct SQL queries step-by-step, allowing for flexibility 
	/// and readability in query generation. It supports specifying the SELECT clause, FROM clause,  WHERE conditions, and
	/// locking mechanisms, as well as resetting the query builder to its initial state.</remarks>
	public interface IQueryBuilder
	{
		/// <summary>
		/// Constructs and returns a query builder configured for a SELECT operation.
		/// </summary>
		/// <remarks>Use the returned <see cref="IQueryBuilder"/> to further customize the SELECT query, such as
		/// adding filters, projections, or ordering. This method does not execute the query; it only prepares the query
		/// structure.</remarks>
		/// <returns>An <see cref="IQueryBuilder"/> instance representing the SELECT query being built.</returns>
		IQueryBuilder BuildSelect();

		/// <summary>
		/// Resets the query builder to its initial state, clearing any previously applied configurations.
		/// </summary>
		/// <returns>The current instance of the query builder, allowing for method chaining.</returns>
		IQueryBuilder Reset();

		/// <summary>
		/// Specifies the name of the table to query.
		/// </summary>
		/// <param name="tableName">The name of the table to use as the data source. Cannot be null or empty.</param>
		/// <returns>An instance of <see cref="IQueryBuilder"/> configured with the specified table name.</returns>
		IQueryBuilder From(string tableName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName"></param>
		/// <param name="compareType"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		IQueryBuilder Where(string columnName, CompareTypes compareType, object value);

		/// <summary>
		/// Adds a condition to the query that filters results based on the specified column and value.
		/// </summary>
		/// <param name="columnName">The name of the column to apply the condition to. Cannot be null or empty.</param>
		/// <param name="value">The value to compare against the specified column. Typically used for equality checks.</param>
		/// <returns>An instance of <see cref="IQueryBuilder"/> with the condition applied, allowing for further query customization.</returns>
		IQueryBuilder Where(string columnName, object value);

		/// <summary>
		/// Specifies the locking behavior to be applied to the query.
		/// </summary>
		/// <remarks>Use this method to configure the locking strategy for the query, such as applying a shared or
		/// exclusive lock. The exact behavior of the lock depends on the database provider and the specified <paramref
		/// name="lockType"/>.</remarks>
		/// <param name="lockType">The type of lock to apply, represented by a value from the <see cref="LockTypes"/> enumeration.</param>
		/// <returns>An instance of <see cref="IQueryBuilder"/> with the specified locking behavior applied.</returns>
		IQueryBuilder Lock(LockTypes lockType);

		/// <summary>
		/// Gets the query string used to retrieve or filter data.
		/// </summary>
		string Query { get; }
	}
}
