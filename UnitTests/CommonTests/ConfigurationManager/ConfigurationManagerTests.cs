
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
		public void GetInstanceTest()
		{
			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			Assert.IsNotNull(configurationManager);
		}

		[TestMethod]
		public void GetTestConfigurationModelTest()
		{
			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			TestConfiguration? testConfigurationModel = configurationManager.GetConfigurationModel<TestConfiguration>("TestConfiguration");

			Assert.IsNotNull(testConfigurationModel);
		}

		[TestMethod]
		public void GetTestConfigurationModelAndCompareValuesTest()
		{
			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			TestConfiguration? testConfigurationModel = configurationManager.GetConfigurationModel<TestConfiguration>("TestConfiguration");

			Assert.AreEqual("TestValue", testConfigurationModel?.Value);
		}

		[TestMethod]
		public void GetNonExistingTestConfigurationModelTest()
		{
			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			TestConfiguration? testConfigurationModel = configurationManager.GetConfigurationModel<TestConfiguration>("NotExistingTestConfiguration");

			Assert.IsNull(testConfigurationModel);
		}

		[TestMethod]
		public void GetTestConfigurationModels()
		{
			ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
			IEnumerable<TestConfiguration>? testConfigurations = configurationManager.GetConfigurationModels<TestConfiguration>("TestConfigurations");

			Assert.AreEqual(2, testConfigurations?.Count());
		}
	}
}
