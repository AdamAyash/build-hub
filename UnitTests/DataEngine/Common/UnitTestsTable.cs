using BuildHub.DataEngine.Tables.Base;
using UnitTests.DataEngineTests.Tables;

namespace UnitTests.DataEngine.Common
{
	internal sealed class UnitTestsTable : BaseTable<UnitTest>
	{
		public UnitTestsTable()
			: base(DatabaseSource.IntegrationTests)
		{
		}
	}
}
