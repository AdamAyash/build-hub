namespace BuildHub.DataEngine.Entities
{
	internal class VersionedEntity : BaseEntity
	{
		[ColumnDescription("VERSION")]
		public int Version { get; set; }
		[ColumnDescription("CREATED_AT")]
		public DateTime CreatedAt { get; set; }
		[ColumnDescription("UPDATED_AT")]
		public DateTime UpdatedAt { get; set; }

		public VersionedEntity()
		{
		}
	}
}
