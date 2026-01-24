
namespace UnitTests.CommonTests.ConfigurationManager
{
	using BuildHub.Common.Configuration;
	using BuildHub.Common.Configuration.Base;

	[TestClass]
	public sealed class ConfigurationManagerTests
	{
		private sealed class TestConfiguration : IConfigurationModel
		{
			public string? Value { get; set; }
		}

		[TestMethod]
		public void Get_Instance()
		{
			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			Assert.IsNotNull(configurationManager);
		}

		[TestMethod]
		public void Try_To_Get_Test_Configuration_Model()
		{
			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			TestConfiguration? testConfigurationModel = configurationManager.GetConfigurationModel<TestConfiguration>("TestConfiguration");

			Assert.IsNotNull(testConfigurationModel);
		}

		[TestMethod]
		public void Try_To_Get_Test_Configuration_Model_And_Compare_If_Values_Match()
		{
			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			TestConfiguration? testConfigurationModel = configurationManager.GetConfigurationModel<TestConfiguration>("TestConfiguration");

			Assert.AreEqual("TestValue", testConfigurationModel?.Value);
		}

		[TestMethod]
		public void Try_To_Get_Not_Existing_Config_And_Aseert_Its_Null()
		{
			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			TestConfiguration? testConfigurationModel = configurationManager.GetConfigurationModel<TestConfiguration>("NotExistingTestConfiguration");

			Assert.IsNull(testConfigurationModel);
		}

		[TestMethod]
		public void Get_Test_Configuration_List_And_Check_If_Number_Of_Configurations_Match()
		{
			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			IEnumerable<TestConfiguration>? testConfigurations = configurationManager.GetConfigurationModels<TestConfiguration>("TestConfigurations");

			Assert.AreEqual(2, testConfigurations?.Count());
		}
	}
}
