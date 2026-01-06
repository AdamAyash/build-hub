namespace UnitTests.DataEngineTests.Entities
{
	using BuildHub.DataEngine.Entities;
	using UnitTests.DataEngineTests.Tables;

	[TestClass]
	public sealed class EntityDataMapperTests
	{
		[TestMethod]
		public void Assert_Get_Column_Name_Returns_Correct_Column_Name()
		{
			Assert.AreEqual("NAME", EntityDataMapper.GetColumnInfo<UnitTest>(x => x.Name).ColumnName);
		}

		[TestMethod]
		public void Assert_Get_Primary_Key_Mapping_Data_Returns_Correct_Data()
		{
			Assert.AreEqual("GUID", EntityDataMapper.GetPrimaryKeyMappingData<UnitTest>().ColumnInfo.ColumnName);
		}

		[TestMethod]
		public void Assert_Get_Table_Name_Returns_Correct_Table_Name()
		{
			Assert.AreEqual("UNIT_TESTS", EntityDataMapper.GetTableName<UnitTest>());
		}
	}
}
