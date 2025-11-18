using BuildHub.Common.ConfigurationManager.Base;

namespace BuildHub.DataEngine.DatabaseConnection
{
	internal sealed class DatabaseConfiguration : IConfigurationModel
	{
		public DatabaseSource DatabaseSource { get; set; }
		public int MinPoolConnections { get; set; }
		public int MaxPoolConnections { get; set; }
		public DatabaseConfiguration()
		{
		}
	}
}
