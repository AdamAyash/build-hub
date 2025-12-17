
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
			Assert.IsTrue(uniTestTable.Insert(unitTest));
			Assert.IsTrue(contextTransaction.Commit());
		}
	}
}
