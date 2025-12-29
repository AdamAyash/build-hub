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
		public void Test_Connection_Should_Return_False_When_Connection_String_Is_Empty(DatabaseSource databaseSource)
		{
			var databaseConnection = new DatabaseConnection(databaseSource, "");
			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);

			Assert.IsFalse(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void Test_Connection_Should_Return_True_When_Connection_Is_Valid(DatabaseSource databaseSource)
		{
			var pool = DatabaseConnectionPool.GetInstance();
			using var databaseConnection = pool.GetDatabaseConnection(databaseSource);
			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);

			Assert.IsTrue(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void Test_Connection_Should_Return_False_When_Connection_Is_Closed(DatabaseSource databaseSource)
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
		public void Test_Connection_With_Transaction_Context_Should_Return_True_When_Valid(DatabaseSource databaseSource)
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
		public void Test_Connection_With_Null_Transaction_Context_Should_Return_True_When_Valid(DatabaseSource databaseSource)
		{
			var pool = DatabaseConnectionPool.GetInstance();
			using var databaseConnection = pool.GetDatabaseConnection(databaseSource);

			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection, null);

			Assert.IsTrue(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void Test_Connection_Should_Handle_Multiple_Consecutive_Validations(DatabaseSource databaseSource)
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
		public void Test_Connection_Should_Return_False_After_Connection_Is_Disposed(DatabaseSource databaseSource)
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
		public void Validator_Should_Accept_Connection_With_Invalid_Database_Source()
		{
			// Testing that validator handles edge case of invalid enum value
			var databaseConnection = new DatabaseConnection((DatabaseSource)999, "");
			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);

			Assert.IsFalse(databaseConnectionValidator.TestDatabaseConnection());
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void Test_Connection_With_Disposed_Transaction_Should_Handle_Gracefully(DatabaseSource databaseSource)
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