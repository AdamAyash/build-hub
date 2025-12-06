namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;
	using BuildHub.DataEngine.Exceptions;

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
			using DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);

			Assert.IsTrue(databaseConnection.IsConnectionOpen());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void ReleaseConnectionsTest(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);

			databaseConnectionPoolInstance.ReleaseDatabaseConnection(databaseConnection);
			Assert.IsTrue(databaseConnection.IsConnectionOpen());
		}

		[TestMethod]
		[DataRow(-1)]
		[DataRow(-2)]
		[DataRow(-3)]
		public void GetDatabaseConnectionFromNonExistingSourceTest(int falseDatabaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			Assert.Throws<MissingDatabaseConfigurationException>(() => databaseConnectionPoolInstance.GetDatabaseConnection((DatabaseSource)falseDatabaseSource));
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
		public void DisposeConnectionTest(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();

			DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);

			int currentlyUsedConnectionsBeforeDispose = databaseConnectionPoolInstance.GetCurrentlyUsedConnectionsCount(databaseSource);
			databaseConnection.Dispose();

			Assert.IsGreaterThan<int>(databaseConnectionPoolInstance.GetCurrentlyUsedConnectionsCount(databaseSource), currentlyUsedConnectionsBeforeDispose);
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void GetParallelDatbaseConnectionsTest(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			for(int index = 0; index < databaseConnectionPoolInstance.GetAvailableDatabaseConnectionsCount(databaseSource); ++index)
			{
				Parallel.Invoke(() =>
				{
					using DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);
					Assert.IsNotNull(databaseConnection);
				});
			}
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void GetMaxAvailableDatabseConenctionPoolTest(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();

			for (int index = 0; index < databaseConnectionPoolInstance.GetAvailableDatabaseConnectionsCount(databaseSource); ++index)
			{
				Parallel.Invoke(() =>
				{
					using DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);
					Assert.IsNotNull(databaseConnection);
				});
			}
		}
	}
}
