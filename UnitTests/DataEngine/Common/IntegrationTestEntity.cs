using BuildHub.DataEngine.Entities;

namespace UnitTests.DataEngineTests.Tables
{
	[TableName("INTEGRATION_TESTS")]
	internal class IntegrationTestEntity : VersionedEntity
	{
		[ColumnInfo("NAME")]
		public string Name { get; set; }

		public IntegrationTestEntity()
		{
			this.Name = string.Empty;
		}
	}
}
