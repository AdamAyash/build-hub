using System.ComponentModel;

namespace BuildHub.DataEngine.Queries
{
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
