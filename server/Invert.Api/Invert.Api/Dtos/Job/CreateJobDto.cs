using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Invert.Api.Dtos.Job
{
    public enum EmploymentType { Unknown, FullTime, PartTime, Contract, Hybrid }
    public enum ExperienceLevel { Unknown, Junior, Mid, Senior }
    public enum JobStatus { Unknown, Available, NotAvailable }

    public record CreateJobDto
    {
        [Required, MaxLength(200)]
        public string Title { get; init; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EmploymentType EmploymentType { get; init; } = EmploymentType.Unknown;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ExperienceLevel ExperienceLevel { get; init; } = ExperienceLevel.Unknown;

        [Range(0, double.MaxValue)]
        public decimal? Salary { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobStatus Status { get; init; } = JobStatus.Unknown;

        public DateTime? DatePosted { get; init; }
        public DateTime? ClosingDate { get; init; }
        public string? Description { get; init; }

        // Frontend sends these as JSON arrays
        public string[] Requirements { get; init; } = Array.Empty<string>();
        public string[] Skills { get; init; } = Array.Empty<string>();
        public string[] Benefits { get; init; } = Array.Empty<string>();
    }
}
