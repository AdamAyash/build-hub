using BuildHub.DataEngine.Tables.Base;

namespace UnitTests.DataEngineTests.Tables
{
	internal sealed class UnitTestsTable : BaseTable<UnitTest>
	{
		public UnitTestsTable()
			: base("UNIT_TESTS", DatabaseSource.Core)
		{
		}
	}
}
