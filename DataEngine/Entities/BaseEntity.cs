namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Represents an entity with read-only properties.
	/// </summary>
	/// <remarks><see cref="BaseEntity"/> is intended for scenarios where entity data should not be modified
	/// after creation. All properties are read-only.</remarks>
	public class BaseEntity : IEntity
	{
		[Identity]
		[ColumnDescription("ID")]
		public int Id { get; private set; }

		[PrimaryKey]
		[ColumnDescription("GUID")]
		public Guid Guid { get; set; }

		public BaseEntity()
		{
		}
	}
}
