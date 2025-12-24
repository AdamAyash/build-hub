namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Represents an entity that maintains versioning and timestamp information for creation and updates.
	/// </summary>
	/// <remarks>Use this class as a base for entities that require tracking of version history and audit
	/// timestamps. The version number can be used to implement optimistic concurrency or change tracking. The creation and
	/// update timestamps provide audit information for when the entity was first created and last modified.</remarks>
	public class VersionedEntity : BaseEntity
	{
		[ColumnInfo("VERSION")]
		public int Version { get; set; }

		[ColumnInfo("CREATED_AT")]
		public DateTime CreatedAt { get; set; }

		[ColumnInfo("UPDATED_AT")]
		public DateTime UpdatedAt { get; set; }

		public VersionedEntity()
		{
		}
	}
}
