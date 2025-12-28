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
		public void Enum_Description_Should_Match()
		{
			Assert.AreEqual("TestDescription", Utilities.GetEnumDescription<TestEnumeration>(TestEnumeration.Test));
		}

		[TestMethod]
		public void Aseert_That_Enum_Without_Description_Returns_Empty_String()
		{
			Assert.AreEqual(string.Empty, Utilities.GetEnumDescription<TestEnumeration>(TestEnumeration.InvalidTest));
		}

		[TestMethod]
		[DataRow("Build")]
		[DataRow("User Password")]
		[DataRow("Build -  Hub")]
		public void Stringify_And_Match(string text)
		{
			Assert.AreEqual<string>($"'{text}'", Utilities.Stringify(text));
		}
	}
}
