using System.Reflection;

namespace BuildHub.DataEngine.Exceptions.Entities
{
	public class MissingColumnDescriptionException : Exception
	{
		public MissingColumnDescriptionException(PropertyInfo property)
			: base($"The property '{property.Name}' of entity '{property.DeclaringType}' has no description.")
		{
		}
	}
}
