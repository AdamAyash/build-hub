using BuildHub.Common.Configuration.Base;

namespace BuildHub.DataEngine.Configuration
{
	internal sealed class DatabaseConfiguration : IConfigurationModel
	{
		public DatabaseSource DatabaseSource { get; set; }
		public int MinPoolConnections { get; set; }
		public int MaxPoolConnections { get; set; }
		public int RetrieveConnectionRetryCount { get; set; }
		public int RetrieveConnectionTimeout { get; set; }
	}
}
