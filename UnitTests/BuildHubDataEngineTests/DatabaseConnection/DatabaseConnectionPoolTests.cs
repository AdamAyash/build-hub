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

        [TestMethod]
        public void GetDatabaseConnectionTest()
        {
            DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
            int availableConnectionsCount = databaseConnectionPoolInstance.AvailableConnections;
            var databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection();

            Assert.IsTrue(databaseConnection != null
              && databaseConnection.IsConnectionOpen()
              && Math.Abs(databaseConnectionPoolInstance.AvailableConnections - availableConnectionsCount) == 1);
        }

        [TestMethod]
        public void ReleaseDatabaseConnectionTest()
        {
            DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
            var databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection();
            int availableConnectionsCount = databaseConnectionPoolInstance.AvailableConnections;

            databaseConnectionPoolInstance.ReleaseDatabaseConnection(databaseConnection);

            Assert.AreEqual(1, databaseConnectionPoolInstance.AvailableConnections - availableConnectionsCount);
        }
    }
}
