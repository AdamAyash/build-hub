using BuildHub.Common.Utilities;
using BuildHub.DataEngine.SQLQueries;

namespace UnitTests.DataEngineTests.SQLQueries
{
	[TestClass]
	public class SQLQueriesTests
	{
		[TestMethod]
		[DataRow("Users")]
		public void GenerateSimpleSelectStatementTest(string tableName)
		{
			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.BuildSelect();

			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK)", queryBuilder.Query);
		}

		[TestMethod]
		[DataRow("Users", LockTypes.None)]
		[DataRow("Builds", LockTypes.Update)]
		public void GenerateSimpleSelectStatementWithDifrentLockTypesTest(string tableName, LockTypes lockType)
		{
			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Lock(lockType)
				.BuildSelect();

			Assert.AreEqual($"SELECT * FROM {tableName} WITH({Utilities.GetEnumDescription<LockTypes>(lockType)})", queryBuilder.Query);
		}
	}
}
