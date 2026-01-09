namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;
	using BuildHub.DataEngine.Transactions;
	using Moq;

	[TestClass]
	[DoNotParallelize]
	public class DatabaseContextTests
	{

		[TestMethod]
		public void Test_Multiple_Database_Sources_Are_Independent()
		{
			var databaseContext = DatabaseContext.GetCurrentContext;

			DatabaseConnection usersConnection = databaseContext.GetConnection(DatabaseSource.Users);
			DatabaseConnection coreConnection = databaseContext.GetConnection(DatabaseSource.Core);

			Assert.AreNotEqual(usersConnection, coreConnection);
			Assert.IsTrue(databaseContext.HasContextDatabaseConnection(DatabaseSource.Users));
			Assert.IsTrue(databaseContext.HasContextDatabaseConnection(DatabaseSource.Core));

			databaseContext.ClearContext();
		}

		[TestMethod]
		[DataRow(DatabaseSource.UnitTests)]
		public void Test_Has_Context_Database_Connection_Returns_False_For_Non_Existent_Connection(DatabaseSource databaseSource)
		{
			var databaseContext = DatabaseContext.GetCurrentContext;
			databaseContext.ClearContext();

			Assert.IsFalse(databaseContext.HasContextDatabaseConnection(databaseSource));
		}

		[TestMethod]
		[DataRow(DatabaseSource.UnitTests)]
		public void Test_Transaction_Context_Is_Null_By_Default(DatabaseSource databaseSource)
		{
			var databaseContext = DatabaseContext.GetCurrentContext;

			Assert.IsNull(databaseContext.TransactionContext);
			databaseContext.ClearContext();
		}

		[TestMethod]
		[DataRow(DatabaseSource.UnitTests)]
		public void Test_Transaction_Context_Can_Be_Set(DatabaseSource databaseSource)
		{
			var databaseContext = DatabaseContext.GetCurrentContext;
			var mockTransactionContext = new Mock<ITransactionContext>();

			databaseContext.TransactionContext = mockTransactionContext.Object;

			Assert.IsNotNull(databaseContext.TransactionContext);
			Assert.AreEqual(mockTransactionContext.Object, databaseContext.TransactionContext);

			databaseContext.ClearContext();
		}

		[TestMethod]
		public void Test_Different_Threads_Get_Different_Contexts()
		{
			var databaseContext1 = DatabaseContext.GetCurrentContext;
			DatabaseContext databaseContext2 = null;

			var thread = new Thread(() =>
			{
				databaseContext2 = DatabaseContext.GetCurrentContext;
			});

			thread.Start();
			thread.Join();

			Assert.AreNotEqual(databaseContext1, databaseContext2);
			databaseContext1.ClearContext();
			databaseContext2.ClearContext();
		}

		[TestMethod]
		[DataRow(DatabaseSource.UnitTests)]
		public void Test_Get_Connection_Returns_Valid_Connection(DatabaseSource databaseSource)
		{
			var databaseContext = DatabaseContext.GetCurrentContext;
			DatabaseConnection databaseConnection = databaseContext.GetConnection(databaseSource);

			Assert.IsNotNull(databaseConnection);
			Assert.IsNotNull(databaseConnection.InternalConnection);

			databaseContext.ClearContext();
		}

		[TestMethod]
		[DataRow(DatabaseSource.UnitTests)]
		public void Test_Connection_Validation_Replaces_Invalid_Connection(DatabaseSource databaseSource)
		{
			var databaseContext = DatabaseContext.GetCurrentContext;
			 DatabaseConnection firstConnection = databaseContext.GetConnection(databaseSource);
			firstConnection.InternalConnection.Close();

			DatabaseConnection secondConnection = databaseContext.GetConnection(databaseSource);
			Assert.IsNotNull(secondConnection);

			databaseContext.ClearContext();
		}

		[TestMethod]
		public async Task Test_Async_Context_Isolation()
		{
			var context1 = DatabaseContext.GetCurrentContext;
			var conn1 = context1.GetConnection(DatabaseSource.Users);

			DatabaseContext context2 = null;
			DatabaseConnection conn2 = null;

			await Task.Run(() =>
			{
				context2 = DatabaseContext.GetCurrentContext;
				conn2 = context2.GetConnection(DatabaseSource.Users);
			});

			Assert.AreNotEqual(context1, context2);
			Assert.AreNotEqual(conn1, conn2);
			context1.ClearContext();
			context2.ClearContext();
		}
	}
}
