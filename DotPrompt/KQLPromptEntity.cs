/// <summary>
/// Represents a record held in the storage table
/// </summary>
public class KQLPromptEntity : ITableEntity
{
    /// <summary>
    /// Gets, sets the partition key for the record
    /// </summary>
    public string PartitionKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets, sets the row key for the record
    /// </summary>
    public string RowKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets, sets the timestamp of the entry
    /// </summary>
    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Gets, sets the records ETag value
    /// </summary>
    public ETag ETag { get; set; }

    /// <summary>
    /// Gets, sets the model to use
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    /// Gets, sets the output format
    /// </summary>
    public string OutputFormat { get; set; } = string.Empty;

    /// <summary>
    /// Gets, sets the maximum number of tokens
    /// </summary>
    public int MaxTokens { get; set; }

    /// <summary>
    /// Gets, sets the parameter information which is held as a JSON string value
    /// </summary>
    public string Parameters { get; set; } = string.Empty;

    /// <summary>
    /// Gets, sets the default values which are held as a JSON string value
    /// </summary>
    public string Default { get; set; } = string.Empty;

    /// <summary>
    /// Gets, sets the system prompt template
    /// </summary>
    public string SystemPrompt { get; set; } = string.Empty;

    /// <summary>
    /// Gets, sets the user prompt template
    /// </summary>
    public string UserPrompt { get; set; } = string.Empty;

    /// <summary>
    /// Returns the prompt entity record into a <see cref="PromptFile"/> instance
    /// </summary>
    /// <returns></returns>
    public PromptFile ToPromptFile()
    {
        var parameters = new Dictionary<string, string>();
        var defaults = new Dictionary<string, object>();

        // If there are parameter values then convert them into a dictionary
        if (!string.IsNullOrEmpty(Parameters))
        {
            var entityParameters = (JsonObject)JsonNode.Parse(Parameters)!;
            foreach (var (prop, propType) in entityParameters)
            {
                parameters.Add(prop, propType?.AsValue().ToString() ?? string.Empty);
            }
        }

        // If there are default values then convert them into a dictionary
        if (!string.IsNullOrEmpty(Default))
        {
            var entityDefaults = (JsonObject)JsonNode.Parse(Default)!;
            foreach (var (prop, defaultValue) in entityDefaults)
            {
                defaults.Add(prop, defaultValue?.AsValue().GetValue<object>() ?? string.Empty);
            }
        }

        // Generate the new prompt file
        var promptFile = new PromptFile
        {
            Name = RowKey,
            Model = Model,
            Config = new PromptConfig
            {
                OutputFormat = Enum.Parse<OutputFormat>(OutputFormat, true),
                MaxTokens = MaxTokens,
                Input = new InputSchema
                {
                    Parameters = parameters,
                    Default = defaults
                }
            },
            Prompts = new Prompts
            {
                System = SystemPrompt,
                User = UserPrompt
            }
        };

        return promptFile;
    }
}