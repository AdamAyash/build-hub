namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;
	using BuildHub.DataEngine.Exceptions.DatabaseConnection;

	[TestClass]
	[TestCategory("Integration")]
	[DoNotParallelize]
	public sealed class DatabaseConnectionPoolTests
	{
		[TestMethod]
		public void Get_Instance_Should_Return_Singleton_Instance()
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			Assert.IsNotNull(databaseConnectionPoolInstance);
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_connection_should_return_open_connection_for_valid_database_source(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			using DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);

			Assert.IsTrue(databaseConnection.IsConnectionOpen());
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Release_Connection_Should_Keep_Connection_Open_And_Return_To_Pool(DatabaseSource databaseSource)
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
		public void Get_Connection_Should_Throw_Key_Not_Found_Exception_For_Invalid_Database_Source(int falseDatabaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			Assert.Throws<KeyNotFoundException>(() => databaseConnectionPoolInstance.GetDatabaseConnection((DatabaseSource)falseDatabaseSource));
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_Available_Connections_Count_Should_Return_Positive_Number(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			Assert.IsGreaterThan(0, databaseConnectionPoolInstance.GetAvailableDatabaseConnectionsCount(databaseSource));
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Dispose_Connection_Should_Decrease_Currently_Used_Connections_Count(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();

			DatabaseConnection databaseConnection = databaseConnectionPoolInstance.GetDatabaseConnection(databaseSource);

			int currentlyUsedConnectionsBeforeDispose = databaseConnectionPoolInstance.GetCurrentlyUsedConnectionsCount(databaseSource);
			databaseConnection.Dispose();

			Assert.IsGreaterThan<int>(databaseConnectionPoolInstance.GetCurrentlyUsedConnectionsCount(databaseSource), currentlyUsedConnectionsBeforeDispose);
		}

		[TestMethod]
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_Connections_Concurrently_Should_Return_Valid_Connections(DatabaseSource databaseSource)
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
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_Connections_Exceeding_Pool_Size_Concurrently_Should_Handle_Gracefully(DatabaseSource databaseSource)
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
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Exhausted_Pool_With_Retry_Should_Eventually_Get_Connection_When_Released(DatabaseSource databaseSource)
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
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Exhausted_Pool_Exceeding_Max_Retries_Should_Throw_Pool_Exhausted_Exception(DatabaseSource databaseSource)
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
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Release_Connection_Twice_Should_Not_Corrupt_Pool_Count(DatabaseSource databaseSource)
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
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_Currently_Used_Connections_Count_Should_Reflect_Actual_Usage(DatabaseSource databaseSource)
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
		[DataRow(DatabaseSource.IntegrationTests)]
		public void Get_Connection_From_Exhausted_Pool_Should_Throw_Pool_Exhausted_Exception(DatabaseSource databaseSource)
		{
			DatabaseConnectionPool pool = DatabaseConnectionPool.GetInstance();
			int maxConnections = pool.GetAvailableDatabaseConnectionsCount(databaseSource);

			Parallel.For(0, maxConnections, x =>
			{
				var connection = pool.GetDatabaseConnection(databaseSource);
			});

			Assert.Throws<ConnectionPoolExhaustedException>(() => pool.GetDatabaseConnection(databaseSource));
		}
	}
}
