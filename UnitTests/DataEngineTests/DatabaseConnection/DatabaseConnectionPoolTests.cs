namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;

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
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void GetConnectionTest(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);

			Assert.IsTrue(databaseConnection.IsConnectionOpen());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void RelseaseConnectionsTest(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);

			databaseConnectionPoolInstance.ReleaseDatabaseConnection(databaseConnection);

			Assert.IsTrue(databaseConnection.IsConnectionOpen());
		}

		[TestMethod]
		public void GetDatabaseConnectionFromNonExistingSourceTest()
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			Assert.Throws<KeyNotFoundException>(() => databaseConnectionPoolInstance.GetDatabaseConnection((DatabaseSource)3));
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void GetAvailableConnectionsTest(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			Assert.IsGreaterThan(0, databaseConnectionPoolInstance.GetAvailableDatabaseConnectionsCount(databaseSource));
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void GetCurrentlyUsedConnectionsTest(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);

			int currentlyUsedConnections = databaseConnectionPoolInstance.GetCurrentlyUsedConnectionsCount(databaseSource);
			Assert.AreEqual(1, currentlyUsedConnections);
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void DisposeConnectionTest(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();

			DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);

			int currentlyUsedConnectionsBeforeDispose = databaseConnectionPoolInstance.GetCurrentlyUsedConnectionsCount(databaseSource);
			databaseConnection.Dispose();

			Assert.IsGreaterThan<int>(databaseConnectionPoolInstance.GetCurrentlyUsedConnectionsCount(databaseSource), currentlyUsedConnectionsBeforeDispose);
		}
	}
}
