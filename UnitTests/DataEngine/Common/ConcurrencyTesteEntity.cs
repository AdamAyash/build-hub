using BuildHub.DataEngine.Entities;

namespace UnitTests.DataEngine.Common
{
	[TableName("CONCURRENCY_TESTS")]
	internal class ConcurrencyTesteEntity : VersionedEntity
	{
		[ColumnInfo("NAME")]
		public string Name { get; set; }

		public ConcurrencyTesteEntity()
		{
			this.Name = string.Empty;
		}
	}
}
