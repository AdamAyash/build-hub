namespace BuildHub.DataEngine.Tables.Base
{
	using DatabaseConnection;
	using Microsoft.Data.SqlClient;
	using Tables.Entities;

	internal abstract class BaseTable<Entity>
		where Entity : IEntity
	{
		private readonly DatabaseConnectionPool _databaseConnectionPoolInstance;
		private readonly DatabaseSource _databaseSource;

		public string TableName { get; private set; }

		protected BaseTable(string tableName, DatabaseSource databaseSource)
		{
			this._databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			this.TableName = tableName;
			this._databaseSource = databaseSource;
		}

		public abstract bool InitializeTableBindings();

		public bool GetAll(IEnumerable<Entity> entities)
		{
			using var databaseConnection = this._databaseConnectionPoolInstance.GetDatabaseConnection(this._databaseSource);

			SqlCommand sqlCommand = new SqlCommand($"SELECT * FROM {this.TableName}", databaseConnection.InternalConnection);
			sqlCommand.ExecuteNonQuery();

			return true;
		}
	}
}
