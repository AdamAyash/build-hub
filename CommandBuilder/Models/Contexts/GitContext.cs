namespace BuildHub.CommandBuilder.Models.Contexts;

/// <summary>
/// Context for Git command builder.
/// Holds repository-specific information and checkout options.
/// </summary>
public class GitContext
{
    public string Repository { get; set; } = default!;
    public string Branch { get; set; } = "main";
    public string? Version { get; set; } = null;
    public string TargetDirectory { get; set; } = default!;
    public bool Clean { get; set; } = false;
}

