using System.Collections.Generic;

namespace DocumentManager.Core.Models
{
    public class ValidatorSettings
    {
        public const string SectionName = "Validation";
        public int MaximumFileSizeInBytes { get; set; }
        public List<string> AllowedContentTypes { get; set; }
    }
}
