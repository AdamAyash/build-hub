using System.Reflection;

namespace BuildHub.DataEngine.Entities
{
	public sealed class ColumnMappingData
	{
		public ColumnInfo ColumnInfo { get; private set; }
		public PropertyInfo PropertyInfo { get; private set; }

		public ColumnMappingData(ColumnInfo columnInfo, PropertyInfo PropertyInfo)
		{
			this.ColumnInfo = columnInfo;
			this.PropertyInfo = PropertyInfo;
		}
	}
}