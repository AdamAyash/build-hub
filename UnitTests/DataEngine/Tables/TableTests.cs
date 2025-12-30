using BuildHub.DataEngine.Exceptions;
using BuildHub.DataEngine.Transactions;
using Microsoft.CodeAnalysis;
using Microsoft.Data.SqlClient;

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
		public void InsertUnitTest()
		{
			using var transaction = new ScopedTransaction();

			var unitTest = new UnitTest();
			unitTest.Name = "Insert Test";

			var unitTestTable = new UnitTestsTable();
			unitTestTable.Insert(unitTest);

			var dbUnitTest = unitTestTable.GetByGuid(unitTest.Guid);
			Assert.AreEqual(unitTest.Guid, dbUnitTest.Guid);
		}

		[TestMethod]
		public void InsertDuplicateUnitTest()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Insert Test";

			var unitTestTable = new UnitTestsTable();
			unitTestTable.Insert(unitTest);
			Assert.Throws<SqlException>( () => unitTestTable.Insert(unitTest));
		}

		[TestMethod]
		public void UpdateUnitTest()
		{
			var unitTest = new UnitTest();
			unitTest.Name = "Insert Test";

			var unitTestTable = new UnitTestsTable();
			unitTestTable.Insert(unitTest);


			unitTest.Name = "UPDATED UNIT TEST";
			unitTestTable.Update(unitTest);

		}
	}
}
