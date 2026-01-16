using BuildHub.CommandBuilder.Models.Execution.Enums;

namespace TestProject.CommandBuilders.General
{
	[TestClass]
	public class VisualStudionInfoTests
	{

		[TestMethod]
		[DataRow(VisualStudioVersions.VS2008, "vc9")]
		[DataRow(VisualStudioVersions.VS2010, "vc10")]
		[DataRow(VisualStudioVersions.VS2017, "vc15")]
		[DataRow(VisualStudioVersions.VS2019, "vc16")]
		[DataRow(VisualStudioVersions.VS2022, "vc17")]
		[DataRow(null, "vc15")]
		public void GetCodeName_ReturnsExpectedCodeName(VisualStudioVersions? tool, string expectedCodeName)
		{
			var result = VisualStudioInfo.GetCodeName(tool);
			Assert.AreEqual(expectedCodeName, result);
		}

		[TestMethod]
		public void GetCodeName_Null_ReturnsDefault()
		{
			var result = VisualStudioInfo.GetCodeName(null);
			Assert.AreEqual("vc15", result);
		}

		[TestMethod]
		public void GetCodeName_UnknownEnumValue_ReturnsDefault()
		{
			var unknown = (VisualStudioVersions)999;
			var result = VisualStudioInfo.GetCodeName(unknown);
			Assert.AreEqual("vc15", result);
		}

		[TestMethod]
		[DataRow("vc9", VisualStudioVersions.VS2008)]
		[DataRow("vc10", VisualStudioVersions.VS2010)]
		[DataRow("vc15", VisualStudioVersions.VS2017)]
		[DataRow("vc16", VisualStudioVersions.VS2019)]
		[DataRow("vc17", VisualStudioVersions.VS2022)]
		public void GetVersionFromCodeName_ValidInput_ReturnsExpectedEnum(
			string input,
			VisualStudioVersions expected)
		{
			var result = VisualStudioInfo.GetVersionFromCodeName(input);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		[DataRow("")]
		[DataRow(null)]
		[DataRow("vc11")]
		[DataRow("VC15")]
		[DataRow("invalid")]
		public void GetVersionFromCodeName_InvalidInput_ThrowsFormatException(string input)
		{
			Assert.Throws<FormatException>(
				() => VisualStudioInfo.GetVersionFromCodeName(input));
		}
	}
}