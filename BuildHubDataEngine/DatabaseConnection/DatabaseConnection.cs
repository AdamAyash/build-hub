using Microsoft.Data.SqlClient;

namespace BuildHubDataEngine.DatabaseConnection
{
    public class DatabaseConnection
    {
        private SqlConnection _internalDatabaseConnection;

        public DatabaseConnection()
        {
            this._internalDatabaseConnection = new SqlConnection();
        }

        public DatabaseConnection(string connectionString)
        {
            this._internalDatabaseConnection = new SqlConnection(connectionString);
        }

        public SqlConnection InternalConnection
        {
            get
            {
                return this._internalDatabaseConnection;
            }
        }
    }
}
