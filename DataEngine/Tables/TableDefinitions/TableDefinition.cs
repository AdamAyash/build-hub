using System.Reflection;

namespace BuildHub.DataEngine.Tables.TableDefinitions
{
	internal sealed class TableDefinition
	{
		internal class Item
		{
			public Item(string columnName, string propertyNameToMap)
			{
			}
		}

		public TableDefinition(string columnName, PropertyInfo propertyToBeMapped)
		{

		}
	}
}
