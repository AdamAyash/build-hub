using BuildHub.CommandBuilder.Models;
using BuildHub.CommandBuilder.Models.Execution.Enums;

namespace UnitTests.CommandBuilders.Models
{
	[TestClass]
	public class RetryPolicyTests
	{
		[TestMethod]
		public void Defaults_AreCorrect()
		{
			var p = new RetryPolicy();

			Assert.AreEqual(1, p.MaxAttempts);
			Assert.AreEqual(TimeSpan.Zero, p.Delay);
			Assert.AreEqual(RetryOn.FailuresOnly, p.RetryOn);
		}

		[TestMethod]
		public void ToString_ContainsKeyParts()
		{
			var p = new RetryPolicy
			{
				MaxAttempts = 3,
				Delay = TimeSpan.FromSeconds(2),
				RetryOn = RetryOn.FailuresAndTimeouts
			};

			var s = p.ToString();

			StringAssert.Contains(s, "Retry policy:");
			StringAssert.Contains(s, "Max retries: 3");
			StringAssert.Contains(s, "timeout: 00:00:02");
			StringAssert.Contains(s, "FailuresAndTimeouts");
		}
	}
}
