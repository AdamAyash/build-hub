#region
using BuildHub.Common.Utilities;
using BuildHub.DataEngine.Exceptions;
using BuildHub.DataEngine.SQLQueries;
using System.Reflection.Metadata;
using UnitTests.DataEngineTests.Tables;
#endregion

namespace UnitTests.DataEngineTests.SQLQueries
{
	[TestClass]
	public class SQLQueryBuilderTests
	{
		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		[DataRow("UNIT_TESTS")]
		public void Build_Select_Should_Generate_Correct_Simple_Query(string tableName)
		{
			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.BuildSelect();

			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK)", queryBuilder.GetQuery());
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

			Assert.AreEqual($"SELECT * FROM {tableName} WITH({Utilities.GetEnumDescription<LockTypes>(lockType)})", queryBuilder.GetQuery());
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
			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK) WHERE {columnName} {compareOperator} {value}", queryBuilder.GetQuery());
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
			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK) WHERE {columnName} {compareOperator} '{value}'", queryBuilder.GetQuery());
		}

		[TestMethod]
		[DataRow("USERS", "AGE", CompareTypes.Equal)]
		public void GenerateWhereStatementWithInvalidTypesTest(string tableName, string columnName, CompareTypes compareTypes)
		{
			var anonymousInvalidType = new { typeName = "Invalid Type" };

			var queryBuilder = new SQLQueryBuilder()
				.From(tableName)
				.Where(columnName, compareTypes, anonymousInvalidType);

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

			string dateGetQueryFormat = Utilities.FormatDateTime(new DateTime(year, month, day));
			string compareOperator = Utilities.GetEnumDescription<CompareTypes>(compareTypes);

			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK) WHERE {columnName} {compareOperator} '{dateGetQueryFormat}'", queryBuilder.GetQuery());
		}

		[TestMethod]
		public void Reset_Query_Should_Erase_Current_State_Of_The_Query()
		{
			var queryBuilder = new SQLQueryBuilder()
				.From("Builds")
				.Where("BuildCount", CompareTypes.GreaterThan, 100)
				.Where("BuildName", CompareTypes.NotEqual, "Test Build");
			queryBuilder.Reset();

			var newQueryBuilder = queryBuilder
				.From("USERS")
				.BuildSelect();

			Assert.AreEqual("SELECT * FROM USERS WITH(NOLOCK)", newQueryBuilder.GetQuery());
		}


		[TestMethod]
		[DataRow(10)]
		[DataRow(1)]
		[DataRow(1000)]
		[DataRow(230)]
		[DataRow(60)]
		[DataRow(6 )]
		public void Test_Top_Clause_Is_Generated_Correctly(int topClauseCount)
		{
			var queryBuilder = new SQLQueryBuilder()
				.Top(topClauseCount)
				.From("USERS")
				.BuildSelect();

			Assert.AreEqual($"SELECT TOP {topClauseCount} * FROM USERS WITH(NOLOCK)", queryBuilder.GetQuery());
		}

		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		public void Assert_That_Not_Built_Query_Throws_Exception(string tableName)
		{
			var queryBuilder = new SQLQueryBuilder()
			.From(tableName);

			Assert.Throws<NotBuiltQueryException>(() => queryBuilder.GetQuery());
		}

		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		public void Assert_That_Build_Select_Has_No_Affect_If_Called_Twice(string tableName)
		{
			var queryBuilder = new SQLQueryBuilder()
			.From(tableName)
			.BuildSelect();

			Assert.AreEqual(queryBuilder.GetQuery(), queryBuilder.BuildSelect().GetQuery());
		}

		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		public void Assert_That_Build_Insert_Has_No_Affect_If_Called_Twice(string tableName)
		{
			var unitTest = new UnitTest();

			var queryBuilder = new SQLQueryBuilder()
			.From(tableName)
			.BuildInsert<UnitTest>(unitTest);

			Assert.AreEqual(queryBuilder.GetQuery(), queryBuilder.BuildInsert<UnitTest>(unitTest).GetQuery());
		}

		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		public void Assert_That_Build_Update_Has_No_Affect_If_Called_Twice(string tableName)
		{
			var unitTest = new UnitTest();

			var queryBuilder = new SQLQueryBuilder()
			.From(tableName)
			.BuildUpdate<UnitTest>(unitTest);

			Assert.AreEqual(queryBuilder.GetQuery(), queryBuilder.BuildUpdate<UnitTest>(unitTest).GetQuery());
		}

		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		public void Assert_That_Build_Delete_Has_No_Affect_If_Called_Twice(string tableName)
		{
			var unitTest = new UnitTest();

			var queryBuilder = new SQLQueryBuilder()
			.From(tableName)
			.BuildDelete<UnitTest>(unitTest);

			Assert.AreEqual(queryBuilder.GetQuery(), queryBuilder.BuildDelete<UnitTest>(unitTest).GetQuery());
		}

		[TestMethod]
		public void Test_Build_Insert()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "INSERT TEST";
			unitTest.Guid = Guid.NewGuid();

			var queryBuilder = new SQLQueryBuilder()
				.Top()
				.Where()

			var query = queryBuilder.GetQuery();
			Assert.AreEqual($"INSERT INTO UNIT_TESTS (NAME, GUID) VALUES ('INSERT TEST', '{unitTest.Guid}')", query);
		}

		[TestMethod]
		public void Test_Build_Update()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "UPDATE TEST";
			unitTest.Guid = Guid.NewGuid();

			var queryBuilder = new SQLQueryBuilder()
				.From("UNIT_TESTS")
				.BuildUpdate<UnitTest>(unitTest);

			var query = queryBuilder.GetQuery();
			Assert.AreEqual($"UPDATE UNIT_TESTS SET NAME = 'UPDATE TEST' WHERE GUID = '{unitTest.Guid}'", query);
		}

		[TestMethod]
		public void Test_Build_Delete()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "DELETE TEST";
			unitTest.Guid = Guid.NewGuid();

			var queryBuilder = new SQLQueryBuilder()
				.From("UNIT_TESTS")
				.BuildDelete<UnitTest>(unitTest);

			var query = queryBuilder.GetQuery();
			Assert.AreEqual($"DELETE FROM UNIT_TESTS WHERE GUID = '{unitTest.Guid}'", query);
		}
	}
}
