using BuildHub.DataEngine.Exceptions;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Provides functionality to map data from a <see cref="SqlDataReader"/> to an instance of the specified entity type.
	/// </summary>
	/// <remarks><para> The <see cref="EntityDataMapper{Entity}"/> class is intended for internal use to facilitate
	/// the conversion of database records into strongly typed entity objects. The entity type must implement <see
	/// cref="IEntity"/> and have properties decorated with <c>ColumnDescription</c> attributes to enable mapping. </para>
	/// <para> This class is not thread-safe. Each instance should be used only within the context of a single data reader
	/// and mapping operation. </para></remarks>
	/// <typeparam name="Entity"></typeparam>
	internal sealed class  EntityDataMapper<Entity> where Entity : IEntity
	{
		private readonly SqlDataReader sqlDataReader;

		public EntityDataMapper(SqlDataReader sqlDataReader) => this.sqlDataReader = sqlDataReader;

		public Entity MaDataToEntity()
		{
			Entity entity = Activator.CreateInstance<Entity>();

			List<PropertyInfo> properties = typeof(Entity).GetProperties().ToList();
			foreach (PropertyInfo property in properties) 
			{
				ColumnDescription? columnDescription =  property.GetCustomAttribute<ColumnDescription>();
				if (columnDescription is null)
					throw new MissingColumnDescriptionException(property);

				object columnValue = sqlDataReader[columnDescription.ColumnName];
				property.SetValue(entity, columnValue, null);
			}

			return entity;
		}
	}
}
