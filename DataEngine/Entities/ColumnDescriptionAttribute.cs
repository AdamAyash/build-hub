namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Specifies metadata for a property that maps to a database column.
	/// </summary>
	/// <remarks>Apply this attribute to a property to indicate the corresponding column name and, optionally, the
	/// column size in the database schema. This attribute is typically used in data access scenarios to facilitate
	/// object-relational mapping.</remarks>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class ColumnInfo : Attribute
	{
		/// <summary>
		/// Name of the column
		/// </summary>
		private readonly string _columnName;

		/// <summary>
		/// Size of the column
		/// </summary>
		private readonly int _size;

		public string ColumnName => this._columnName;
		public int Size => this._size;

		public ColumnInfo(string columnName, int size = 0)
		{
			this._columnName = columnName;
			this._size = size;
		}
	}
}
