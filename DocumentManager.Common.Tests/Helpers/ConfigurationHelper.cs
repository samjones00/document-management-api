using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DocumentManager.Core.Tests.Helpers
{
    public static class ConfigurationHelper
    {
        public static void SetupMaximumFileSizeInBytes(Mock<IConfiguration> configuration, int bytes)
        {
            var section1 = new Mock<IConfigurationSection>();
            section1.Setup(a => a.Value).Returns(bytes.ToString);

            configuration.Setup(a =>
                    a.GetSection(
                        $"{Core.Constants.ValidatorSettings.SectionName}:{Core.Constants.ValidatorSettings.MaximumFileSizeInBytes}"))
                .Returns(section1.Object);
        }

        public static void SetupAllowedContentTypes(Mock<IConfiguration> configuration, params string[] allowedTypes)
        {
            var configurationSections = new List<IConfigurationSection>();

            foreach (var allowedType in allowedTypes)
            {
                var section = new Mock<IConfigurationSection>();
                section.Setup(s => s.Value).Returns(allowedType);
                configurationSections.Add(section.Object);
            }

            var sectionGroup = new Mock<IConfigurationSection>();
            sectionGroup.Setup(s => s.GetChildren())
                .Returns(configurationSections);

            configuration.Setup(a =>
                    a.GetSection(
                        $"{Core.Constants.ValidatorSettings.SectionName}:{Core.Constants.ValidatorSettings.AllowedContentTypes}"))
                .Returns(sectionGroup.Object);
        }
    }
}
