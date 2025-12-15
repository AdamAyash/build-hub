namespace BuildHub.DataEngine.Entities
{
	/// <summary>
	/// Specifies that the decorated property or field is part of the primary key for the containing entity.
	/// </summary>
	/// <remarks>Apply this attribute to a property or field to indicate that it uniquely identifies an instance of
	/// the entity, typically for use with object-relational mapping frameworks or data access layers.</remarks>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class PrimaryKey : Attribute
	{
		public PrimaryKey()
		{
		}
	}
}
