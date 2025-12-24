
using BuildHub.DataEngine.Transactions;
using UnitTests.DataEngineTests.Tables;

namespace UnitTests.DataEngineTests.Transactions
{
	[TestClass]
	public class TransactionsTests
	{
		[TestMethod]
		public void ScopedStransactionTest()
		{
			using var contextTransaction = new ScopedTransaction();

			var unitTest = new UnitTest();
			unitTest.Name = "Transaction Test";
			 
			var uniTestTable = new UnitTestsTable();
			uniTestTable.Insert(unitTest);
			var dbEntiy = uniTestTable.GetByCondition(unitTest => unitTest.Guid).First();

			Assert.IsTrue(contextTransaction.Commit());
			Assert.AreEqual(unitTest, dbEntiy);
		}

		[TestMethod]
		public void RollbackTest()
		{
			using var contextTransaction = new ScopedTransaction();

			var unitTest = new UnitTest();
			unitTest.Name = "Transaction Test";
			unitTest.Guid = Guid.NewGuid();

			var uniTestTable = new UnitTestsTable();
			uniTestTable.Insert(unitTest);
			Assert.IsTrue(contextTransaction.Rollback());

			Assert.IsNull(uniTestTable.GetByCondition(unitTest => unitTest.Guid).First());
		}
	}
}
