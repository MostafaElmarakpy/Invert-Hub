using System.Text.Json;
using System.Text.Json.Serialization;

namespace Invert.Api.Dtos.Article
{
    /// <summary>
    /// DTO for returning article data to frontend
    /// </summary>
    public class ArticleDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("contentJson")]
        public string? ContentJson { get; init; }

        [JsonPropertyName("time")]
        public long Time { get; init; }

        [JsonPropertyName("blocks")]
        public List<BlockDto> Blocks { get; init; } = new();

        [JsonPropertyName("version")]
        public string Version { get; init; } = "2.28.2";

        [JsonPropertyName("author")]
        public string? Author { get; init; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; init; }

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; init; }

        // Static method to create from Entity
        public static ArticleDto FromEntity(Entities.Article article)
        {
            var editorData = JsonSerializer.Deserialize<EditorJsData>(article.ContentJson);

            return new ArticleDto
            {
                Id = article.Id,
                Time = editorData?.Time ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                Blocks = editorData?.Blocks ?? new List<BlockDto>(),
                Version = editorData?.Version ?? "2.28.2",
                Author = article.Author,
                CreatedAt = article.CreatedAt,
                UpdatedAt = article.UpdatedAt
            };
        }
    }

    // Helper class for deserializing EditorJS JSON
    public class EditorJsData
    {
        [JsonPropertyName("time")]
        public long Time { get; set; }

        [JsonPropertyName("blocks")]
        public List<BlockDto> Blocks { get; set; } = new();

        [JsonPropertyName("version")]
        public string Version { get; set; } = "2.28.2";
    }
}
