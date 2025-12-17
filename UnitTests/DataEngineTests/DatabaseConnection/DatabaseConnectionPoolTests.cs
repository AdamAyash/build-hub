namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;
	using BuildHub.DataEngine.Exceptions;

	[TestClass]
	public sealed class DatabaseConnectionPoolTests
	{
		[TestMethod]
		[DoNotParallelize]
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
			Assert.Throws<KeyNotFoundException>(() => databaseConnectionPoolInstance.GetDatabaseConnection((DatabaseSource)falseDatabaseSource));
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
			for (int index = 0; index < databaseConnectionPoolInstance.GetAvailableDatabaseConnectionsCount(databaseSource); ++index)
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

			for (int index = 0; index < databaseConnectionPoolInstance.GetAvailableDatabaseConnectionsCount(databaseSource) + 1; ++index)
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
		public void PoolExhaustion_WithRetry_ShouldEventuallyGetConnection(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool pool = DatabaseConnectionPool.GetInstance();
			int maxConnections = pool.GetAvailableDatabaseConnectionsCount(databaseSource);
			List<DatabaseConnection> heldConnections = new List<DatabaseConnection>();

			try
			{
				for (int i = 0; i < maxConnections; i++)
				{
					heldConnections.Add(pool.GetDatabaseConnection(databaseSource));
				}

				var task = Task.Run(() =>
				{
					Thread.Sleep(1000); // Wait a bit
					using var conn = pool.GetDatabaseConnection(databaseSource);
					Assert.IsNotNull(conn);
				});

				Thread.Sleep(500);
				pool.ReleaseDatabaseConnection(heldConnections[0]);
				heldConnections.RemoveAt(0);

				Assert.IsTrue(task.Wait(5000), "Should get connection after retry");
			}
			finally
			{
				foreach (var conn in heldConnections)
				{
					pool.ReleaseDatabaseConnection(conn);
				}
			}
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void PoolExhaustion_ExceedMaxRetries_ShouldThrowException(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool pool = DatabaseConnectionPool.GetInstance();
			int maxConnections = pool.GetAvailableDatabaseConnectionsCount(databaseSource);
			List<DatabaseConnection> heldConnections = new List<DatabaseConnection>();

			try
			{
				for (int i = 0; i < maxConnections; i++)
				{
					heldConnections.Add(pool.GetDatabaseConnection(databaseSource));
				}

				Assert.Throws<ConnectionPoolExhaustedException>(() =>
					pool.GetDatabaseConnection(databaseSource));
			}
			finally
			{
				foreach (var conn in heldConnections)
				{
					pool.ReleaseDatabaseConnection(conn);
				}
			}
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void TryToReleaseConnectionTwice(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool pool = DatabaseConnectionPool.GetInstance();
			int initialCount = pool.GetAvailableDatabaseConnectionsCount(databaseSource);

			var conn = pool.GetDatabaseConnection(databaseSource);
			pool.ReleaseDatabaseConnection(conn);

			// Releasing again should either be idempotent or throw exception
			// Adjust based on your implementation
			try
			{
				pool.ReleaseDatabaseConnection(conn);
			}
			catch (InvalidOperationException)
			{
				// Expected if your implementation throws on double release
			}

			// Pool count should not exceed initial
			Assert.IsLessThanOrEqualTo(pool.GetAvailableDatabaseConnectionsCount(databaseSource), initialCount);
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void GetCurrentlyUsedConnections_ShouldReflectActualUsage(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool pool = DatabaseConnectionPool.GetInstance();
			int initialUsed = pool.GetCurrentlyUsedConnectionsCount(databaseSource);

			var conn1 = pool.GetDatabaseConnection(databaseSource);
			Assert.AreEqual(initialUsed + 1, pool.GetCurrentlyUsedConnectionsCount(databaseSource));

			var conn2 = pool.GetDatabaseConnection(databaseSource);
			Assert.AreEqual(initialUsed + 2, pool.GetCurrentlyUsedConnectionsCount(databaseSource));

			pool.ReleaseDatabaseConnection(conn1);
			Assert.AreEqual(initialUsed + 1, pool.GetCurrentlyUsedConnectionsCount(databaseSource));

			pool.ReleaseDatabaseConnection(conn2);
			Assert.AreEqual(initialUsed, pool.GetCurrentlyUsedConnectionsCount(databaseSource));
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void TryToGetConnectionFromEmptyPool(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool pool = DatabaseConnectionPool.GetInstance();
			int maxConnections = pool.GetCurrentlyUsedConnectionsCount(databaseSource);
			Parallel.For(0, maxConnections, x =>
			{
				var connection = pool.GetDatabaseConnection(databaseSource);
			});

			Assert.Throws<ConnectionPoolExhaustedException>(() => pool.GetDatabaseConnection(databaseSource));
		}
	}
}
