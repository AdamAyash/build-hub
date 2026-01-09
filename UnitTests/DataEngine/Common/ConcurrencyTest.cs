using BuildHub.DataEngine.Entities;

namespace UnitTests.DataEngine.Common
{
	[TableName("CONCURRENCY_TESTS")]
	internal class ConcurrencyTest : VersionedEntity
	{
		[ColumnInfo("NAME")]
		public string Name { get; set; }

		public ConcurrencyTest()
		{
			this.Name = string.Empty;
		}
	}
}
