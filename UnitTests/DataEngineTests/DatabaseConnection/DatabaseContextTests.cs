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
		public void GetDiffrentThreadContextTest(DatabaseSource databaseSource)
		{
			Parallel.Invoke(() =>
			{
				var databaseContext = DatabaseContext.GetCurrentContext;
			});

			Parallel.Invoke(() =>
			{
				var databaseContext = DatabaseContext.GetCurrentContext;
			});


		}
	}
}
