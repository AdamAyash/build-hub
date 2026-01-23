using BuildHub.DataEngine.DatabaseConnection;
using BuildHub.DataEngine.Exceptions.Entities;
using BuildHub.DataEngine.Queries;
using BuildHub.DataEngine.Transactions;
using UnitTests.DataEngine.Common;

namespace UnitTests.DataEngineTests.Tables
{
	[TestClass]
	[TestCategory("Integration")]
	public class IntegrationTestsTableTests
	{
		public TestContext TestContext { get; set; }

		[ClassCleanup]
		public static void Cleanup()
		{
			using var scopedTransaction = new ScopedTransaction(DatabaseSource.IntegrationTests);

			var integrationTestsTable = new IntegrationTestsTable();
			var allIntegrationTests = integrationTestsTable.GetAll();

			foreach (var test in allIntegrationTests)
				integrationTestsTable.Delete(test);

			scopedTransaction.Commit();
		}

		[TestMethod]
		public void Construct_Table_Should_Not_Be_Null()
		{
			var integrationTestsTable = new IntegrationTestsTable();
			Assert.IsNotNull(integrationTestsTable);
		}

		[TestMethod]
		public void Get_All_Unit_Test_Should_Not_Be_Null()
		{
			var integrationTestsTable = new IntegrationTestsTable();
			var uniTests = integrationTestsTable.GetAll();

			Assert.IsNotNull(uniTests);
		}

		[TestMethod]
		public void Assert_Get_By_Guid_Returns_Null_If_Entity_Does_Not_Exist()
		{
			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsNull(integrationTestTable.GetByGuid(Guid.NewGuid()));
		}

		[TestMethod]
		public void Get_Unit_Test_By_Guid()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			Assert.IsGreaterThan(0, integrationTestTable.GetByGuid(integrationTest.Guid).Id);
		}

		[TestMethod]
		public void Get_All_Unit_Test_With_Unmapped_Fields_Should_Throw_Exception()
		{
			var integrationTestTable = new InegrationTestWithUnmappedFieldTable();
			Assert.Throws<MissingColumnDescriptionException>(() => integrationTestTable.GetAll());
		}

		[TestMethod]
		public void Get_By_Unit_Test_By_Condition()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;
			var integrationTestTable = new IntegrationTestsTable();

			Assert.IsTrue(integrationTestTable.Insert(integrationTest));
			Assert.IsNotNull(integrationTestTable.GetByCondition(integrationTest, (integrationTest) => integrationTest.Id));
		}

		[TestMethod]
		public void Get_By_Unit_Test_By_Condition_With_Query_Builder()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;
			var integrationTestTable = new IntegrationTestsTable();

			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			QueryBuilder queryBuilder = new QueryBuilder()
				.Where(integrationTest, (integrationTest) => integrationTest.Guid);

			Assert.IsNotNull(integrationTestTable.GetByCondition(queryBuilder));
		}

		[TestMethod]
		public void Assert_Insert_Unit_Test()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			var dbUnitTest = integrationTestTable.GetByGuid(integrationTest.Guid);
			Assert.AreEqual(integrationTest.Guid, dbUnitTest.Guid);
		}

		[TestMethod]
		public void Assert_That_Inserting_Duplicate_Unit_Test_Should_Throw()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));
			Assert.IsFalse(integrationTestTable.Insert(integrationTest));
		}

		[TestMethod]
		public void Assert_Update_Unit_Test_Is_True()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			integrationTest.Name = this.TestContext.TestName.ToLower();
			Assert.IsTrue(integrationTestTable.Update(integrationTest));
		}

		[TestMethod]
		public void Assert_Delete_Unit_Test_Is_True()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			integrationTest.Name = this.TestContext.TestName;
			Assert.IsTrue(integrationTestTable.Delete(integrationTest));
		}

		[TestMethod]
		public void Deleting_An_Existing_Unit_Test_Should_Return_False()
		{
			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			integrationTest.Name = this.TestContext.TestName;
			Assert.IsTrue(integrationTestTable.Delete(integrationTest));

			Assert.IsFalse(integrationTestTable.Delete(integrationTest));
		}

		[TestMethod]
		public void Assert_That_Unit_Test_Entity_Without_Table_Name_attribute_Throws()
		{
			Assert.Throws<MissingTableNameException>(() => new IntegrationTestWithoutTableNameAttributeTable());
		}
	}
}
