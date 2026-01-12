using BuildHub.DataEngine.Tables.Base;

namespace UnitTests.DataEngine.Common
{
	internal class ConcurrencyTestsTable : BaseTable<ConcurrencyTesteEntity>
	{
		public ConcurrencyTestsTable()
			: base(DatabaseSource.IntegrationTests)
		{
		}
	}
}
