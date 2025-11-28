using System.ComponentModel;

namespace BuildHub.DataEngine.SQLQueries
{
	public enum LockTypes
	{
		[Description("NOLOCK")]
		None,
		[Description("UPDLOCK")]
		Update
	}
}
