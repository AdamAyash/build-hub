namespace UnitTests.CommonTests.Utilities
{
	using BuildHub.Common.Utilities;
	using System.ComponentModel;

	[TestClass]
	public sealed class UtilitiesTests
	{
		private enum TestEnumeration
		{
			[Description("TestDescription")]
			Test,
			InvalidTest
		}

		[TestMethod]
		public void GetEnumDescriptionTest()
		{
			Assert.AreEqual("TestDescription", Utilities.GetEnumDescription<TestEnumeration>(TestEnumeration.Test));
		}

		[TestMethod]
		public void GetNotExistingEnumDescriptionTest()
		{
			Assert.AreEqual(string.Empty, Utilities.GetEnumDescription<TestEnumeration>(TestEnumeration.InvalidTest));
		}

		[TestMethod]
		[DataRow("Buid")]
		[DataRow("User Password")]
		[DataRow("Build -  Hub")]
		public void StringifyTest(string text)
		{
			Assert.AreEqual<string>($"'{text}'", Utilities.Stringify(text));
		}
	}
}
