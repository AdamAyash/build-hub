using Microsoft.Data.SqlClient;

namespace BuildHubDataEngine.DatabaseConnection
{
    /// <summary>
    /// A class aiming to test the database connection before use
    /// </summary>
    public sealed class DatabaseConnectionValidator
    {
        /// <summary>
        /// Test query constant
        /// </summary>
        private const string _TEST_SQL_QUERY = "SELECT 1";

        /// <summary>
        /// Database connection member
        /// </summary>
        private readonly DatabaseConnection _databaseConnection;

        public DatabaseConnectionValidator(DatabaseConnection databaseConnection)
        {
            this._databaseConnection = databaseConnection;
        }

        /// <summary>
        /// Tests the database connection by performing a simple select statement
        /// </summary>
        /// <returns>bool</returns>
        public bool TestDatabaseConnection()
        {
            if (!this._databaseConnection.IsConnectionOpen())
                return false;

            var testQuery = new SqlCommand(_TEST_SQL_QUERY, _databaseConnection.InternalConnection);
            bool isSuccessful = Convert.ToInt32(testQuery.ExecuteScalar()) == 1;

            return isSuccessful;
        }
    }
}
