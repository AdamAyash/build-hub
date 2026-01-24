using System.ComponentModel;

namespace BuildHub.DataEngine.Queries
{
	/// <summary>
	/// SQL Lock types.
	/// </summary>
	public enum LockTypes
	{
		[Description("NOLOCK")]
		None,
		[Description("UPDLOCK")]
		Update
	}
}
