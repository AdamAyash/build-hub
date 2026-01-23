using System.ComponentModel;

namespace BuildHub.DataEngine.Queries
{
	/// <summary>
	/// Compare types for SQL Queries
	/// </summary>
	public enum CompareTypes
	{
		[Description("=")]
		Equal,
		[Description("<>")]
		NotEqual,
		[Description(">")]
		GreaterThan,
		[Description("<")]
		LessThan,
		[Description(">=")]
		GreaterThanOrEqual,
		[Description("<=")]
		LessThanOrEqual
	}
}
