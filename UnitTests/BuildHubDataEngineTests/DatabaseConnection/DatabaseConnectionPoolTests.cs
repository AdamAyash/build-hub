using BuildHubDataEngine.DatabaseConnection;

namespace UnitTests.BuildHubDataEngineTests.DatabaseConnection
{
    [TestClass]
    public sealed class DatabaseConnectionPoolTests
    {
        [TestMethod]
        public void GetInstanceTest()
        {
            DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
            Assert.IsNotNull(databaseConnectionPoolInstance);
        }
    }
}
