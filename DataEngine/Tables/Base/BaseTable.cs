namespace BuildHub.DataEngine.Tables.Base
{
	#region
	using BuildHub.Common.Logger;
	using BuildHub.Common.Utilities;
	using BuildHub.DataEngine.Exceptions.Entities;
	using DatabaseConnection;
	using Entities;
	using Microsoft.Data.SqlClient;
	using Queries;
	using System;
	using System.Linq.Expressions;

	#endregion

	/// <summary>
	/// Provides a base class for database table access, supporting retrieval of all entities of a specified type.
	/// </summary>
	/// <remarks><para> <see cref="BaseTable{TEntity}"/> is intended to be inherited by concrete table classes that
	/// represent specific database tables. It encapsulates common functionality for interacting with a database table,
	/// such as retrieving all entities. </para> <para> The class manages the table name and database source, and uses a
	/// shared database connection pool for efficient resource management. </para></remarks>
	/// <typeparam name="TEntity">The type of entity represented by the table. Must implement <see cref="IEntity"/>.</typeparam>
	public abstract class BaseTable<TEntity> where TEntity : IEntity
	{
		/// <summary>
		/// Instance to the connection pool.
		/// </summary>
		private readonly DatabaseConnectionPool _databaseConnectionPoolInstance;

		/// <summary>
		/// Database source.
		/// </summary>
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

		protected BaseTable(DatabaseSource databaseSource = DatabaseSource.Core)
		{
			this._databaseConnectionPoolInstance = DatabaseConnectionPool.GetInstance();
			this._databaseSource = databaseSource;
			this._isConnectionLocal = false;
			this._databaseConnection = null;

			try
			{
				this.TableName = EntityDataMapper.GetTableName<TEntity>();
			}
			catch (MissingTableNameException exception)
			{
				Logger.LogError(exception, $"Entity doesn't have a table name defined.");
				throw;
			}
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
		private void AcquireDatabaseConnection()
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
			if (primaryKeyValue is null)
				throw new ArgumentNullException("Null primary key value");

			var internalQueryBuilder = new InternalQueryBuilder()
				.From(this.TableName)
				.Where(primaryKeyMappingData.ColumnInfo.ColumnName, primaryKeyValue)
				.Lock(withLock ? LockTypes.Update : LockTypes.None)
				.BuildSelect();

			return internalQueryBuilder.GetQuery();
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
				this.AcquireDatabaseConnection();

				var internalQueryBuilder = new InternalQueryBuilder()
					.From(this.TableName)
					.BuildSelect();

				using SqlCommand selectCommand = new SqlCommand(internalQueryBuilder.GetQuery(), 
					this._databaseConnection?.InternalConnection);

				if (!this._isConnectionLocal)
					selectCommand.Transaction = DatabaseContext.GetCurrentContext?.TransactionContext?.InternalTransaction;

				using var sqlReader = selectCommand.ExecuteReader();

				var entities = new List<TEntity>();
				while (sqlReader.Read())
				{
					var entity = EntityDataMapper.MapDataToEntity<TEntity>(sqlReader);
					entities.Add(entity);
				}

				return entities;
			}
			catch (MissingColumnDescriptionException missingColumnDescriptionException)
			{
				Logger.LogError(missingColumnDescriptionException, $"Failed to map entity because of missing column description.");
				throw;
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

		/// <summary>
		/// Retrieves an entity of type <typeparamref name="TEntity"/> from the database using its primary key GUID value.
		/// </summary>
		/// <remarks>This method performs a database query to locate an entity by its primary key GUID.  If the entity
		/// does not exist or an error occurs, the method returns <see langword="null"/>.</remarks>
		/// <param name="guid">The GUID value of the entity's primary key to search for.</param>
		/// <returns>The entity of type <typeparamref name="TEntity"/> that matches the specified GUID;  or <see langword="null"/> if
		/// no matching entity is found or if an error occurs during retrieval.</returns>
		public virtual TEntity? GetByGuid(Guid guid)
		{
			try
			{
				this.AcquireDatabaseConnection();

				var primaryKeyColumnInfo = EntityDataMapper.GetPrimaryKeyMappingData<TEntity>().ColumnInfo;
				var queryBuilder = new InternalQueryBuilder()
					.From(this.TableName)
					.Where(primaryKeyColumnInfo.ColumnName, guid)
					.BuildSelect();

				using SqlCommand selectCommand = new SqlCommand(queryBuilder.GetQuery(), 
					this._databaseConnection?.InternalConnection);
				if (!this._isConnectionLocal)
					selectCommand.Transaction = DatabaseContext.GetCurrentContext?.TransactionContext?.InternalTransaction;

				using var sqlReader = selectCommand.ExecuteReader();

				if (!sqlReader.Read())
					return default(TEntity);

				return EntityDataMapper.MapDataToEntity<TEntity>(sqlReader);
			}
			catch (MissingColumnDescriptionException missingColumnDescriptionException)
			{
				Logger.LogError(missingColumnDescriptionException, $"Failed to map entity because of missing column description.");
				throw;
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Retrieving records for table {TableName} failed.");
				return default(TEntity);
			}
			finally
			{
				this.ReleaseDatabaseConnection();
			}
		}

		/// <summary>
		/// Retrieves a collection of entities that match the specified query conditions.
		/// </summary>
		/// <remarks>This method executes a SELECT query against the underlying database table using the criteria
		/// defined in the <paramref name="queryBuilder"/>. The returned entities are mapped from the result set using the
		/// configured entity data mapper.</remarks>
		/// <param name="queryBuilder">The <see cref="QueryBuilder"/> instance that specifies the filtering conditions and query parameters to apply when
		/// selecting entities.</param>
		/// <returns>An <see cref="IEnumerable{TEntity}"/> containing all entities that satisfy the provided query conditions. If no
		/// entities match, the collection will be empty.</returns>
		public virtual IEnumerable<TEntity> GetByCondition(QueryBuilder queryBuilder)
		{
			try
			{
				this.AcquireDatabaseConnection();

				var internalQueryBuilder = new InternalQueryBuilder(queryBuilder)
					.From(this.TableName)
					.BuildSelect();

				using SqlCommand selectCommand = new SqlCommand(internalQueryBuilder.GetQuery(), 
					this._databaseConnection?.InternalConnection);
				if (!this._isConnectionLocal)
					selectCommand.Transaction = DatabaseContext.GetCurrentContext?.TransactionContext?.InternalTransaction;

				using var sqlReader = selectCommand.ExecuteReader();

				var entities = new List<TEntity>();
				while (sqlReader.Read())
				{
					var entity = EntityDataMapper.MapDataToEntity<TEntity>(sqlReader);
					entities.Add(entity);
				}

				return entities;
			}
			catch (MissingColumnDescriptionException missingColumnDescriptionException)
			{
				Logger.LogError(missingColumnDescriptionException, $"Failed to map entity because of missing column description.");
				throw;
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

		/// <summary>
		/// Retrieves a collection of entities that match a specified condition based on the values of a given entity.
		/// </summary>
		/// <remarks>The method uses the value of the property specified by <paramref name="condition"/> from the
		/// provided <paramref name="entity"/> to filter records in the underlying data source. The comparison is performed
		/// against the corresponding column in the database. This method does not track changes to the returned
		/// entities.</remarks>
		/// <param name="entity">The entity whose property value is used to evaluate the condition. The value of the property specified by
		/// <paramref name="condition"/> will be extracted from this entity and used in the query.</param>
		/// <param name="condition">An expression that selects the property of <typeparamref name="TEntity"/> to use as the condition for filtering
		/// results. The property referenced in this expression determines which column is compared in the query.</param>
		/// <returns>An <see cref="IEnumerable{TEntity}"/> containing all entities from the data source that match the specified
		/// condition. Returns an empty collection if no entities satisfy the condition.</returns>
		public virtual IEnumerable<TEntity> GetByCondition(TEntity entity,
			Expression<Func<TEntity, object>> condition, CompareTypes compareType = CompareTypes.Equal)
		{
			try
			{
				this.AcquireDatabaseConnection();

				var columnInfo = EntityDataMapper.GetColumnInfo<TEntity>(condition);
				var value = condition.Compile()(entity);

				var internalQueryBuilder = new InternalQueryBuilder()
					.From(this.TableName)
					.Where(columnInfo.ColumnName, compareType, value)
					.BuildSelect();

				using SqlCommand selectCommand = new SqlCommand(internalQueryBuilder.GetQuery(), this._databaseConnection?.InternalConnection);
				if (!this._isConnectionLocal)
					selectCommand.Transaction = DatabaseContext.GetCurrentContext?.TransactionContext?.InternalTransaction;

				using var sqlReader = selectCommand.ExecuteReader();

				var entities = new List<TEntity>();
				while (sqlReader.Read())
				{
					var currentEntity = EntityDataMapper.MapDataToEntity<TEntity>(sqlReader);
					entities.Add(currentEntity);
				}

				return entities;
			}
			catch (MissingColumnDescriptionException missingColumnDescriptionException)
			{
				Logger.LogError(missingColumnDescriptionException, $"Failed to map entity because of missing column description.");
				throw;
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

		/// <summary>
		/// Inserts the specified entity into the database table associated with this repository.
		/// </summary>
		/// <remarks>This method acquires and releases a database connection as part of the insert operation. If the
		/// entity is a <see cref="BaseEntity"/> or <see cref="VersionedEntity"/>, certain properties may be automatically set
		/// prior to insertion.</remarks>
		/// <param name="entity">The entity to insert. If the entity implements <see cref="BaseEntity"/>, its <c>Guid</c> property will be set if
		/// not already assigned. If the entity implements <see cref="VersionedEntity"/>, its <c>CreatedAt</c> and
		/// <c>UpdatedAt</c> properties will be set to the current date and time.</param>
		/// <returns><see langword="true"/> if the entity was successfully inserted; otherwise, <see langword="false"/>.</returns>
		public virtual bool Insert(TEntity entity)
		{
			try
			{
				this.AcquireDatabaseConnection();

				if (entity is BaseEntity)
				{
					BaseEntity? baseEntity = entity as BaseEntity;

					if (baseEntity?.Guid == Guid.Empty)
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

				var internalQueryBuilder = new InternalQueryBuilder()
					.From(this.TableName)
					.BuildInsert(entity);

				using SqlCommand insertCommand = new SqlCommand(internalQueryBuilder.GetQuery(), 
					this._databaseConnection?.InternalConnection);

				if (!this._isConnectionLocal)
					insertCommand.Transaction = DatabaseContext.GetCurrentContext?.TransactionContext?.InternalTransaction;

				insertCommand.ExecuteNonQuery();
				return true;
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Failed to insert a record for table: '{TableName}'.");
				return false;
			}
			finally
			{
				this.ReleaseDatabaseConnection();
			}
		}

		/// <summary>
		/// Updates the specified entity in the database using its primary key.
		/// </summary>
		/// <remarks>If the entity implements versioning (i.e., is a <see cref="VersionedEntity"/>), the method checks
		/// for version consistency before updating and increments the version upon a successful update. The method returns
		/// <see langword="false"/> if the update fails due to an error or exception.</remarks>
		/// <param name="entity">The entity to update. Must contain a valid primary key corresponding to an existing record in the database.</param>
		/// <returns><see langword="true"/> if the entity was successfully updated; otherwise, <see langword="false"/>.</returns>
		public virtual bool Update(TEntity entity)
		{
			try
			{
				this.AcquireDatabaseConnection();

				var selectQuery = GenerateSelectQueryByPrimaryKey(entity, true);
				using SqlCommand updateCommand = new SqlCommand(selectQuery, 
					this._databaseConnection?.InternalConnection);

				if (!this._isConnectionLocal)
					updateCommand.Transaction = DatabaseContext.GetCurrentContext?.TransactionContext?.InternalTransaction;

				var sqlReader = updateCommand.ExecuteReader();

				TEntity existingEntity;

				if (sqlReader.Read())
					existingEntity = EntityDataMapper.MapDataToEntity<TEntity>(sqlReader);
				else
					throw new EntityDoesNotExistException();

				sqlReader.Close();

				if (entity is VersionedEntity)
				{
					VersionedEntity? existingVersionedEntity = existingEntity as VersionedEntity;
					VersionedEntity? currentVersionedEntity = entity as VersionedEntity;

					if (existingVersionedEntity is not null && currentVersionedEntity is not null)
					{
						if (existingVersionedEntity.Version != currentVersionedEntity.Version)
							throw new InconsistentEntityVersionException();

						currentVersionedEntity.IncrementVersion();
						currentVersionedEntity.UpdatedAt = Utilities.GetCurrentDateTime;
					}
				}

				var updateQueryBuilder = new InternalQueryBuilder()
					.From(this.TableName)
					.BuildUpdate<TEntity>(entity);

				updateCommand.CommandText = updateQueryBuilder.GetQuery();
				updateCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Failed to update a record for table: '{TableName}'.");
				return false;
			}
			finally
			{
				this.ReleaseDatabaseConnection();
			}

			return true;
		}

		/// <summary>
		/// Deletes the specified entity from the database.
		/// </summary>
		/// <remarks>This method attempts to remove the provided entity from the underlying table. If the entity does
		/// not exist or an error occurs during deletion, the method returns <see langword="false"/>.</remarks>
		/// <param name="entity">The entity to delete. Must not be <c>null</c>; the entity should contain a valid primary key value.</param>
		/// <returns><see langword="true"/> if the entity was successfully deleted; otherwise, <see langword="false"/>.</returns>
		public bool Delete(TEntity entity)
		{
			try
			{
				this.AcquireDatabaseConnection();

				var selectQuery = GenerateSelectQueryByPrimaryKey(entity);

				using SqlCommand deleteCommand = new SqlCommand(selectQuery,
					this._databaseConnection?.InternalConnection);

				if (!this._isConnectionLocal)
					deleteCommand.Transaction = DatabaseContext.GetCurrentContext?.TransactionContext?.InternalTransaction;

				TEntity existingEntity;
				var sqlReader = deleteCommand.ExecuteReader();

				if (sqlReader.Read())
					existingEntity = EntityDataMapper.MapDataToEntity<TEntity>(sqlReader);
				else
					throw new EntityDoesNotExistException();

				sqlReader.Close();

				var internalQueryBuilder = new InternalQueryBuilder()
					.From(this.TableName)
					.BuildDelete<TEntity>(entity);

				deleteCommand.CommandText = internalQueryBuilder.GetQuery();
				deleteCommand.ExecuteNonQuery();
			}
			catch (Exception exception)
			{
				Logger.LogError(exception, $"Failed to delete a record for table: '{TableName}'.");
				return false;
			}
			finally
			{
				this.ReleaseDatabaseConnection();
			}

			return true;
		}
	}
}
