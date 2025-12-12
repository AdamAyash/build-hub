namespace BuildHub.DataEngine.Entities
{
	[AttributeUsage(AttributeTargets.Property)]
	internal class ColumnDescription : Attribute
	{
		private readonly string _columnName;
		private readonly int _size;

		public string ColumnName => this._columnName;
		public int Size => this._size;

		public ColumnDescription(string columnName, int size = 0)
		{
			this._columnName = columnName;
			this._size = size;
		}
	}
}
