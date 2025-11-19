using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

public class CreateArticleDto
{
    // EditorJS fields
    [JsonPropertyName("time")]
    public long? Time { get; init; }

    [Required]
    [JsonPropertyName("blocks")]
    public List<BlockDto> Blocks { get; init; } = new();

    [JsonPropertyName("version")]
    public string? Version { get; init; }

    // Additional fields
    [MaxLength(200)]
    [JsonPropertyName("author")]
    public string? Author { get; init; }

    // Computed property to extract title from first header block
    [JsonIgnore]
    public string Title
    {
        get
        {
            var headerBlock = Blocks?.FirstOrDefault(b => b.Type == "header");
            return headerBlock?.Data.GetProperty("text").GetString() ?? "Untitled";
        }
    }

    // Computed property to serialize to JSON
    [JsonIgnore]
    public string ContentJson
    {
        get
        {
            return System.Text.Json.JsonSerializer.Serialize(new
            {
                time = Time ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                blocks = Blocks,
                version = Version ?? "2.28.2"
            });
        }
    }
}

public class BlockDto
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [Required]
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [Required]
    [JsonPropertyName("data")]
    public JsonElement Data { get; init; }
}
