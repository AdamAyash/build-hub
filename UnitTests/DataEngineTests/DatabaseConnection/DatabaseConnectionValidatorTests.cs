namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;
	using BuildHub.DataEngine.Transactions;

	[TestClass]
	public class DatabaseConnectionValidatorTests
	{
		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void test_connection_should_return_false_when_connection_string_is_empty(DatabaseSource databaseSource)
		{
			var databaseConnection = new DatabaseConnection(databaseSource, "");
			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);

			Assert.IsFalse(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void test_connection_should_return_true_when_connection_is_valid(DatabaseSource databaseSource)
		{
			var pool = DatabaseConnectionPool.GetInstance();
			using var databaseConnection = pool.GetDatabaseConnection(databaseSource);
			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);

			Assert.IsTrue(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void test_connection_should_return_false_when_connection_is_closed(DatabaseSource databaseSource)
		{
			var pool = DatabaseConnectionPool.GetInstance();
			var databaseConnection = pool.GetDatabaseConnection(databaseSource);
			databaseConnection.Dispose();

			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);

			Assert.IsFalse(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void test_connection_with_transaction_context_should_return_true_when_valid(DatabaseSource databaseSource)
		{
			var pool = DatabaseConnectionPool.GetInstance();
			using var databaseConnection = pool.GetDatabaseConnection(databaseSource);

			var transactionContext = new ScopedTransaction();
			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection, transactionContext);

			Assert.IsTrue(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void test_connection_with_null_transaction_context_should_return_true_when_valid(DatabaseSource databaseSource)
		{
			var pool = DatabaseConnectionPool.GetInstance();
			using var databaseConnection = pool.GetDatabaseConnection(databaseSource);

			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection, null);

			Assert.IsTrue(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void test_connection_should_handle_multiple_consecutive_validations(DatabaseSource databaseSource)
		{
			var pool = DatabaseConnectionPool.GetInstance();
			using var databaseConnection = pool.GetDatabaseConnection(databaseSource);
			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);

			// Test multiple times to ensure validator is reusable
			Assert.IsTrue(databaseConnectionValidator.TestDatabaseConnection());
			Assert.IsTrue(databaseConnectionValidator.TestDatabaseConnection());
			Assert.IsTrue(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void test_connection_should_return_false_after_connection_is_disposed(DatabaseSource databaseSource)
		{
			var pool = DatabaseConnectionPool.GetInstance();
			var databaseConnection = pool.GetDatabaseConnection(databaseSource);
			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);

			// First validation should succeed
			Assert.IsTrue(databaseConnectionValidator.TestDatabaseConnection());

			// Close the connection
			databaseConnection.Dispose();

			// Second validation should fail
			Assert.IsFalse(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		public void validator_should_accept_connection_with_invalid_database_source()
		{
			// Testing that validator handles edge case of invalid enum value
			var databaseConnection = new DatabaseConnection((DatabaseSource)999, "");
			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);

			Assert.IsFalse(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void test_connection_with_disposed_transaction_should_handle_gracefully(DatabaseSource databaseSource)
		{
			var pool = DatabaseConnectionPool.GetInstance();
			using var databaseConnection = pool.GetDatabaseConnection(databaseSource);

			var transactionContext = new ScopedTransaction();
			transactionContext.Dispose();

			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection, transactionContext);

			// Should handle disposed transaction without throwing
			try
			{
				bool result = databaseConnectionValidator.TestDatabaseConnection();
				// Result may be true or false depending on implementation, but shouldn't throw
				Assert.IsNotNull(result);
			}
			catch (Exception ex)
			{
				Assert.Fail($"Should not throw exception: {ex.Message}");
			}
		}
	}
}