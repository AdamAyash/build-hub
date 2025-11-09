using BuildHubCommon.ConfigurationManager.Base;

namespace BuildHubDataEngine.DatabaseConnection
{
    internal sealed class DatabaseSettingsModel : IConfigurationModel
    {
        public DatabaseSources DatabaseSource { get; set; }
        public int MinPoolConnections { get; set; }
        public int MaxPoolConnections { get; set; }
        public DatabaseSettingsModel()
        {
        }
    }
}
