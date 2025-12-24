using BuildHub.DataEngine.Entities;

namespace UnitTests.DataEngineTests.Tables
{
	internal class UnitTest : BaseEntity
	{
		[ColumnInfo("NAME")]
		public string Name { get; set; }

		public UnitTest()
		{
			this.Name = string.Empty;
		}
	}
}
