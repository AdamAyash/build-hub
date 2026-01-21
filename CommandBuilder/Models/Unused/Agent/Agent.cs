using BuildHub.CommandBuilder.Models.Unused.Agent.Enums;

namespace BuildHub.CommandBuilder.Models.Unused.Agent;

/// <summary>
/// Represents a machine capable of executing pipeline steps.
/// </summary>
public class Agent
{
	/// <summary>
	/// Unique identifier of the build agent.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Human-readable Name for the build agent.
	/// </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Network or DNS hostname of the machine.
	/// </summary>
	public string HostName { get; set; } = string.Empty;

	/// <summary>
	/// Indicates whether the agent is currently online and reachable.
	/// </summary>
	public bool IsOnline { get; set; }

	/// <summary>
	/// List of installed tools on the agent (e.g., MSBuild, .NET SDK, Node.js).
	/// </summary>
	public List<AgentCapability> InstalledTools { get; set; } = new();

	/// <summary>
	/// Current status of the agent (Idle, Busy, Offline, Error).
	/// </summary>
	public AgentStatus Status { get; set; } = AgentStatus.Offline;

	/// <summary>
	/// Last time the agent reported its status (heartbeat).
	/// </summary>
	public DateTime LastHeartbeat { get; set; }

	/// <summary>
	/// Build profiles for different flavors (Debug, Release, etc.).
	/// Each profile defines a BasePath, environment variables, and optional timeout.
	/// </summary>
	public List<BuildProfile> BuildProfiles { get; init; } = new List<BuildProfile>();
}
