using BuildHub.DataEngine.Queries;
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

			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = "Insert with transaction";

			Assert.IsTrue(IntegrationTestsTable.Insert(integrationTest));
			Assert.IsTrue(transaction.Commit());
		}

		[TestMethod]
		public void Assert_That_Rollback_Rollbacks_The_Inserted_Unit_Test()
		{
			using var transaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
			var integrationTestTable = new IntegrationTestsTable();

			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = "Insert with transaction"; 

			Assert.IsTrue(integrationTestTable.Insert(integrationTest));
			Assert.IsNotNull(integrationTestTable.GetByGuid(integrationTest.Guid));

			Assert.IsTrue(transaction.Rollback());
			Assert.IsNull(integrationTestTable.GetByGuid(integrationTest.Guid));
		}

		[TestMethod]
		public void Assert_That_The_Transaction_Will_Be_Disposed_When_Out_Of_Scope()
		{
			var integrationTestTable = new IntegrationTestsTable();
			var integrationTest = new IntegrationTestEntity();

			{
				integrationTest.Name = "Transaction out of scope test";

				using var scopedTransaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
				Assert.IsTrue(integrationTestTable.Insert(integrationTest));
			}

			Assert.IsNull(integrationTestTable.GetByGuid(integrationTest.Guid));
		}

		[TestMethod]
		public void Calling_Rollback_Twice_Reurns_False()
		{
			using var transaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
			var integrationTestTable = new IntegrationTestsTable();

			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = "Insert with transaction";

			Assert.IsTrue(integrationTestTable.Insert(integrationTest));
			Assert.IsNotNull(integrationTestTable.GetByGuid(integrationTest.Guid));

			Assert.IsTrue(transaction.Rollback());
			Assert.IsFalse(transaction.Rollback());
		}

		[TestMethod]
		public void Calling_Commit_Twice_Reurns_False()
		{
			using var transaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
			var integrationTestTable = new IntegrationTestsTable();

			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = "Insert with transaction";

			Assert.IsTrue(integrationTestTable.Insert(integrationTest));
			Assert.IsNotNull(integrationTestTable.GetByGuid(integrationTest.Guid));

			Assert.IsTrue(transaction.Commit());
			Assert.IsFalse(transaction.Commit());
		}

		[TestMethod]
		public void Assert_That_Scoped_Transaction_Commit_Works_For_More_Than_One_Tables()
		{
			using var transaction = new ScopedTransaction(DatabaseSource.IntegrationTests);
			var integrationTestTable = new IntegrationTestsTable();

			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = "Insert with transaction";

			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

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
			var integrationTestTable = new IntegrationTestsTable();

			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = "Insert with transaction";

			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			var concurrencyTable = new ConcurrencyTestsTable();
			var concurrencyTest = new ConcurrencyTesteEntity();
			concurrencyTest.Name = "ConcurrencyTest";

			Assert.IsTrue(concurrencyTable.Insert(concurrencyTest));
			Assert.IsTrue(transaction.Rollback());

			Assert.IsNull(integrationTestTable.GetByGuid(integrationTest.Guid));
			Assert.IsNull(concurrencyTable.GetByGuid(concurrencyTest.Guid));
		}

		[TestMethod]
		public void Assert_Update_Locked_Resource_Is_True()
		{
			using ScopedTransaction scopedTransaction = new ScopedTransaction(DatabaseSource.IntegrationTests);

			var integrationTest = new IntegrationTestEntity();
			integrationTest.Name = this.TestContext.TestName;

			var integrationTestTable = new IntegrationTestsTable();
			Assert.IsTrue(integrationTestTable.Insert(integrationTest));

			QueryBuilder queryBuilder = new QueryBuilder()
				.Where<IntegrationTestEntity>(integrationTest, integrationTest => integrationTest.Guid)
				.Lock(LockTypes.Update);

			var sameIntegrationTest = integrationTestTable.GetByCondition(queryBuilder).FirstOrDefault();

			Assert.IsFalse(integrationTestTable.Update(sameIntegrationTest));
		}
	}
}
