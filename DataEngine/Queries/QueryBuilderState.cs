namespace BuildHub.DataEngine.Queries
{
	/// <summary>
	/// Represents the current state of a query being constructed, including filtering conditions, locking behavior, and
	/// result limits.
	/// </summary>
	/// <remarks> <see cref="QueryBuilderState"/> is typically used to track and modify the components of a query as
	/// it is built programmatically. It encapsulates the collection of WHERE conditions, the locking strategy to apply,
	/// and the maximum number of results to return. The state can be reset to its default values using the <see
	/// cref="Reset"/> method.</remarks>
	public sealed class QueryBuilderState
	{
		/// <summary>
		/// Gets or sets the collection of WHERE clause conditions to apply to a query.
		/// </summary>
		public List<WhereCondition> WhereStatements {  get; set; }

		/// <summary>
		/// Gets or sets the type of lock to apply to the resource.
		/// </summary>
		public LockTypes LockType { get; set; }

		/// <summary>
		/// Gets or sets the top statement count.
		/// </summary>
		public int TopStatementCount { get; set; }

		public QueryBuilderState()
		{
			this.WhereStatements = new List<WhereCondition>();
			this.Reset();
		}

		/// <summary>
		/// Resets the query builder to its initial state, clearing all applied filters and settings.
		/// </summary>
		/// <remarks>After calling <see cref="Reset"/>, all previously specified query conditions, lock types, and
		/// result limits are removed. The builder can then be reused to construct a new query from scratch.</remarks>
		public void Reset()
		{
			this.WhereStatements.Clear();
			this.LockType = LockTypes.None;
			this.TopStatementCount = -1;
		}
	}
}
