namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Represents an entity with read-only properties.
	/// </summary>
	/// <remarks><see cref="BaseEntity"/> is intended for scenarios where entity data should not be modified
	/// after creation. All properties are read-only.</remarks>
	public class BaseEntity : IEntity
	{
		[ColumnDescription("ID")]
		public int Id { get; set; }

		[PrimaryKey]
		[ColumnDescription("GUID")]
		public Guid Guid { get; set; }

		public BaseEntity()
		{
		}
	}
}
