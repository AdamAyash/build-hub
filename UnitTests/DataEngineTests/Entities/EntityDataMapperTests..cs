namespace UnitTests.DataEngineTests.Entities
{
	using BuildHub.DataEngine.Entities;
	using UnitTests.DataEngineTests.Tables;

	[TestClass]
	public sealed class EntityDataMapperTests
	{
		[TestMethod]
		public void GetColumnNameTest()
		{
			Assert.AreEqual("NAME", EntityDataMapper.GetColumnInfo<UnitTest>(x => x.Name).ColumnName);
		}

		[TestMethod]
		public void GetPrimaryKeyMappingDataTest()
		{
			Assert.AreEqual("GUID", EntityDataMapper.GetPrimaryKeyMappingData<UnitTest>().ColumnInfo.ColumnName);
		}
	}
}
