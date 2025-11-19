using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Invert.Api.Dtos.Article
{
    public class UpdateArticleDto
    {
        [JsonPropertyName("time")]
        public long? Time { get; init; }

        [JsonPropertyName("blocks")]
        public List<BlockDto>? Blocks { get; init; }

        [JsonPropertyName("version")]
        public string? Version { get; init; }

        [MaxLength(200)]
        [JsonPropertyName("author")]
        public string? Author { get; init; }

        // Computed properties
        [JsonIgnore]
        public string? Title
        {
            get
            {
                if (Blocks == null || Blocks.Count == 0) return null;
                var headerBlock = Blocks.FirstOrDefault(b => b.Type == "header");
                return headerBlock?.Data.GetProperty("text").GetString();
            }
        }

        [JsonIgnore]
        public string? ContentJson
        {
            get
            {
                if (Blocks == null) return null;
                return JsonSerializer.Serialize(new
                {
                    time = Time ?? DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    blocks = Blocks,
                    version = Version ?? "2.28.2"
                });
            }
        }
    }
}
