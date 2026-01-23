namespace BuildHub.DataEngine.Exceptions.Entities
{
	/// <summary>
	/// Represents an exception that is thrown when an operation attempts to access an entity that does not exist.
	/// </summary>
	/// <remarks>This exception is typically used to indicate that a requested entity could not be found in a data
	/// store or repository. It can be caught to handle scenarios where missing entities are expected or require special
	/// handling.</remarks>
	public class EntityDoesNotExistException : Exception
	{
		public EntityDoesNotExistException()
			: base()
		{
		}
	}
}
