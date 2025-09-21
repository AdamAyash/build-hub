namespace BuildHubDataEngine.DatabaseConnection
{
    public class DatabaseConnectionPool
    {
        private const short _MAXIMUM_DATABASE_CONNECTIONS_COUNT = 10;
        private const string _CONNECTION_STRING = "Data Source=DESKTOP-E4G86BK\\AAYASH;Persist Security Info=False;User ID=sa;Password=Presiyana890131871;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=30";

        private static DatabaseConnectionPool? _databaseConnectionPoolInstance = null;

        private readonly List<DatabaseConnection> _availableDatabaseConnections;
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

        public static DatabaseConnectionPool GetInstance()
        {
            if (_databaseConnectionPoolInstance == null)
                _databaseConnectionPoolInstance = new DatabaseConnectionPool();

            return _databaseConnectionPoolInstance;
        }

        public DatabaseConnection GetDatabaseConnection()
        {
            DatabaseConnection? databaseConnection = _availableDatabaseConnections.FirstOrDefault();
            if (databaseConnection != null)
            {
                //Validate the connection here.
                this._currentlyUsedDatabaseConnections.Add(databaseConnection);
            }
            else
            {
                databaseConnection = new DatabaseConnection();
            }

            return databaseConnection;
        }

        public void ReleaseDatabaseConnection(DatabaseConnection databaseConnection)
        {
            this._currentlyUsedDatabaseConnections.Remove(databaseConnection);
            this._availableDatabaseConnections.Add(databaseConnection);
        }

        private void Initialize()
        {
            for (int index = 0;  index < _MAXIMUM_DATABASE_CONNECTIONS_COUNT; ++index)
            {
                DatabaseConnection databaseConnection = new DatabaseConnection(_CONNECTION_STRING);
                databaseConnection.InternalConnection.Open();

                //Validate here.
                this._availableDatabaseConnections.Add(databaseConnection);
            };
        }

        private void Cleanup()
        {
            foreach (DatabaseConnection databaseConnection in this._availableDatabaseConnections)
            {
                databaseConnection.InternalConnection.Close();
            }

            foreach (DatabaseConnection databaseConnection in this._currentlyUsedDatabaseConnections)
            {
                databaseConnection.InternalConnection.Close();
            }

            this._availableDatabaseConnections.Clear();
            this._currentlyUsedDatabaseConnections.Clear();
        }
    }
}
