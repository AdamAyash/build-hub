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
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		public void GenerateSimpleSelectStatementTest(string tableName)
		{
			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.BuildSelect();

			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK)", queryBuilder.ToString());
		}

		[TestMethod]
		[DataRow("USERS", LockTypes.None)]
		[DataRow("BUILDS", LockTypes.Update)]
		public void GenerateSimpleSelectStatementWithDifrentLockTypesTest(string tableName, LockTypes lockType)
		{
			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Lock(lockType)
				.BuildSelect();

			Assert.AreEqual($"SELECT * FROM {tableName} WITH({Utilities.GetEnumDescription<LockTypes>(lockType)})", queryBuilder.ToString());
		}

		[TestMethod]
		[DataRow("USERS",  "AGE", CompareTypes.Equal, 20)]
		[DataRow("BUILDS", "BUILD_COUNT", CompareTypes.Equal, 200)]
		[DataRow("BUILDS", "BUILD_COUNT", CompareTypes.NotEqual, 7000)]
		[DataRow("BUILDS", "BUILD_COUNT", CompareTypes.LessThanOrEqual, 400)]
		[DataRow("BUILDS", "BUILD_COUNT", CompareTypes.LessThan, 12)]
		[DataRow("BUILDS", "BUILD_COUNT", CompareTypes.GreaterThanOrEqual, 56)]
		public void GenerateSimpleSelectStatementWithDifferentWhereStatementsOnlyNumbersTest(string tableName, string columnName, CompareTypes compareTypes, object value)
		{
			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Where(columnName, compareTypes, value)
				.BuildSelect();

			string compareOperator = Utilities.GetEnumDescription<CompareTypes>(compareTypes);
			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK) WHERE {columnName} {compareOperator} {value}", queryBuilder.ToString());
		}

		[TestMethod]
		[DataRow("USERS", "AGE", CompareTypes.Equal, "TestBuild")]
		[DataRow("BUILDS", "BUILD_NAME", CompareTypes.Equal, "Test Build")]
		[DataRow("BUILDS", "BUILD_NAME", CompareTypes.NotEqual, "Test Build")]
		[DataRow("BUILDS", "BUILD_NAME", CompareTypes.LessThanOrEqual, "Build")]
		[DataRow("BUILDS", "BUILD_NAME", CompareTypes.LessThan, "Build")]
		[DataRow("BUILDS", "BUILD_NAME", CompareTypes.GreaterThanOrEqual, "Build")]
		public void GenerateSimpleSelectStatementWithDifferentWhereStatementsOnlyStringsTest(string tableName, string columnName, CompareTypes compareTypes, object value)
		{
			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Where(columnName, compareTypes, value)
				.BuildSelect();

			string compareOperator = Utilities.GetEnumDescription<CompareTypes>(compareTypes);
			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK) WHERE {columnName} {compareOperator} '{value}'", queryBuilder.ToString());
		}

		[TestMethod]
		[DataRow("USERS", "AGE", CompareTypes.Equal)]
		public void GenerateWhereStatementWithInvalidTypesTest(string tableName, string columnName, CompareTypes compareTypes)
		{
			var testObject = new TestClass();

			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Where(columnName, compareTypes, testObject);

			Assert.Throws<ArgumentException>(() => queryBuilder.BuildSelect());
		}

		[TestMethod]
		[DataRow("BUILDS", "DATE_CREATED", CompareTypes.Equal, 2026, 1, 1)]
		[DataRow("BUILDS", "DATE_CREATED", CompareTypes.LessThanOrEqual, 2024, 4, 19)]
		public void GenerateWhereStatementWithDateTimeTest(string tableName, string columnName, CompareTypes compareTypes
			, int year, int month, int day)
		{

			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Where(columnName, compareTypes, new DateTime(year, month, day))
				.BuildSelect();

			string dateToStringFormat = Utilities.FormatDateTime(new DateTime(year, month, day));
			string compareOperator = Utilities.GetEnumDescription<CompareTypes>(compareTypes);

			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK) WHERE {columnName} {compareOperator} '{dateToStringFormat}'", queryBuilder.ToString());
		}

		[TestMethod]
		public void ResetSQLQueryBuilderTest()
		{
			var queryBuilder = new SQLQueryBuilder()
				.From("Builds")
				.Where("BuildCount", CompareTypes.GreaterThan, 100)
				.Where("BuildName", CompareTypes.NotEqual, "Test Build");
			queryBuilder.Reset();

			var newQueryBuilder = queryBuilder
				.From("USERS")
				.BuildSelect();

			Assert.AreEqual("SELECT * FROM USERS WITH(NOLOCK)", newQueryBuilder.ToString());
		}


		[TestMethod]
		[DataRow(10)]
		[DataRow(1)]
		[DataRow(1000)]
		[DataRow(230)]
		[DataRow(60)]
		[DataRow(6 )]
		public void TopClauseTest(int topClauseCount)
		{
			var queryBuilder = new SQLQueryBuilder()
				.Top(10)
				.From("USERS");

			Assert.AreEqual($"SELECT TOP {topClauseCount} * FROM USERS WITH(NOLOCK)", queryBuilder.ToString());
		}
	}
}
