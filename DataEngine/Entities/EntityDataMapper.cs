#region
using BuildHub.Common.Utilities;
using BuildHub.DataEngine.Exceptions;
using BuildHub.DataEngine.Exceptions.Entities;
using Microsoft.Data.SqlClient;
using System.Collections.Concurrent;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
#endregion

namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Provides static methods for mapping data between database columns and entity properties, supporting attribute-based
	/// configuration and caching for efficient data access.
	/// </summary>
	/// <remarks>The EntityDataMapper class is designed to facilitate the conversion of data from a SqlDataReader to
	/// strongly-typed entity objects that implement the IEntity interface. It uses reflection and custom attributes to
	/// determine column mappings and supports caching to improve performance when mapping multiple entities of the same
	/// type. This class is thread-safe and intended for use in data access layers where attribute-driven mapping is
	/// required.</remarks>
	public sealed class EntityDataMapper
	{
		/// <summary>
		/// Caches the mapping information between entity types
		/// and their corresponding column metadata to improve lookup
		/// performance.
		/// </summary>
		/// <remarks>This cache enables efficient retrieval of column mapping data for entity types, reducing repeated
		/// computation or database schema inspection. The cache is thread-safe and can be accessed concurrently.</remarks>
		private static ConcurrentDictionary<Type, List<ColumnMappingData>> _entityColumnMappingCache = new();

		/// <summary>
		/// Creates a new entity instance and populates its properties with values from the current row of the specified
		/// SqlDataReader.
		/// </summary>
		/// <remarks>The mapping is based on cached column-property associations for performance. The SqlDataReader
		/// must be positioned on a valid row before calling this method. Only properties with corresponding columns in the
		/// data reader will be set.</remarks>
		/// <typeparam name="TEntity">The type of entity to create and populate. Must implement the IEntity interface.</typeparam>
		/// <param name="sqlDataReader">The SqlDataReader positioned at the row to map to the entity. Must not be null.</param>
		/// <returns>An instance of the specified entity type with properties set to the corresponding column values from the
		/// SqlDataReader.</returns>
		public static TEntity MapDataToEntity<TEntity>(SqlDataReader sqlDataReader)
			where TEntity : IEntity
		{
			Type entityType = typeof(TEntity);
			TEntity entity = Activator.CreateInstance<TEntity>();

			if (_entityColumnMappingCache.ContainsKey(entityType))
			{
				var cachedColumnMappingData = _entityColumnMappingCache[entityType];
				foreach (ColumnMappingData columnMappingData in cachedColumnMappingData)
				{
					object columnValue = sqlDataReader[columnMappingData.ColumnInfo.ColumnName];
					columnMappingData.PropertyInfo.SetValue(entity, columnValue, null);
				}
			}
			else
			{
				var entityColumnMappingList = new List<ColumnMappingData>();

				var properties = Utilities.GetObjectProperties<TEntity>();
				foreach (PropertyInfo property in properties)
				{
					ColumnInfo? columnInfo = GetColumnInfo(property);

					object columnValue = sqlDataReader[columnInfo.ColumnName];
					property.SetValue(entity, columnValue, null);

					entityColumnMappingList.Add(new ColumnMappingData(columnInfo, property));
				}

				_entityColumnMappingCache.TryAdd(entityType, entityColumnMappingList);
			}

			return entity;
		}

		/// <summary>
		/// Retrieves the <see cref="ColumnInfo"/> attribute applied to the specified property.
		/// </summary>
		/// <param name="property">The property for which to retrieve the associated <see cref="ColumnInfo"/> attribute. Cannot be null.</param>
		/// <returns>The <see cref="ColumnInfo"/> attribute instance associated with the specified property.</returns>
		/// <exception cref="MissingColumnDescriptionException">Thrown if the specified property does not have a <see cref="ColumnInfo"/> attribute applied.</exception>
		public static ColumnInfo GetColumnInfo(PropertyInfo property)
		{
			ColumnInfo? columnDescription = property.GetCustomAttribute<ColumnInfo>();
			if (columnDescription is null)
				throw new MissingColumnDescriptionException(property);

			return columnDescription;
		}

		public static string GetTableName<TEntity>() where TEntity : IEntity
		{
			Type entityType = typeof(TEntity);

			TableName? tableName = entityType.GetCustomAttribute<TableName>();
			if (tableName is null)
				throw new MissingTableNameException(entityType);

			return tableName.Name;
		}

		/// <summary>
		/// Retrieves column metadata for the specified property of an entity type.
		/// </summary>
		/// <remarks>Use this method to obtain database column information for a strongly-typed property of an entity.
		/// This approach provides compile-time safety and avoids errors from using string property names.</remarks>
		/// <typeparam name="TEntity">The entity type that contains the property for which to retrieve column information. Must implement <see
		/// cref="IEntity"/>.</typeparam>
		/// <param name="propertyExpressions">An expression that identifies the property of the entity type. Typically provided as a lambda expression, such as
		/// <c>x => x.PropertyName</c>.</param>
		/// <returns>A <see cref="ColumnInfo"/> object containing metadata about the specified property.</returns>
		public static ColumnInfo GetColumnInfo<TEntity>(Expression<Func<TEntity, object>> propertyExpressions)
			 where TEntity : IEntity
		{
			PropertyInfo propertyInfo = Utilities.GetPropertyInfo<TEntity>(propertyExpressions);

			return GetColumnInfo(propertyInfo);
		}

		/// <summary>
		/// Retrieves the column mapping data for the primary key property of the specified entity type.
		/// </summary>
		/// <remarks>Use this method to obtain metadata about the primary key column for an entity type, which is
		/// typically required for database operations such as updates or deletes. The entity type must have exactly one
		/// property marked with the <see cref="PrimaryKey"/> attribute.</remarks>
		/// <typeparam name="TEntity">The type of the entity for which to retrieve primary key mapping data. Must implement <see cref="IEntity"/>.</typeparam>
		/// <returns>A <see cref="ColumnMappingData"/> instance representing the mapping information for the primary key property of
		/// the specified entity type.</returns>
		/// <exception cref="MissingPrimaryKeyException">Thrown if the specified entity type does not define a property marked with the <see cref="PrimaryKey"/> attribute.</exception>
		public static ColumnMappingData GetPrimaryKeyMappingData<TEntity>()
			where TEntity : IEntity
		{
			List<PropertyInfo> properties = Utilities.GetObjectProperties<TEntity>().ToList();
			PropertyInfo? primaryKeyProperty = properties.Find(property => property.GetCustomAttributes<PrimaryKey>().Count() > 0);
			if (primaryKeyProperty is null)
				throw new MissingPrimaryKeyException();

			return new ColumnMappingData(GetColumnInfo(primaryKeyProperty), primaryKeyProperty);
		}

		/// <summary>
		/// Determines whether the specified property is marked with the Identity attribute, indicating that it represents an
		/// identity column.
		/// </summary>
		/// <param name="property">The property to inspect for the Identity attribute. Cannot be null.</param>
		/// <returns>true if the property is decorated with the Identity attribute; otherwise, false.</returns>
		public static bool HasIdentityColumn(PropertyInfo property) => property.GetCustomAttribute<Identity>() is not null;

		/// <summary>
		/// Determines whether the specified property is marked with the primary key attribute, indicating that it represents a
		/// primary column.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public static bool HasPrimaryKeyColumn(PropertyInfo property) => property.GetCustomAttribute<PrimaryKey>() is not null;

		/// <summary>
		/// Retrieves the value of the specified property from the given entity instance.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity from which to retrieve the property value.</typeparam>
		/// <param name="entity">The instance of the entity containing the property. Cannot be null.</param>
		/// <param name="propertyInfo">The property metadata that identifies which property value to retrieve. Must refer to a property of the entity
		/// type. Cannot be null.</param>
		/// <returns>The value of the specified property for the given entity, or null if the property value is null.</returns>
		public static object? GetColumnValue<TEntity>(TEntity entity, PropertyInfo propertyInfo) => propertyInfo.GetValue(entity);
	}
}
