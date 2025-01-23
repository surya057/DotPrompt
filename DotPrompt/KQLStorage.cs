/// <summary>
/// Implementation of the IPromptStore for Azure Storage Tables
/// </summary>
public class KQLPromptStore : IPromptStore
{
    /// <summary>
    /// Loads the prompts from the table store
    /// </summary>
    public IEnumerable<PromptFile> Load()
    {
        var tableClient = GetTableClient();
        var promptEntities = tableClient.Query<PromptEntity>(e => e.PartitionKey == "DotPromptTest");

        var promptFiles = promptEntities
            .Select(pe => pe.ToPromptFile())
            .ToList();

        return promptFiles;
    }

    /// <summary>
    /// Gets a table client
    /// </summary>
    private static TableClient GetTableClient()
    {
        // Replace the configuration items here with your value or switch to using
        // Entra based authentication
        var client = new TableServiceClient(
            new Uri($"https://{Configuration.StorageAccountName}.table.core.windows.net/"),
            new TableSharedKeyCredential(Configuration.StorageAccountName, Configuration.StorageAccountKey)
        );

        var tableClient = client.GetTableClient("prompts");
        tableClient.CreateIfNotExists();

        return tableClient;
    }
}