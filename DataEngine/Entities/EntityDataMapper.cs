using BuildHub.Common.Utilities;
using BuildHub.DataEngine.Exceptions;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;

namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Provides functionality to map data from a <see cref="SqlDataReader"/> to an instance of the specified entity type.
	/// </summary>
	/// <remarks><para> The <see cref="EntityDataMapper{Entity}"/> class is intended for internal use to facilitate
	/// the conversion of database records into strongly typed entity objects. The entity type must implement <see
	/// <see cref="IEntity"/> and have properties decorated with <c>ColumnDescription</c> attributes to enable mapping. </para>
	/// <para> This class is not thread-safe. Each instance should be used only within the context of a single data reader
	/// and mapping operation. </para></remarks>
	/// <typeparam name="Entity"></typeparam>
	internal sealed class EntityDataMapper<Entity> where Entity : IEntity
	{
		private readonly SqlDataReader sqlDataReader;
		public EntityDataMapper(SqlDataReader sqlDataReader) => this.sqlDataReader = sqlDataReader;

		public Entity MaDataToEntity()
		{
			Entity entity = Activator.CreateInstance<Entity>();

			var properties = Utilities.GetObjectProiperties<Entity>();
			foreach (PropertyInfo property in properties)
			{
				ColumnDescription? columnDescription = property.GetCustomAttribute<ColumnDescription>();
				if (columnDescription is null)
					throw new MissingColumnDescriptionException(property);

				object columnValue = sqlDataReader[columnDescription.ColumnName];
				property.SetValue(entity, columnValue, null);
			}

			return entity;
		}

		public static string GetColumnName(Expression<Func<Entity, object>> propertyExpressions)
		{
			PropertyInfo propertyInfo = Utilities.GetPropertyInfo<Entity>(propertyExpressions);
			ColumnDescription? columnDescription = propertyInfo.GetCustomAttribute<ColumnDescription>();
			if (columnDescription is null)
				throw new MissingColumnDescriptionException(propertyInfo);

			return columnDescription.ColumnName;
		}

		public static ColumnMappingData GetPrimaryKeyInfo()
		{
			List<PropertyInfo> properties = Utilities.GetObjectProiperties<Entity>().ToList();
			PropertyInfo? primaryKeyProperty = properties.Find(property => property.GetCustomAttributes<PrimaryKey>().Count() > 0);
			if (primaryKeyProperty is null)
				throw new Exception();

			ColumnDescription? columnDescription = primaryKeyProperty.GetCustomAttribute<ColumnDescription>();
			if (columnDescription is null)
				throw new MissingColumnDescriptionException(primaryKeyProperty);

			return new ColumnMappingData(columnDescription, primaryKeyProperty);
		}
	}
}
