namespace BuildHub.DataEngine.Tables.Base
{
	#region
	using Entities;
	using SQLQueries;
	using System;
	using DatabaseConnection;
	using BuildHub.Common.Logger;
	using System.Linq.Expressions;
	using Microsoft.Data.SqlClient;
	using BuildHub.DataEngine.Exceptions;
	using BuildHub.Common.Utilities;
	#endregion

	/// <summary>
	/// Provides a base class for database table access, supporting retrieval of all entities of a specified type.
	/// </summary>
	/// <remarks><para> <see cref="BaseTable{Entity}"/> is intended to be inherited by concrete table classes that
	/// represent specific database tables. It encapsulates common functionality for interacting with a database table,
	/// such as retrieving all entities. </para> <para> The class manages the table name and database source, and uses a
	/// shared database connection pool for efficient resource management. </para></remarks>
	/// <typeparam name="TEntity">The type of entity represented by the table. Must implement <see cref="IEntity"/>.</typeparam>
	public abstract class BaseTable<TEntity> where TEntity : IEntity
	{
		private readonly DatabaseConnectionPool _databaseConnectionPoolInstance;
		private readonly DatabaseSource _databaseSource;

		/// <summary>
		/// Represents whether the connection is retrieved from context of not.
		/// </summary>
		private bool _isConnectionLocal;
		private DatabaseConnection? _databaseConnection;

		/// <summary>
		/// Represents the table name
		/// </summary>
		public string TableName { get; private set; }

