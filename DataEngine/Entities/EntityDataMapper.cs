using Microsoft.Data.SqlClient;
using System.Reflection;

namespace BuildHub.DataEngine.Entities
{
	internal class EntityDataMapper<Entity> where Entity : IEntity
	{
		private readonly SqlDataReader sqlDataReader;

		public EntityDataMapper(SqlDataReader sqlDataReader)
		{
			this.sqlDataReader = sqlDataReader;
		}

		public Entity MapToEntity()
		{
			Entity entity = Activator.CreateInstance<Entity>();

			List<PropertyInfo> properties = typeof(Entity).GetProperties().ToList();
			foreach (PropertyInfo property in properties) 
			{
				ColumnDescription? columnDescription =  property.GetCustomAttribute<ColumnDescription>();
				if (columnDescription is null)
				{
					//TODO throw exception;
				}

				object columnValue = sqlDataReader[columnDescription.ColumnName];
				property.SetValue(entity, columnValue, null);
			}

			return entity;
		}
	}
}
