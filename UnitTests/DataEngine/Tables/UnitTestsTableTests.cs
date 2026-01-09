using BuildHub.DataEngine.Exceptions.Entities;
using BuildHub.DataEngine.Queries;
using UnitTests.DataEngine.Common;

namespace UnitTests.DataEngineTests.Tables
{
	[TestClass]
	[TestCategory("Integration")]
	public class UnitTestsTableTests
	{
		[TestMethod]
		public void Construct_Table_Should_Not_Be_Null()
		{
			var unitTestTable = new UnitTestsTable();
			Assert.IsNotNull(unitTestTable);
		}

		[TestMethod]
		public void Get_All_Unit_Test_Should_Not_Be_Null()
		{
			var unitTestTable = new UnitTestsTable();
			var uniTests = unitTestTable.GetAll();

			Assert.IsNotNull(uniTests); 
		}

		[TestMethod]
		public void Assert_Get_By_Guid_Returns_Null_If_Entity_Does_Not_Exist()
		{
			var unitTestTable = new UnitTestsTable();
			Assert.IsNull(unitTestTable.GetByGuid(Guid.NewGuid()));
		}

		[TestMethod]
		public void Get_Unit_Test_By_Guid()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Get by GUID Test";

			var unitTestTable = new UnitTestsTable();
			Assert.IsTrue(unitTestTable.Insert(unitTest));

			Assert.IsGreaterThan(0, unitTestTable.GetByGuid(unitTest.Guid).Id);
		}

		[TestMethod]
		public void Get_All_Unit_Test_With_Unmapped_Fields_Should_Throw_Exception()
		{
			var unitTestTable = new UnitTestsWithUnmappedFieldTable();
			Assert.Throws<MissingColumnDescriptionException>(() => unitTestTable.GetAll());
		}

		[TestMethod]
		public void Get_By_Unit_Test_By_Condition()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Get by GUID Test";
			var unitTestTable = new UnitTestsTable();

			Assert.IsTrue(unitTestTable.Insert(unitTest));
			Assert.IsNotNull(unitTestTable.GetByCondition(unitTest, (unitTest) => unitTest.Id));
		}

		[TestMethod]
		public void Get_By_Unit_Test_By_Condition_With_Query_Builder()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Get by GUID Test";
			var unitTestTable = new UnitTestsTable();

			Assert.IsTrue(unitTestTable.Insert(unitTest));

			QueryBuilder queryBuilder = new QueryBuilder()
				.Where(unitTest, (unitTest) => unitTest.Guid);

			Assert.IsNotNull(unitTestTable.GetByCondition(queryBuilder));
		}

		[TestMethod]
		public void Assert_Insert_Unit_Test()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Insert Test";

			var unitTestTable = new UnitTestsTable();
			Assert.IsTrue(unitTestTable.Insert(unitTest));

			var dbUnitTest = unitTestTable.GetByGuid(unitTest.Guid);
			Assert.AreEqual(unitTest.Guid, dbUnitTest.Guid);
		}

		[TestMethod]
		public void Assert_That_Inserting_Duplicate_Unit_Test_Should_Throw()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Insert Test";

			var unitTestTable = new UnitTestsTable();
			Assert.IsTrue(unitTestTable.Insert(unitTest));
			Assert.IsFalse(unitTestTable.Insert(unitTest));
		}

		[TestMethod]
		public void Assert_Update_Unit_Test_Is_True()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Insert Test";

			var unitTestTable = new UnitTestsTable();
			Assert.IsTrue(unitTestTable.Insert(unitTest));

			unitTest.Name = "UPDATED UNIT TEST";
			Assert.IsTrue(unitTestTable.Update(unitTest));
		}

		[TestMethod]
		public void Assert_Delete_Unit_Test_Is_True()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Insert Test";

			var unitTestTable = new UnitTestsTable();
			Assert.IsTrue(unitTestTable.Insert(unitTest));

			unitTest.Name = "UPDATED UNIT TEST";
			Assert.IsTrue(unitTestTable.Delete(unitTest));
		}

		[TestMethod]
		public void Deleting_An_Existing_Unit_Test_Should_Return_True()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Insert Test";

			var unitTestTable = new UnitTestsTable();
			Assert.IsTrue(unitTestTable.Insert(unitTest));

			unitTest.Name = "UPDATED UNIT TEST";
			Assert.IsTrue(unitTestTable.Delete(unitTest));

			Assert.IsTrue(unitTestTable.Delete(unitTest));
		}

		[TestMethod]
		public void Assert_That_Unit_Test_Entity_Without_Table_Name_attribute_Throws()
		{
			Assert.Throws<MissingTableNameException>(() => new UnitTestWithoutTableNameAttributeTable());
		}
	}
}
