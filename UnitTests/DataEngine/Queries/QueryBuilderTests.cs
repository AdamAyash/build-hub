#region
using BuildHub.Common.Utilities;
using BuildHub.DataEngine.Exceptions.Queries;
using BuildHub.DataEngine.Queries;
using UnitTests.DataEngineTests.Tables;
#endregion

namespace UnitTests.DataEngineTests.SQLQueries
{
	[TestClass]
	public class QueryBuilderTests
	{
		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		[DataRow("UNIT_TESTS")]
		public void Build_Select_Should_Generate_Correct_Simple_Query(string tableName)
		{
			var queryBuilder = new InternalQueryBuilder()
				.From(tableName)
				.BuildSelect();

			Assert.AreEqual($"SELECT * FROM {tableName} WITH(NOLOCK)", queryBuilder.GetQuery());
		}

		[TestMethod]
		[DataRow("USERS", LockTypes.None)]
		[DataRow("BUILDS", LockTypes.Update)]
		public void GenerateSimpleSelectStatementWithDifrentLockTypesTest(string tableName, LockTypes lockType)
		{
			var queryBuilder = new InternalQueryBuilder()
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
		public void Generate_Simple_Select_Statement_With_Different_Where_Statements_Only_Numbers_Test(string tableName, string columnName, CompareTypes compareTypes, object value)
		{
			var queryBuilder = new InternalQueryBuilder()
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
			var queryBuilder = new InternalQueryBuilder()
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

			var queryBuilder = new InternalQueryBuilder()
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
			var queryBuilder = new InternalQueryBuilder()
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
			var queryBuilder = new InternalQueryBuilder()
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
			var queryBuilder = new InternalQueryBuilder()
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
			var queryBuilder = new InternalQueryBuilder()
			.From(tableName);

			Assert.Throws<NotBuiltQueryException>(() => queryBuilder.GetQuery());
		}

		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		public void Assert_That_Build_Select_Has_No_Affect_If_Called_Twice(string tableName)
		{
			var queryBuilder = new InternalQueryBuilder()
			.From(tableName)
			.BuildSelect();

			Assert.AreEqual(queryBuilder.GetQuery(), queryBuilder.BuildSelect().GetQuery());
		}

		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		public void Assert_That_Build_Insert_Has_No_Affect_If_Called_Twice(string tableName)
		{
			var unitTest = new IntegrationTestEntity();

			var queryBuilder = new InternalQueryBuilder()
			.From(tableName)
			.BuildInsert<IntegrationTestEntity>(unitTest);

			Assert.AreEqual(queryBuilder.GetQuery(), queryBuilder.BuildInsert<IntegrationTestEntity>(unitTest).GetQuery());
		}

		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		public void Assert_That_Build_Update_Has_No_Affect_If_Called_Twice(string tableName)
		{
			var unitTest = new IntegrationTestEntity();

			var queryBuilder = new InternalQueryBuilder()
			.From(tableName)
			.BuildUpdate<IntegrationTestEntity>(unitTest);

			Assert.AreEqual(queryBuilder.GetQuery(), queryBuilder.BuildUpdate<IntegrationTestEntity>(unitTest).GetQuery());
		}

		[TestMethod]
		[DataRow("USERS")]
		[DataRow("BUILDS")]
		public void Assert_That_Build_Delete_Has_No_Affect_If_Called_Twice(string tableName)
		{
			var unitTest = new IntegrationTestEntity();

			var queryBuilder = new InternalQueryBuilder()
			.From(tableName)
			.BuildDelete<IntegrationTestEntity>(unitTest);

			Assert.AreEqual(queryBuilder.GetQuery(), queryBuilder.BuildDelete<IntegrationTestEntity>(unitTest).GetQuery());
		}

		[TestMethod]
		public void Test_Build_Insert()
		{
			var unitTest = new IntegrationTestEntity();
			unitTest.Name = "INSERT TEST";
			unitTest.Guid = Guid.NewGuid();

			var queryBuilder = new InternalQueryBuilder()
				.From("INTEGRATION_TESTS")
				.BuildInsert<IntegrationTestEntity>(unitTest);

			var query = queryBuilder.GetQuery();
			Assert.AreEqual($"INSERT INTO INTEGRATION_TESTS (NAME, GUID, VERSION, CREATED_AT, UPDATED_AT) " +
				$"VALUES ('INSERT TEST', '{unitTest.Guid}', 0, '{Utilities.FormatDateTime(unitTest.CreatedAt)}', '{Utilities.FormatDateTime(unitTest.UpdatedAt)}')", query);
		}

		[TestMethod]
		public void Test_Build_Update()
		{
			var unitTest = new IntegrationTestEntity();
			unitTest.Name = "UPDATE TEST";
			unitTest.Guid = Guid.NewGuid();

			var queryBuilder = new InternalQueryBuilder()
				.From("INTEGRATION_TESTS")
				.BuildUpdate<IntegrationTestEntity>(unitTest);

			var query = queryBuilder.GetQuery();
			Assert.AreEqual($"UPDATE INTEGRATION_TESTS SET NAME = '{unitTest.Name}'" +
				$", VERSION = 0, CREATED_AT = '{Utilities.FormatDateTime(unitTest.CreatedAt)}'," +
				$" UPDATED_AT = '{Utilities.FormatDateTime(unitTest.UpdatedAt)}' WHERE GUID = '{unitTest.Guid}'", query);
		}

		[TestMethod]
		public void Test_Build_Delete()
		{
			var unitTest = new IntegrationTestEntity();
			unitTest.Name = "DELETE TEST";
			unitTest.Guid = Guid.NewGuid();

			var queryBuilder = new InternalQueryBuilder()
				.From("UNIT_TESTS")
				.BuildDelete<IntegrationTestEntity>(unitTest);

			var query = queryBuilder.GetQuery();
			Assert.AreEqual($"DELETE FROM UNIT_TESTS WHERE GUID = '{unitTest.Guid}'", query);
		}

		[TestMethod]
		public void Client_SQL_Builder_Should_Transfer_Its_State_To_Internal_Builder()
		{
			var sqlQueryBuider = new QueryBuilder()
			.Where("BUILD_COUNT", CompareTypes.GreaterThan, 100)
			.Where("BUILD_NAME", CompareTypes.NotEqual, "Test Build");

			var queryBuilder = new InternalQueryBuilder(sqlQueryBuider)
				.From("BUILDS")
				.BuildSelect();

			Assert.AreEqual("SELECT * FROM BUILDS WITH(NOLOCK) WHERE BUILD_COUNT > 100 AND BUILD_NAME <> 'Test Build'", queryBuilder.GetQuery());
		}
	}
}
