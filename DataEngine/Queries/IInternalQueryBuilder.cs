namespace BuildHub.DataEngine.Queries
{
	using Entities;

	/// <summary>
	/// Defines methods for constructing SQL queries for SELECT, INSERT, UPDATE, and DELETE operations within the internal
	/// query building infrastructure.
	/// </summary>
	/// <remarks>This interface extends <see cref="IQueryBuilder"/> and provides additional methods for configuring
	/// and generating SQL queries based on entity instances or table names. It is intended for internal use within the
	/// query building system and is not designed for direct consumption by application code.</remarks>
	internal interface IInternalQueryBuilder<TQueryBuilder> : IQueryBuilder<TQueryBuilder>
	{
		/// <summary>
		/// Specifies the name of the table to query.
		/// </summary>
		/// <param name="tableName">The name of the table to use as the data source. Cannot be null or empty.</param>
		/// <returns>An instance of <see cref="IQueryBuilder"/> configured with the specified table name.</returns>
		TQueryBuilder From(string tableName);

		/// <summary>
		/// Constructs and returns a query builder configured for a SELECT operation.
		/// </summary>
		/// <remarks>Use the returned <see cref="IQueryBuilder"/> to further customize the SELECT query, such as
		/// adding filters, projections, or ordering. This method does not execute the query; it only prepares the query
		/// structure.</remarks>
		/// <returns>An <see cref="IQueryBuilder"/> instance representing the SELECT query being built.</returns>
		TQueryBuilder BuildSelect();

		/// <summary>
		/// Constructs an insert query.
		/// </summary>
		/// <returns>an insert statement</returns>
		TQueryBuilder BuildInsert<Entity>(Entity entity) where Entity : IEntity;

		/// <summary>
		/// Creates an update query for the specified entity instance.
		/// </summary>
		/// <typeparam name="Entity">The type of the entity to update. Must implement <see cref="IEntity"/>.</typeparam>
		/// <param name="entity">The entity instance containing the updated values to be applied. Cannot be null.</param>
		/// <returns>An <see cref="IQueryBuilder"/> instance representing the update query for the specified entity.</returns>
		TQueryBuilder BuildUpdate<Entity>(Entity entity) where Entity : IEntity;

		/// <summary>
		/// Builds a delete query for the specified entity instance.
		/// </summary>
		/// <typeparam name="Entity">The type of the entity to delete. Must implement <see cref="IEntity"/>.</typeparam>
		/// <param name="entity">The entity instance to be deleted. Cannot be null.</param>
		/// <returns>An <see cref="IQueryBuilder"/> instance representing the delete query for the specified entity.</returns>
		TQueryBuilder BuildDelete<Entity>(Entity entity) where Entity : IEntity;

		/// <summary>
		/// Retrieves the SQL query string associated with the current context.
		/// </summary>
		/// <returns>A string containing the SQL query. The string is empty if no query is defined.</returns>
		string GetQuery();
	}
}
