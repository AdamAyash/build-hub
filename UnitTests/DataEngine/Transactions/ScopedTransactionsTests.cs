using BuildHub.DataEngine.Transactions;
using UnitTests.DataEngine.Common;
using UnitTests.DataEngineTests.Tables;

namespace UnitTests.DataEngine.Transactions
{
	[TestClass]
	public class ScopedTransactionsTests
	{
		[TestMethod]
		public void Assert_Commit_Returns_True()
		{
			using var transaction = new ScopedTransaction();
			var unitTestTable = new UnitTestsTable();

			var unitTest = new UnitTest();
			unitTest.Name = "Insert with transaction";

			Assert.IsTrue(unitTestTable.Insert(unitTest));
			Assert.IsTrue(transaction.Commit());
		}

		[TestMethod]
		public void Assert_That_Rollback_Rollbacks_The_Inserted_Unit_Test()
		{
			using var transaction = new ScopedTransaction();
			var unitTestTable = new UnitTestsTable();

			var unitTest = new UnitTest();
			unitTest.Name = "Insert with transaction"; 

			Assert.IsTrue(unitTestTable.Insert(unitTest));
			Assert.IsNotNull(unitTestTable.GetByGuid(unitTest.Guid));

			Assert.IsTrue(transaction.Rollback());
			Assert.IsNull(unitTestTable.GetByGuid(unitTest.Guid));
		}

		[TestMethod]
		public void Assert_That_The_Transaction_Will_Be_Disposed_When_Out_Of_Scope()
		{
			var unitTestTable = new UnitTestsTable();
			var unitTest = new UnitTest();

			{
				unitTest.Name = "Transaction out of scope test";

				using var scopedTransaction = new ScopedTransaction();
				Assert.IsTrue(unitTestTable.Insert(unitTest));
			}

			Assert.IsNull(unitTestTable.GetByGuid(unitTest.Guid));
		}

		[TestMethod]
		public void Calling_Rollback_Twice_Reurns_False()
		{
			using var transaction = new ScopedTransaction();
			var unitTestTable = new UnitTestsTable();

			var unitTest = new UnitTest();
			unitTest.Name = "Insert with transaction";

			Assert.IsTrue(unitTestTable.Insert(unitTest));
			Assert.IsNotNull(unitTestTable.GetByGuid(unitTest.Guid));

			Assert.IsTrue(transaction.Rollback());
			Assert.IsFalse(transaction.Rollback());
		}

		[TestMethod]
		public void Calling_Commit_Twice_Reurns_False()
		{
			using var transaction = new ScopedTransaction();
			var unitTestTable = new UnitTestsTable();

			var unitTest = new UnitTest();
			unitTest.Name = "Insert with transaction";

			Assert.IsTrue(unitTestTable.Insert(unitTest));
			Assert.IsNotNull(unitTestTable.GetByGuid(unitTest.Guid));

			Assert.IsTrue(transaction.Commit());
			Assert.IsFalse(transaction.Commit());
		}
	}
}
