namespace BuildHub.DataEngine.Tables.Base
{
	using BuildHub.Common.Logger;
	using DatabaseConnection;
	using Entities;
	using Microsoft.Data.SqlClient;
	using SQLQueries;
	using System;

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

		private bool _isConnectionLocal;

		public string TableName { get; private set; }

		protected BaseTable(string tableName, DatabaseSource databaseSource)
		{
			this._databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			this._databaseSource = databaseSource;
			this._isConnectionLocal = false;
			this.TableName = tableName;
		}

		/// <summary>
		/// Generates a new globally unique identifier (GUID).
		/// </summary>
		/// <returns>A <see cref="Guid"/> value that is guaranteed to be unique across space and time.</returns>
		private Guid GenerateGUID() => Guid.NewGuid();

		/// <summary>
		/// Resolves whether to use a context connection from the current thread or use a local one.
		/// </summary>
		/// <returns></returns>
		private DatabaseConnection GetConnection()
		{
			DatabaseConnection databaseConnection;

			var databaseConnectionContext = DatabaseContext.GetCurrentContext;
			if (databaseConnectionContext.HasContexDatabaseConnection(this._databaseSource))
			{
				databaseConnection = databaseConnectionContext.GetConnection(this._databaseSource);
				this._isConnectionLocal = false;
			}
			else
			{
				databaseConnection = this._databaseConnectionPoolInstance.GetDatabaseConnection(this._databaseSource);
				this._isConnectionLocal = true;
			}

			return databaseConnection;
		}

		/// <summary>
		/// Retrieves all entities from the underlying data source.
		/// </summary>
		/// <remarks>This method queries the entire table associated with the <see cref="Entity"/> type and returns
		/// all records as entity objects. The returned collection reflects the state of the data source at the time of the
		/// call.</remarks>
		/// <returns>An <see cref="IEnumerable{T}"/> containing all <see cref="Entity"/> instances found in the data source. The
		/// collection will be empty if no records are present.</returns>
		public virtual IEnumerable<Entity> GetAll()
		{
			DatabaseConnection? databaseConnection = null;

			try
			{
				databaseConnection = this.GetConnection();

				var query = new SQLQueryBuilder()
					.From(this.TableName)
					.BuildSelect();

				SqlCommand sqlCommand = new SqlCommand(query.ToString(), databaseConnection.InternalConnection);
				using var sqlReader = sqlCommand.ExecuteReader();

				var entityDataMapper = new EntityDataMapper(sqlReader);

				var entities = new List<Entity>();
				while (sqlReader.Read())
				{
					var entity = entityDataMapper.MaDataToEntity<Entity>();
					entities.Add(entity);
				}

				return entities;
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Retrieving records for table {TableName} failed.");
				throw;
			}
			finally
			{
				if (this._isConnectionLocal && databaseConnection is not null)
					databaseConnection.Dispose();
			}
		}
		private string FormQueryByGuid(Guid guid)
		{
			ColumnMappingData primaryKeyMappingData = EntityDataMapper.GetPrimaryKeyMappingData<Entity>();
			var query = new SQLQueryBuilder()
				.From(this.TableName)
				.Where(primaryKeyMappingData.ColumnDescription.ColumnName, guid)
				.BuildSelect();

			return query.ToString();
		}

		public Entity GetByGuid(Guid guid)
		{
			DatabaseConnection? databaseConnection = null;

			try
			{
				databaseConnection = GetConnection();

				var query = FormQueryByGuid(guid);
				SqlCommand sqlCommand = new SqlCommand(query, databaseConnection.InternalConnection);

				using var sqlReader = sqlCommand.ExecuteReader();
				var entityDataMapper = new EntityDataMapper(sqlReader);

				if (sqlReader.Read())
					return entityDataMapper.MaDataToEntity<Entity>();
				else
					return default;
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Retrieving records for table: '{TableName}' failed with GUID: {guid}.");
				throw;
			}
			finally
			{
				if (this._isConnectionLocal && databaseConnection is not null)
					databaseConnection.Dispose();
			}
		}

		public virtual bool Insert(Entity entity)
		{
			DatabaseConnection? databaseConnection = null;

			try
			{
				databaseConnection = GetConnection();

				if(entity is BaseEntity)
				{
					BaseEntity? baseEntity = entity as BaseEntity;

					if(baseEntity?.Guid.ToString() == string.Empty)
						baseEntity.Guid = this.GenerateGUID();
				}

				var insertQuery = new SQLQueryBuilder()
					.From(this.TableName)
					.BuildInsert<Entity>(entity);

				SqlCommand sqlCommand = new SqlCommand(insertQuery.ToString(), databaseConnection.InternalConnection);

				if(!this._isConnectionLocal)
					sqlCommand.Transaction = DatabaseContext.GetCurrentContext?.TransactionContext?.InternalTransaction;

				return sqlCommand.ExecuteNonQuery() > 0;
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Failed to insert a record for table: '{TableName}'.");
				return false;
			}
			finally
			{
				if (this._isConnectionLocal && databaseConnection is not null)
					databaseConnection.Dispose();
			}
		}
	}
}
