using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public enum DatabaseSource
{
	[Description("BuildHubCore")]
	[Required]
	Core = 0,

	[Description("BuildHubUsers")]
	[Required]
	Users = 1,

	[Description("BuildHubUnitTests")]
	UnitTests
}