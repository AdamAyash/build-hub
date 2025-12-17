using System.Reflection;

namespace BuildHub.DataEngine.Entities
{
	public sealed class ColumnMappingData
	{
		public ColumnDescription ColumnDescription { get; private set; }
		public PropertyInfo PropertyInfo { get; private set; }

		public ColumnMappingData(ColumnDescription columnDescription, PropertyInfo PropertyInfo)
		{
			this.ColumnDescription = columnDescription;
			this.PropertyInfo = PropertyInfo;
		}
	}
}