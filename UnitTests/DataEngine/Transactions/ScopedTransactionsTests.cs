using BuildHub.DataEngine.Transactions;
using UnitTests.DataEngine.Common;
using UnitTests.DataEngineTests.Tables;

namespace UnitTests.DataEngine.Transactions
{
	[TestClass]
	[TestCategory("Integration")]
	public class ScopedTransactionsTests
	{
		public TestContext TestContext { get; set; }

		[ClassCleanup]
		public static void Cleanup()
		{
			using ScopedTransaction scopedTransaction = new ScopedTransaction(DatabaseSource.IntegrationTests);

			var integrationTestsTable = new IntegrationTestsTable();
			var allIntegrationTests = integrationTestsTable.GetAll();

			foreach (var test in allIntegrationTests)
				integrationTestsTable.Delete(test);

			scopedTransaction.Commit();
		}

			[TestMethod]
		public void Assert_Commit_Returns_True()
		{
			using var transaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
			var IntegrationTestsTable = new IntegrationTestsTable();

			var unitTest = new IntegrationTestEntity();
			unitTest.Name = "Insert with transaction";

			Assert.IsTrue(IntegrationTestsTable.Insert(unitTest));
			Assert.IsTrue(transaction.Commit());
		}

		[TestMethod]
		public void Assert_That_Rollback_Rollbacks_The_Inserted_Unit_Test()
		{
			using var transaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
			var unitTestTable = new IntegrationTestsTable();

			var unitTest = new IntegrationTestEntity();
			unitTest.Name = "Insert with transaction"; 

			Assert.IsTrue(unitTestTable.Insert(unitTest));
			Assert.IsNotNull(unitTestTable.GetByGuid(unitTest.Guid));

			Assert.IsTrue(transaction.Rollback());
			Assert.IsNull(unitTestTable.GetByGuid(unitTest.Guid));
		}

		[TestMethod]
		public void Assert_That_The_Transaction_Will_Be_Disposed_When_Out_Of_Scope()
		{
			var unitTestTable = new IntegrationTestsTable();
			var unitTest = new IntegrationTestEntity();

			{
				unitTest.Name = "Transaction out of scope test";

				using var scopedTransaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
				Assert.IsTrue(unitTestTable.Insert(unitTest));
			}

			Assert.IsNull(unitTestTable.GetByGuid(unitTest.Guid));
		}

		[TestMethod]
		public void Calling_Rollback_Twice_Reurns_False()
		{
			using var transaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
			var unitTestTable = new IntegrationTestsTable();

			var unitTest = new IntegrationTestEntity();
			unitTest.Name = "Insert with transaction";

			Assert.IsTrue(unitTestTable.Insert(unitTest));
			Assert.IsNotNull(unitTestTable.GetByGuid(unitTest.Guid));

			Assert.IsTrue(transaction.Rollback());
			Assert.IsFalse(transaction.Rollback());
		}

		[TestMethod]
		public void Calling_Commit_Twice_Reurns_False()
		{
			using var transaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
			var unitTestTable = new IntegrationTestsTable();

			var unitTest = new IntegrationTestEntity();
			unitTest.Name = "Insert with transaction";

			Assert.IsTrue(unitTestTable.Insert(unitTest));
			Assert.IsNotNull(unitTestTable.GetByGuid(unitTest.Guid));

			Assert.IsTrue(transaction.Commit());
			Assert.IsFalse(transaction.Commit());
		}

		[TestMethod]
		public void Assert_That_Scoped_Transaction_Commit_Works_For_More_Than_One_Tables()
		{
			using var transaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
			var unitTestTable = new IntegrationTestsTable();

			var unitTest = new IntegrationTestEntity();
			unitTest.Name = "Insert with transaction";

			Assert.IsTrue(unitTestTable.Insert(unitTest));

			var concurrencyTable = new ConcurrencyTestsTable();
			var concurrencyTest = new ConcurrencyTesteEntity();
			concurrencyTest.Name = "ConcurrencyTest";

			Assert.IsTrue(concurrencyTable.Insert(concurrencyTest));
			Assert.IsTrue(transaction.Commit());
		}

		[TestMethod]
		public void Assert_That_Scoped_Transaction_Rollbacks_For_More_Than_One_Tables()
		{
			using var transaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
			var unitTestTable = new IntegrationTestsTable();

			var unitTest = new IntegrationTestEntity();
			unitTest.Name = "Insert with transaction";

			Assert.IsTrue(unitTestTable.Insert(unitTest));

			var concurrencyTable = new ConcurrencyTestsTable();
			var concurrencyTest = new ConcurrencyTesteEntity();
			concurrencyTest.Name = "ConcurrencyTest";

			Assert.IsTrue(concurrencyTable.Insert(concurrencyTest));
			Assert.IsTrue(transaction.Rollback());

			Assert.IsNull(unitTestTable.GetByGuid(unitTest.Guid));
			Assert.IsNull(concurrencyTable.GetByGuid(concurrencyTest.Guid));
		}
	}
}
