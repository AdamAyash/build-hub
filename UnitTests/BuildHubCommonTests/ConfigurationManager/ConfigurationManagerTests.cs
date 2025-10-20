namespace UnitTests.BuildHubCommonTests.ConfigurationManager
{
    using BuildHubCommon.ConfigurationManager;

    [TestClass]
    public sealed class ConfigurationManagerTests
    {
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
            string? connectionString = configurationManager.GetConnectionString("BuildHubDB");

            Assert.IsNotNull(connectionString);
        }
    }
}
