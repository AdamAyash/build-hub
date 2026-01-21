using BuildHub.CommandBuilder.Models;

namespace UnitTests.CommandBuilders.Models
{
	[TestClass]
	public sealed class TimeoutPolicyTests
	{
		[TestMethod]
		public void ToString_ContainsKeyParts()
		{
			var p = new TimeoutPolicy
			{
				Timeout = TimeSpan.FromMilliseconds(250),
				KillProcessTree = true
			};

			var s = p.ToString();

			StringAssert.Contains(s, "Timeout policy:");
			StringAssert.Contains(s, "Timeout: 00:00:00.250");
			StringAssert.Contains(s, "kill process: True");
		}
	}
}
