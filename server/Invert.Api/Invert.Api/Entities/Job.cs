using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Invert.Api.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
        public decimal? Salary { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DatePosted { get; set; }
        public DateTime ClosingDate { get; set; }
        public string Description { get; set; } = string.Empty;

        // Store as JSON strings in database
        public string RequirementsJson { get; set; } = "[]";
        public string SkillsJson { get; set; } = "[]";
        public string BenefitsJson { get; set; } = "[]";


        [NotMapped]
        public string[] Requirements
        {
            get => SafeDeserializeArray(RequirementsJson);
            set => RequirementsJson = JsonSerializer.Serialize(value ?? Array.Empty<string>());
        }

        [NotMapped]
        public string[] Skills
        {
            get => SafeDeserializeArray(SkillsJson);
            set => SkillsJson = JsonSerializer.Serialize(value ?? Array.Empty<string>());
        }

        [NotMapped]
        public string[] Benefits
        {
            get => SafeDeserializeArray(BenefitsJson);
            set => BenefitsJson = JsonSerializer.Serialize(value ?? Array.Empty<string>());
        }

        private static string[] SafeDeserializeArray(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return Array.Empty<string>();

            try
            {
                return JsonSerializer.Deserialize<string[]>(json) ?? Array.Empty<string>();
            }
            catch
            {
                var s = json.Trim();

                if (s.Contains(","))
                {
                    return s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim()).Where(x => x.Length > 0).ToArray();
                }

                s = s.Trim('"');

                return new[] { s };
            }
        }
    }
}



// Link to AppUser

//        [NotMapped]
//        public string[] Requirements
//        {
//            get => string.IsNullOrEmpty(RequirementsJson) ? Array.Empty<string>()
//                   : JsonSerializer.Deserialize<string[]>(RequirementsJson) ?? Array.Empty<string>();
//            set => RequirementsJson = JsonSerializer.Serialize(value ?? Array.Empty<string>());
//        }

//        [NotMapped]
//        public string[] Skills
//        {
//            get => string.IsNullOrEmpty(SkillsJson) ? Array.Empty<string>()
//                   : JsonSerializer.Deserialize<string[]>(SkillsJson) ?? Array.Empty<string>();
//            set => SkillsJson = JsonSerializer.Serialize(value ?? Array.Empty<string>());
//        }

//        [NotMapped]
//        public string[] Benefits
//        {
//            get => string.IsNullOrEmpty(BenefitsJson) ? Array.Empty<string>()
//                   : JsonSerializer.Deserialize<string[]>(BenefitsJson) ?? Array.Empty<string>();
//            set => BenefitsJson = JsonSerializer.Serialize(value ?? Array.Empty<string>());
//        }
//    }
//}
