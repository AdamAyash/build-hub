using BuildHub.Common.Utilities;
using BuildHub.DataEngine.SQLQueries;

namespace UnitTests.DataEngineTests.SQLQueries
{
	[TestClass]
	public class SQLQueriesTests
	{
		private class TestClass
		{
			public int Id { get; set; }
			public int Value { get; set; }
		}

		[TestMethod]
		[DataRow("Users")]
		[DataRow("Builds")]
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

		[TestMethod]
		[DataRow("Users", "Age", CompareTypes.Equal, 20)]
		[DataRow("Builds", "BuildCount", CompareTypes.Equal, 200)]
		[DataRow("Builds", "BuildCount", CompareTypes.NotEqual, 7000)]
		[DataRow("Builds", "BuildCount", CompareTypes.LessThanOrEqual, 400)]
		[DataRow("Builds", "BuildCount", CompareTypes.LessThan, 12)]
		[DataRow("Builds", "BuildCount", CompareTypes.GreaterThanOrEqual, 56)]
		public void GenerateSimpleSelectStatementWithDifferentWhereStatementsOnlyNumbersTest(string tableName, string columnName, CompareTypes compareTypes, object value)
		{
			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Where(columnName, compareTypes, value)
				.BuildSelect();

			string compareOperator = Utilities.GetEnumDescription<CompareTypes>(compareTypes);
			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK) WHERE {columnName} {compareOperator} {value}", queryBuilder.Query);
		}

		[TestMethod]
		[DataRow("Users", "Age", CompareTypes.Equal, "TestBuild")]
		[DataRow("Builds", "BuildName", CompareTypes.Equal, "Test Build")]
		[DataRow("Builds", "BuildName", CompareTypes.NotEqual, "Test Build")]
		[DataRow("Builds", "BuildName", CompareTypes.LessThanOrEqual, "Build")]
		[DataRow("Builds", "BuildName", CompareTypes.LessThan, "Build")]
		[DataRow("Builds", "BuildName", CompareTypes.GreaterThanOrEqual, "Build")]
		public void GenerateSimpleSelectStatementWithDifferentWhereStatementsOnlyStringsTest(string tableName, string columnName, CompareTypes compareTypes, object value)
		{
			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Where(columnName, compareTypes, value)
				.BuildSelect();

			string compareOperator = Utilities.GetEnumDescription<CompareTypes>(compareTypes);
			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK) WHERE {columnName} {compareOperator} '{value}'", queryBuilder.Query);
		}

		[TestMethod]
		[DataRow("Users", "Age", CompareTypes.Equal)]
		public void GenerateWhereStatementWithInvalidTypesTest(string tableName, string columnName, CompareTypes compareTypes)
		{
			var testObject = new TestClass();

			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Where(columnName, compareTypes, testObject);

			Assert.Throws<ArgumentException>(() => queryBuilder.BuildSelect());
		}

		[TestMethod]
		[DataRow("Builds", "DateCreated", CompareTypes.Equal, 2026, 1, 1)]
		[DataRow("Builds", "DateCreated", CompareTypes.LessThanOrEqual, 2024, 4, 19)]
		public void GenerateWhereStatementWithDateTimeTest(string tableName, string columnName, CompareTypes compareTypes
			, int year, int month, int day)
		{

			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Where(columnName, compareTypes, new DateTime(year, month, day))
				.BuildSelect();

			string dateToStringFormat = Utilities.FormatDateTime(new DateTime(year, month, day));
			string compareOperator = Utilities.GetEnumDescription<CompareTypes>(compareTypes);

			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK) WHERE {columnName} {compareOperator} '{dateToStringFormat}'", queryBuilder.Query);
		}
	}
}
