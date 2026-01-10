using BuildHub.DataEngine.Tables.Base;

namespace UnitTests.DataEngine.Common
{
	internal class ConcurrencyTestsTable : BaseTable<ConcurrencyTest>
	{
		public ConcurrencyTestsTable()
			: base(DatabaseSource.IntegrationTests)
		{
		}
	}
}
