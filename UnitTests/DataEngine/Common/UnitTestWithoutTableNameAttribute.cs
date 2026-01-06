using BuildHub.DataEngine.Entities;
using BuildHub.DataEngine.Tables.Base;

namespace UnitTests.DataEngine.Common
{
	internal sealed class UnitTestWithoutTableNameAttributeTable : BaseTable<UnitTestWithoutTableNameAttribute>
	{
		public UnitTestWithoutTableNameAttributeTable()
		{
		}
	}

	internal class UnitTestWithoutTableNameAttribute : VersionedEntity
	{
		[ColumnInfo("NAME")]
		public string Name { get; set; }

		public UnitTestWithoutTableNameAttribute()
		{
			this.Name = string.Empty;
		}
	}
}
