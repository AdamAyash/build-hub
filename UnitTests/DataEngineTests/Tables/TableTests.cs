using BuildHub.DataEngine.Exceptions;

namespace UnitTests.DataEngineTests.Tables
{
	[TestClass]
	public class TableTests
	{
		[TestMethod]
		public void ConstructTableTest()
		{
			var unitTestTable = new UnitTestsTable();
			Assert.IsNotNull(unitTestTable);
		}

		[TestMethod]
		public void GetAllUnitTestsTest()
		{
			var unitTestTable = new UnitTestsTable();
			var uniTests = unitTestTable.GetAll();

			Assert.IsNotNull(uniTests);
		}

		[TestMethod]
		public void GetAllUnitTestsWithUnmappedPropertyTest()
		{
			var unitTestTable = new UnitTestsWithUnmappedFieldTable();
			Assert.Throws<MissingColumnDescriptionException>(() => unitTestTable.GetAll());
		}

		[TestMethod]
		[DataRow("5DC46FBF-5522-4D76-87D7-A148A4A0B419")]
		public void GetByGuidTest(string guid)
		{
			var unitTestTable = new UnitTestsTable();
			var unitTest = unitTestTable.GetByGuid(Guid.Parse(guid));

			Assert.IsNotNull(unitTest);
			Assert.IsGreaterThan(0, unitTest.Id);		
		}

		[TestMethod]
		[DataRow("5DC46FBF-5522-4D76-87D7-A148A4A0B418")]
		public void GetNotExistinUnitTestgByGuidTest(string guid)
		{
			var unitTestTable = new UnitTestsTable();
			var unitTest = unitTestTable.GetByGuid(Guid.Parse(guid));

			Assert.IsNull(unitTest);
		}

		[TestMethod]
		public void InsertUnitTest()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Insert Test";

			var unitTestTable = new UnitTestsTable();
			Assert.IsTrue(unitTestTable.Insert(unitTest));
		}

		[TestMethod]
		public void InsertDuplicateUnitTest()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Insert Test";

			var unitTestTable = new UnitTestsTable();
			Assert.IsTrue(unitTestTable.Insert(unitTest));
			Assert.IsFalse(unitTestTable.Insert(unitTest));
		}
	}
}
