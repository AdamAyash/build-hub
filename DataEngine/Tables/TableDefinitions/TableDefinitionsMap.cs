namespace BuildHub.DataEngine.Tables.TableDefinitions
{
	internal sealed class TableDefinitionsMap
	{
		private static readonly Lazy<TableDefinitionsMap> _tableDefinitionsMapInstance 
			= new Lazy<TableDefinitionsMap>(() => new TableDefinitionsMap());

		private static Dictionary<string, TableDefinition>? _tableDefinitionsMap = null;

		private TableDefinitionsMap()
		{
			_tableDefinitionsMap = new Dictionary<string, TableDefinition>();
		}

		public TableDefinitionsMap GetInstance()
		{
			return _tableDefinitionsMapInstance.Value;
		}
	}
}
