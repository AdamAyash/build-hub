using BuildHub.DataEngine.Entities;
using BuildHub.DataEngine.Tables.Base;

namespace UnitTests.DataEngineTests.Tables
{
	internal sealed class UnitTestsWithUnmappedFieldTable: BaseTable<UnitTestWithUnmappedField>
	{
		public UnitTestsWithUnmappedFieldTable()
		{
		}
	}

	[TableName("UNIT_TESTS")]
	internal class UnitTestWithUnmappedField : BaseEntity
	{
		public int UnitTestUnmappedProperty { get; set; }

		public UnitTestWithUnmappedField()
		{
		}
	}
}
