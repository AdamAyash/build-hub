namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Represents an entity with read-only properties.
	/// </summary>
	/// <remarks><see cref="BaseEntity"/> is intended for scenarios where entity data should not be modified
	/// after creation. All properties are read-only.
	/// </remarks>
	public class BaseEntity : IEntity
	{
		[Identity]
		[ColumnInfo("ID")]
		public int Id { get; protected set; }

		[PrimaryKey]
		[ColumnInfo("GUID")]
		public Guid Guid { get; set; }

		public BaseEntity()
		{
		}
	}
}
