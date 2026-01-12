using BuildHub.DataEngine.Tables.Base;
using UnitTests.DataEngineTests.Tables;

namespace UnitTests.DataEngine.Common
{
	internal sealed class IntegrationTestsTable : BaseTable<IntegrationTestEntity>
	{
		public IntegrationTestsTable()
			: base(DatabaseSource.IntegrationTests)
		{
		}
	}
}
