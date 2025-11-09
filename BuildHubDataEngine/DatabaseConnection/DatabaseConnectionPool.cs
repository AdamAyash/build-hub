using BuildHubCommon.ConfigurationManager;
using Microsoft.Data.SqlClient;

namespace BuildHubDataEngine.DatabaseConnection
{
    /// <summary>
    /// Database connection pool singleton, initializing and managing a number of database connections.
    /// </summary>
    public sealed class DatabaseConnectionPool
    {
        private const short _MAXIMUM_DATABASE_CONNECTIONS_COUNT = 10;
        private const string _CONNECTION_STRING = "Data Source=AAyash\\SQL2022; Initial Catalog=BuildHub; Persist Security Info=False;User ID=sa;Password=massive;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=True;";

        /// <summary>Singleton instance</summary>
        private static DatabaseConnectionPool? _databaseConnectionPoolInstance = null;
        /// <summary>Available connection ready for use</summary>
        private readonly List<DatabaseConnection> _availableDatabaseConnections;
        /// <summary>Currently used connections</summary>
        private readonly List<DatabaseConnection> _currentlyUsedDatabaseConnections;

        private DatabaseConnectionPool()
        {
            this._availableDatabaseConnections = new List<DatabaseConnection>();
            this._currentlyUsedDatabaseConnections = new List<DatabaseConnection>();

            Initialize();
        }

        ~DatabaseConnectionPool()
        {
            Cleanup();
        }

        /// <summary>
        /// Returns the numbers of available connections
        /// </summary>
        public int AvailableConnections => this._availableDatabaseConnections.Count();

        /// <summary>
        /// Returns the number of connections currently in use
        /// </summary>
        public int ConnectionsCurrentlyInUse => this._availableDatabaseConnections.Count();

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
        public DatabaseConnection GetDatabaseConnection()
        {
            DatabaseConnection? databaseConnection = _availableDatabaseConnections.FirstOrDefault();

            if (databaseConnection == null)
                databaseConnection = TryToInitializeConnection();

            var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);
            if (!databaseConnectionValidator.TestDatabaseConnection())
                throw new InvalidOperationException();

            this._currentlyUsedDatabaseConnections.Add(databaseConnection);
            this._availableDatabaseConnections.Remove(databaseConnection);

            return databaseConnection;
        }

        /// <summary>
        /// Returns a database connection to the pool
        /// </summary>
        /// <param name="databaseConnection"></param>
        public void ReleaseDatabaseConnection(DatabaseConnection databaseConnection)
        {
            this._currentlyUsedDatabaseConnections.Remove(databaseConnection);
            this._availableDatabaseConnections.Add(databaseConnection);
        }

        /// <summary>
        /// Tries to initialize a database connection
        /// </summary>
        /// <returns>DatabaseConnection</returns>
        /// <exception cref="InvalidOperationException"></exception>
        private DatabaseConnection TryToInitializeConnection()
        {
            var databaseConnection = new DatabaseConnection(_CONNECTION_STRING);

            try
            {
                databaseConnection.OpenConnection();
            }
            catch (SqlException sqlException)
            {
                //LOG ERROR
            }

            var databaseConnectionValidator = new DatabaseConnectionValidator(databaseConnection);
            if (!databaseConnectionValidator.TestDatabaseConnection())
                throw new InvalidOperationException();

            return databaseConnection;
        }

        /// <summary>
        /// Initializes a number of database connections
        /// </summary>
        private void Initialize()
        {
            ConfigurationManager configurationManager = ConfigurationManager.GetConfigurationManager();

            IEnumerable<DatabaseSettingsModel>? databaseSettings = configurationManager.GetConfigurationModels<DatabaseSettingsModel>("DatabaseSettings");
            if(databaseSettings is null)
            {
                //LOG error 
                //trow exeception
                return;
            }

            foreach (var settingsModel in databaseSettings)
            {

            }

            //for (vae index = 0; index < _MAXIMUM_DATABASE_CONNECTIONS_COUNT; ++index)
            //    this._availableDatabaseConnections.Add(TryToInitializeConnection());
        }

        /// <summary>
        /// Closes all connections
        /// </summary>
        private void Cleanup()
        {
            foreach (DatabaseConnection databaseConnection in this._availableDatabaseConnections)
                databaseConnection.CloseConnection();

            foreach (DatabaseConnection databaseConnection in this._currentlyUsedDatabaseConnections)
                databaseConnection.CloseConnection();

            this._availableDatabaseConnections.Clear();
            this._currentlyUsedDatabaseConnections.Clear();
        }
    }
}
