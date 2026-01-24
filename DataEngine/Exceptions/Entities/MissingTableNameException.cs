namespace BuildHub.DataEngine.Exceptions.Entities
{
	public class MissingTableNameException : Exception
	{
		public MissingTableNameException(Type entityType)
			: base($"The entity {entityType.Name} doesn't have table name attribute.")
		{
		}
	}
}
