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
	}
}
