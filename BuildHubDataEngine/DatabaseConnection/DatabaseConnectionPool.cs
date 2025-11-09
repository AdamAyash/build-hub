using BuildHubCommon.ConfigurationManager;
using BuildHubCommon.Utilities;

namespace BuildHubDataEngine.DatabaseConnection
{
    /// <summary>
    /// Database connection pool singleton, initializing and managing a number of database connections.
    /// </summary>
    public sealed class DatabaseConnectionPool
    {
        /// <summary>Singleton instance to the connection pool</summary>
        private static DatabaseConnectionPool? _databaseConnectionPoolInstance = null;
        /// <summary>ConfigurationManager</summary>
        private readonly ConfigurationManager _configurationManager;

        /// <summary>Available connection ready for use</summary>
        private readonly Dictionary<DatabaseSource, List<DatabaseConnection>> _availableDatabaseConnectionsMap;
        /// <summary>Currently used connections</summary>
        private readonly Dictionary<DatabaseSource, List<DatabaseConnection>> _currentlyUsedDatabaseConnectionsMap;

        private DatabaseConnectionPool()
        {
            this._availableDatabaseConnectionsMap = new Dictionary<DatabaseSource, List<DatabaseConnection>>();
            this._currentlyUsedDatabaseConnectionsMap = new Dictionary<DatabaseSource, List<DatabaseConnection>>();

            this._configurationManager = ConfigurationManager.GetConfigurationManager();

            Initialize();
        }

        ~DatabaseConnectionPool()
        {
            Cleanup();
        }

        /// <summary>
        /// Returns an instance to the connection pool
        /// </summary>
        /// <returns>DatabaseConnectionPool</returns>
        public static DatabaseConnectionPool GetInstance()
        {
            if (_databaseConnectionPoolInstance == null)
                _databaseConnectionPoolInstance = new DatabaseConnectionPool();

            return _databaseConnectionPoolInstance;
        }

        /// <summary>
        /// Returns a reference to a database connection
        /// </summary>
        /// <returns>DatabaseConnection</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public DatabaseConnection GetDatabaseConnection(DatabaseSource databaseSource)
        {
            List<DatabaseConnection> availableDatabaseConnections = this._availableDatabaseConnectionsMap[databaseSource];
            List<DatabaseConnection> currentlyUsedDatabaseConnections = this._currentlyUsedDatabaseConnectionsMap[databaseSource];

            var databaseConnection = availableDatabaseConnections.FirstOrDefault();

            if (databaseConnection == null)
                databaseConnection = InitializeConnection(databaseSource);

            var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);
            if (!databaseConnectionValidator.TestDatabaseConnection())
                throw new InvalidOperationException();

            currentlyUsedDatabaseConnections.Add(databaseConnection);
            availableDatabaseConnections.Remove(databaseConnection);

            return databaseConnection;
        }

        /// <summary>
        /// Returns a database connection to the pool
        /// </summary>
        /// <param name="databaseConnection"></param>
        public void ReleaseDatabaseConnection(DatabaseConnection databaseConnection)
        {
            var databaseSource = databaseConnection.DatabaseSource;

            this._currentlyUsedDatabaseConnectionsMap[databaseSource].Remove(databaseConnection);
            this._availableDatabaseConnectionsMap[databaseSource].Add(databaseConnection);
        }

        private DatabaseConnection InitializeConnection(DatabaseSource databaseSource)
        {
            string connectionStringKey = Utilities.GetEnumDescription<DatabaseSource>(databaseSource);
            string? connectionString = this._configurationManager.GetConnectionString(connectionStringKey);

            if (connectionString is null)
                throw new Exception();

            var databaseConnection = new DatabaseConnection(databaseSource, connectionString);

            try
            {
                databaseConnection.OpenConnection();
            }
            catch (Exception)
            {
                //LOG ERROR
            }

            var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);
            if (!databaseConnectionValidator.TestDatabaseConnection())
                throw new InvalidOperationException();

            return databaseConnection;
        }

       /// <summary>
       /// Initiliazes connection baed on the configuration provided
       /// </summary>
       /// <param name="databaseConfiguration">Datbase configuration model</param>
       /// <returns></returns>
       /// <exception cref="Exception"></exception>
       /// <exception cref="InvalidOperationException"></exception>
        private void InitializeConnections(DatabaseConfiguration databaseConfiguration)
        {
            var availableDatabaseConnections = new List<DatabaseConnection>();

            for(var index = 0; index < databaseConfiguration.MaxPoolConnections; index++)
            {
               DatabaseConnection databaseConnection = InitializeConnection(databaseConfiguration.DatabaseSource);
               availableDatabaseConnections.Add(databaseConnection);
            }

            this._availableDatabaseConnectionsMap.Add(databaseConfiguration.DatabaseSource, availableDatabaseConnections);
        }

        /// <summary>
        /// Initializes a number of database connections
        /// </summary>
        private void Initialize()
        {
            var databaseConfigurations = _configurationManager.GetConfigurationModels<DatabaseConfiguration>("DatabaseConfigurations");

            if (databaseConfigurations is null)
            {
                //LOG error 
                throw new Exception();
            }

            databaseConfigurations = databaseConfigurations.DistinctBy(x => x.DatabaseSource);

            foreach (var databaseConfiguration in databaseConfigurations)
            {
                InitializeConnections(databaseConfiguration);
            }
        }

        /// <summary>
        /// Closes all connections
        /// </summary>
        private void Cleanup()
        {
            foreach(var databaseConnections in this._availableDatabaseConnectionsMap.Values)
            {
                foreach (DatabaseConnection databaseConnection in databaseConnections)
                    databaseConnection.CloseConnection();

                databaseConnections.Clear();
            }

            foreach (var databaseConnections in this._currentlyUsedDatabaseConnectionsMap.Values)
            {
                foreach (DatabaseConnection databaseConnection in databaseConnections)
                    databaseConnection.CloseConnection();

                databaseConnections.Clear();
            }
        }
    }
}