		protected BaseTable(string tableName, DatabaseSource databaseSource)
		{
			this._databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			this._databaseSource = databaseSource;
			this._isConnectionLocal = false;
			this._databaseConnection = null;
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
		private DatabaseConnection GetDatabaseConnection()
		{
			var databaseConnectionContext = DatabaseContext.GetCurrentContext;
			if (databaseConnectionContext.HasContextDatabaseConnection(this._databaseSource))
			{
				this._databaseConnection = databaseConnectionContext.GetConnection(this._databaseSource);
				this._isConnectionLocal = false;
			}
			else
			{
				this._databaseConnection = this._databaseConnectionPoolInstance.GetDatabaseConnection(this._databaseSource);
				this._isConnectionLocal = true;
			}

			return this._databaseConnection;
		}

		/// <summary>
		/// Releases the database connection if it is locally owned by the current instance.
		/// </summary>
		/// <remarks>This method disposes of the database connection only if the connection was created and is managed
		/// by this instance. It should be called to ensure that local resources are properly released when they are no longer
		/// needed.</remarks>
		private void ReleaseDatabaseConnection()
		{
			if (this._isConnectionLocal && this._databaseConnection is not null)
				this._databaseConnection.Dispose();
		}

		/// <summary>
		/// Forms a SQL SELECT query that retrieves an entity by its primary key using the specified GUID value.
		/// </summary>
		/// <param name="guid">The GUID value to match against the primary key column in the query.</param>
		/// <returns>A string containing the SQL SELECT statement that filters by the specified GUID primary key.</returns>
		private string GenerateSelectQueryByPrimaryKey(TEntity entity, bool withLock = false)
		{
			ColumnMappingData primaryKeyMappingData = EntityDataMapper.GetPrimaryKeyMappingData<TEntity>();

			object? primaryKeyValue = EntityDataMapper.GetColumnValue<TEntity>(entity, primaryKeyMappingData.PropertyInfo);
			if(primaryKeyValue is null)
				throw new ArgumentNullException("Null primary key value");

			var query = new SQLQueryBuilder()
				.From(this.TableName)
				.Where(primaryKeyMappingData.ColumnInfo.ColumnName, primaryKeyValue)
				.Lock(withLock ? LockTypes.Update : LockTypes.None)
				.BuildSelect();

			return query.GetQuery();
		}

		/// <summary>
		/// Retrieves all entities from the underlying data source.
		/// </summary>
		/// <remarks>This method queries the entire table associated with the <see cref="TEntity"/> type and returns
		/// all records as entity objects. The returned collection reflects the state of the data source at the time of the
		/// call.</remarks>
		/// <returns>An <see cref="IEnumerable{T}"/> containing all <see cref="TEntity"/> instances found in the data source. The
		/// collection will be empty if no records are present.</returns>
		public virtual IEnumerable<TEntity> GetAll()
		{
			try
			{
				this._databaseConnection = this.GetDatabaseConnection();

				var queryBuilder = new SQLQueryBuilder()
					.From(this.TableName)
					.BuildSelect();

				using SqlCommand sqlCommand = new SqlCommand(queryBuilder.GetQuery(), this._databaseConnection.InternalConnection);
				using var sqlReader = sqlCommand.ExecuteReader();

				var entities = new List<TEntity>();
				while (sqlReader.Read())
				{
					var entity = EntityDataMapper.MapDataToEntity<TEntity>(sqlReader);
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
				this.ReleaseDatabaseConnection();
			}
		}

		public virtual TEntity GetByGuid(Guid guid)
		{
			try
			{
				this._databaseConnection = this.GetDatabaseConnection();

				var primaryKeyColumnInfo = EntityDataMapper.GetPrimaryKeyMappingData<TEntity>().ColumnInfo;
				var queryBuilder = new SQLQueryBuilder()
					.From(this.TableName)
					.Where(primaryKeyColumnInfo.ColumnName, guid)
					.BuildSelect();

				using SqlCommand sqlCommand = new SqlCommand(queryBuilder.GetQuery(), this._databaseConnection.InternalConnection);
				using var sqlReader = sqlCommand.ExecuteReader();

				if (!sqlReader.Read())
					throw new EntityDoesNotExistException();

				return EntityDataMapper.MapDataToEntity<TEntity>(sqlReader);
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Retrieving records for table {TableName} failed.");
				throw;
			}
			finally
			{
				this.ReleaseDatabaseConnection();
			}
		}
		public virtual IEnumerable<TEntity> GetByCondition(IQueryBuilder queryBuilder)
		{
			try
			{
				this._databaseConnection = this.GetDatabaseConnection();

				using SqlCommand sqlCommand = new SqlCommand(queryBuilder.GetQuery(), this._databaseConnection.InternalConnection);
				using var sqlReader = sqlCommand.ExecuteReader();

				var entities = new List<TEntity>();
				while (sqlReader.Read())
				{
					var entity = EntityDataMapper.MapDataToEntity<TEntity>(sqlReader);
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
				this.ReleaseDatabaseConnection();
			}
		}

		public virtual void Insert(TEntity entity)
		{
			DatabaseConnection? databaseConnection = null;

			try
			{
				databaseConnection = GetDatabaseConnection(); 

				if(entity is BaseEntity)
				{
					BaseEntity? baseEntity = entity as BaseEntity;

					if(baseEntity?.Guid == Guid.Empty)
						baseEntity.Guid = this.GenerateGUID();
				}

				if (entity is VersionedEntity)
				{
					VersionedEntity? veriosnedEntity = entity as VersionedEntity;

					if (veriosnedEntity is not null)
					{
						veriosnedEntity.UpdatedAt = Utilities.GetCurrentDateTime;
						veriosnedEntity.CreatedAt = Utilities.GetCurrentDateTime;
					}
				}

				var queryBuilder = new SQLQueryBuilder()
					.From(this.TableName)
					.BuildInsert(entity);

				using SqlCommand sqlCommand = new SqlCommand(queryBuilder.GetQuery(), databaseConnection.InternalConnection);

				if(!this._isConnectionLocal)
					sqlCommand.Transaction = DatabaseContext.GetCurrentContext?.TransactionContext?.InternalTransaction;

				 sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Failed to insert a record for table: '{TableName}'.");
				throw;
			}
			finally
			{
				this.ReleaseDatabaseConnection();
			}
		}

		public virtual void Update(TEntity entity)
		{
			try
			{
				this._databaseConnection = GetDatabaseConnection();

				var selectQuery = GenerateSelectQueryByPrimaryKey(entity, true);
				using SqlCommand sqlCommand = new SqlCommand(selectQuery, this._databaseConnection.InternalConnection);

				if (!this._isConnectionLocal)
					sqlCommand.Transaction = DatabaseContext.GetCurrentContext?.TransactionContext?.InternalTransaction;

				var sqlReader = sqlCommand.ExecuteReader();

				TEntity existingEntity;

				if (sqlReader.Read())
					existingEntity = EntityDataMapper.MapDataToEntity<TEntity>(sqlReader);
				else
					throw new EntityDoesNotExistException();

				if (entity is VersionedEntity)
				{
					VersionedEntity? existingVersionedEntity = existingEntity as VersionedEntity;
					VersionedEntity? currentVersionedEntity = entity as VersionedEntity;

					if (existingVersionedEntity is not null && currentVersionedEntity is not null)
					{
						if (existingVersionedEntity.Version != currentVersionedEntity.Version)
							throw new InconsistentEntityVersionException();

						currentVersionedEntity.Version++;
						currentVersionedEntity.UpdatedAt = Utilities.GetCurrentDateTime;
					}
				}

				sqlReader.Close();

				var updateQueryBuilder = new SQLQueryBuilder()
					.From(this.TableName)
					.BuildUpdate<TEntity>(entity);

				sqlCommand.CommandText = updateQueryBuilder.GetQuery();
				sqlCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Failed to update a record for table: '{TableName}'.");
				throw;
			}
			finally
			{
				this.ReleaseDatabaseConnection();
			}
		}
	}
}
