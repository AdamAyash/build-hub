namespace BuildHub.DataEngine.Tables.Base
{
	using Entities;
	using SQLQueries;
	using DatabaseConnection;
	using Microsoft.Data.SqlClient;
	using BuildHub.Common.Logger;

	/// <summary>
	/// Provides a base class for database table access, supporting retrieval of all entities of a specified type.
	/// </summary>
	/// <remarks><para> <see cref="BaseTable{Entity}"/> is intended to be inherited by concrete table classes that
	/// represent specific database tables. It encapsulates common functionality for interacting with a database table,
	/// such as retrieving all entities. </para> <para> The class manages the table name and database source, and uses a
	/// shared database connection pool for efficient resource management. </para></remarks>
	/// <typeparam name="Entity">The type of entity represented by the table. Must implement <see cref="IEntity"/>.</typeparam>
	public abstract class BaseTable<Entity> where Entity : IEntity
	{
		private readonly DatabaseConnectionPool _databaseConnectionPoolInstance;
		private readonly DatabaseSource _databaseSource;

		public string TableName { get; private set; }

		protected BaseTable(string tableName, DatabaseSource databaseSource)
		{
			this._databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			this._databaseSource = databaseSource;
			this.TableName = tableName;
		}

		private bool Initialize()
		{

			return true;
		}

		/// <summary>
		/// Generates a new globally unique identifier (GUID).
		/// </summary>
		/// <returns>A <see cref="Guid"/> value that is guaranteed to be unique across space and time.</returns>
		private Guid GenerateGUID() => Guid.NewGuid();

		/// <summary>
		/// Retrieves all entities from the underlying data source.
		/// </summary>
		/// <remarks>This method queries the entire table associated with the <see cref="Entity"/> type and returns
		/// all records as entity objects. The returned collection reflects the state of the data source at the time of the
		/// call.</remarks>
		/// <returns>An <see cref="IEnumerable{T}"/> containing all <see cref="Entity"/> instances found in the data source. The
		/// collection will be empty if no records are present.</returns>
		public IEnumerable<Entity> GetAll()
		{
			var entities = new List<Entity>();

			try
			{
				using var databaseConnection = this._databaseConnectionPoolInstance.GetDatabaseConnection(this._databaseSource);

				var query = new SQLQueryBuilder()
					.From(this.TableName)
					.BuildSelect();

				Logger.LogDebug($"Table '{TableName}' generated a query '{query.Query}'");

				SqlCommand sqlCommand = new SqlCommand(query.Query, databaseConnection.InternalConnection);
				using var sqlReader = sqlCommand.ExecuteReader();

				EntityDataMapper<Entity> entityDataMapper = new EntityDataMapper<Entity>(sqlReader);

				while (sqlReader.Read())
				{
					var entity = entityDataMapper.MaDataToEntity();
					entities.Add(entity);
				}
			}
			catch(Exception exception)
			{
				Logger.LogError(exception, $"Retrieving records for table {TableName} failed.");
				throw;
			}

			return entities;
		}

		public Entity GetByGuid(Guid guid)
		{
			try
			{
				using var databaseConnection = this._databaseConnectionPoolInstance.GetDatabaseConnection(this._databaseSource);

				var query = new SQLQueryBuilder()
					.From(this.TableName)
					.BuildSelect();

				Logger.LogDebug($"Table '{TableName}' generated a query '{query.Query}'");

				SqlCommand sqlCommand = new SqlCommand(query.Query, databaseConnection.InternalConnection);
				using var sqlReader = sqlCommand.ExecuteReader();

				EntityDataMapper<Entity> entityDataMapper = new EntityDataMapper<Entity>(sqlReader);

				while (sqlReader.Read())
				{
					var entity = entityDataMapper.MaDataToEntity();
					entities.Add(entity);
				}
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Retrieving records for table {TableName} failed.");
				throw;
			}
		}
	}
}
