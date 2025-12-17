using BuildHub.DataEngine.Entities;

namespace UnitTests.DataEngineTests.Tables
{
	internal class UnitTest : BaseEntity
	{
		[ColumnDescription("NAME")]
		public string Name { get; set; }

		public UnitTest()
		{
		}
	}
}
