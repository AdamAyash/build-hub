using System.Reflection;

namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Class representing all data necessary for mapping to a specific Entity
	/// </summary>
	public sealed record class ColumnMappingData
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