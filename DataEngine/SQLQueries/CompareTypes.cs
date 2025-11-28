using System.ComponentModel;

namespace BuildHub.DataEngine.SQLQueries
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
