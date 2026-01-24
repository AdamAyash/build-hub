using BuildHub.Common.Configuration.Base;
using BuildHub.DataEngine.DatabaseConnection;

namespace BuildHub.DataEngine.Configuration
{
	internal sealed record class DatabaseConfiguration : IConfigurationModel
	{
		public DatabaseSource DatabaseSource { get; set; }
		public int MinPoolConnections { get; set; }
		public int MaxPoolConnections { get; set; }
		public int RetrieveConnectionRetryCount { get; set; }
		public int RetrieveConnectionTimeout { get; set; }
	}
}
