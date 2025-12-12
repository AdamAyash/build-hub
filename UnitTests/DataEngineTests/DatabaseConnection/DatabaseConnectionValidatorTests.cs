namespace UnitTests.DataEngineTests.DatabaseConnection
{
	using BuildHub.DataEngine.DatabaseConnection;

	[TestClass]
	public class DatabaseConnectionValidatorTests
	{
		[TestMethod]
		[DataRow(DatabaseSource.Users)]
		[DataRow(DatabaseSource.Core)]
		public void TestInvalidConnection(DatabaseSource databaseSource)
		{
			var databaseConnection = new DatabaseConnection(databaseSource, "");
			var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);

			Assert.IsFalse(databaseConnectionValidator.TestDatabaseConnection());
		}
	}
}
