namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Specifies the database table name associated with a class for mapping purposes.
	/// </summary>
	/// <remarks>Apply this attribute to a class to indicate the corresponding table name in the database. This is
	/// commonly used in object-relational mapping (ORM) scenarios to explicitly define the table that a class maps
	/// to.</remarks>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class TableName : Attribute
	{
		/// <summary>
		/// Name of the table
		/// </summary>
		public string Name { get; private set; }

		public TableName(string name)
		{
			this.Name = name;
		}
	}
}
