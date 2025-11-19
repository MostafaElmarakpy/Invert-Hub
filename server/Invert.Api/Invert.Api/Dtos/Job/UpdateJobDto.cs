using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Invert.Api.Dtos.Job
{
    public record UpdateJobDto
    {
        [MaxLength(200)]
        public string? Title { get; init; }

        [MaxLength(200)]
        public string? Location { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EmploymentType? EmploymentType { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExperienceLevel? ExperienceLevel { get; init; }

        [Range(0, double.MaxValue)]
        public decimal? Salary { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobStatus? Status { get; init; }

        public DateTime? DatePosted { get; init; }
        public DateTime? ClosingDate { get; init; }
        public string? Description { get; init; }

        // Frontend sends these as JSON arrays
        public string[]? Requirements { get; init; }
        public string[]? Skills { get; init; }
        public string[]? Benefits { get; init; }
    }
}
