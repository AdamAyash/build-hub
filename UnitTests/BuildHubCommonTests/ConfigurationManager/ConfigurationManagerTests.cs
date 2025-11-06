namespace UnitTests.BuildHubCommonTests.ConfigurationManager
{
    using BuildHubCommon.ConfigurationManager;
    using BuildHubCommon.ConfigurationManager.Base;

    [TestClass]
    public sealed class ConfigurationManagerTests
    {
        private sealed class TestConfigurationModel : IConfigurationModel
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
        public void GetConnectionStringTest()
        {
            ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
            string? connectionString = configurationManager.GetConnectionString("BuildHubCore");

            Assert.IsNotNull(connectionString);
        }

        [TestMethod]
        public void TryToGetInvalidConnectionStringTest()
        {
            ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
            string? connectionString = configurationManager.GetConnectionString("BuildHubDBInvalid");

            Assert.IsNull(connectionString);
        }

        [TestMethod]
        public void GetTestConfigurationModelTest()
        {
            ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
            TestConfigurationModel? testConfigurationModel = configurationManager.GetConfigurationModel<TestConfigurationModel>("TestConfiguration");

            Assert.IsNotNull(testConfigurationModel);
        }

        [TestMethod]
        public void GetTestConfigurationModelAndCompareValuesTest()
        {
            ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
            TestConfigurationModel? testConfigurationModel = configurationManager.GetConfigurationModel<TestConfigurationModel>("TestConfiguration");

            Assert.AreEqual("TestValue", testConfigurationModel?.Value);
        }

        [TestMethod]
        public void GetNonExistingTestConfigurationModelTest()
        {
            ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();
            TestConfigurationModel? testConfigurationModel = configurationManager.GetConfigurationModel<TestConfigurationModel>("NotExistingTestConfiguration");

            Assert.IsNull(testConfigurationModel);
        }
    }
}
