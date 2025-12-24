namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;

	[TestClass]
	public class DatabaseContextTests
	{
		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void DatabaseConnectionGetContextTest(DatabaseSource databaseSource)
		{
			var databaseContext = DatabaseContext.GetCurrentContext;
			DatabaseConnection databaseConnection1 = databaseContext.GetConnection(databaseSource);
			DatabaseConnection databaseConnection2 = databaseContext.GetConnection(databaseSource);

			Assert.AreEqual(databaseConnection1, databaseConnection2);
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void TestHasContextDatabaseConnection(DatabaseSource databaseSource)
		{
			var databaseContext = DatabaseContext.GetCurrentContext;
			DatabaseConnection databaseConnection1 = databaseContext.GetConnection(databaseSource);

			Assert.IsTrue(databaseContext.HasContextDatabaseConnection(databaseSource));
		}

		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void TestHasContextDatabaseConnectionFromAnotherThread(DatabaseSource databaseSource)
		{
			var databaseContext = DatabaseContext.GetCurrentContext;
			DatabaseConnection databaseConnection1 = databaseContext.GetConnection(databaseSource);
			Assert.IsTrue(databaseContext.HasContextDatabaseConnection(databaseSource));

			Parallel.Invoke(() =>
			{
				var databaseContext = DatabaseContext.GetCurrentContext;
				Assert.IsFalse(databaseContext.HasContextDatabaseConnection(databaseSource));
			});
		}
	}
}
