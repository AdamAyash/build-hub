using BuildHub.DataEngine.Entities;
using BuildHub.DataEngine.Tables.Base;

namespace UnitTests.DataEngineTests.Tables
{
	internal sealed class InegrationTestWithUnmappedFieldTable: BaseTable<InegrationTestWithUnmappedFieldEntity>
	{
		public InegrationTestWithUnmappedFieldTable()
			: base(DatabaseSource.IntegrationTests)
		{
		}
	}

	[TableName("INTEGRATION_TESTS")]
	internal class InegrationTestWithUnmappedFieldEntity : BaseEntity
	{
		public int UnitTestUnmappedProperty { get; set; }

		public InegrationTestWithUnmappedFieldEntity()
		{
		}
	}
}
