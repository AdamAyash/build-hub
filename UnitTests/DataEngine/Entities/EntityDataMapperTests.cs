namespace UnitTests.DataEngineTests.Entities
{
    using BuildHub.Common.Logger;
    using BuildHub.DataEngine.Entities;
	using UnitTests.DataEngineTests.Tables;

	[TestClass]
	public sealed class EntityDataMapperTests
	{
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Logger.Initialize();
        }

        [TestMethod]
		public void Assert_Get_Column_Name_Returns_Correct_Column_Name()
		{
			Assert.AreEqual("NAME", EntityDataMapper.GetColumnInfo<IntegrationTestEntity>(x => x.Name).ColumnName);
		}

		[TestMethod]
		public void Assert_Get_Primary_Key_Mapping_Data_Returns_Correct_Data()
		{
			Assert.AreEqual("GUID", EntityDataMapper.GetPrimaryKeyMappingData<IntegrationTestEntity>().ColumnInfo.ColumnName);
		}

		[TestMethod]
		public void Assert_Get_Table_Name_Returns_Correct_Table_Name()
		{
			Assert.AreEqual("INTEGRATION_TESTS", EntityDataMapper.GetTableName<IntegrationTestEntity>());
		}
	}
}
