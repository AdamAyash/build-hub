using System.ComponentModel;

namespace BuildHub.DataEngine.Queries
{
	public enum LockTypes
	{
		[Description("NOLOCK")]
		None,
		[Description("UPDLOCK")]
		Update
	}
}
