using Microsoft.Data.SqlClient;
using System.Data;

namespace BuildHubDataEngine.DatabaseConnection
{
    /// <summary>
    /// A virtual proxy to the SqlConnection class
    /// </summary>
    public class DatabaseConnection
    {
        private readonly SqlConnection _internalDatabaseConnection;

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

        public bool IsConnectionOpen() => this._internalDatabaseConnection.State == ConnectionState.Open;

        public void OpenConnection() => this._internalDatabaseConnection.Open();

        public void CloseConnection() => this._internalDatabaseConnection.Close();
    }
}
