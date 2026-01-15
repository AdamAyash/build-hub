using BuildHub.CommandBuilder.Models;

namespace BuildHub.CommandBuilder.CommandBuilders.Abstractions;

/// <summary>
/// Defines a builder responsible for producing a command-line string
/// and validating its required inputs.
/// </summary>
public interface ICommandBuilder
{
    /// <summary>
    /// Generates the full command string based on the configuration.
    /// </summary>
    /// <returns>The generated command-line string.</returns>
    BuildCommandResult GenerateCommand();

    /// <summary>
    /// Validates the builder state and throws an exception if required
    /// values are missing or invalid.
    /// </summary>
    void ValidateAll();
}
