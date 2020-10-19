using System;
using DocumentManager.Core.Interfaces;
using DocumentManager.Core.Providers;
using Moq;
using Shouldly;
using Xunit;

namespace DocumentManager.Core.Tests.Providers
{
    public class DateTimeProviderTests
    {
        [Fact]
        public void DateTimeProvider_UtcNow_ShouldReturnMockedUtcNow()
        {
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.UtcNow).Returns(new DateTime(2020, 12, 31, 23, 58, 59));

            var result = dateTimeProvider.Object.UtcNow;

            result.ShouldBeOfType<DateTime>();
            result.ShouldNotBe(new DateTime());
            result.ShouldNotBe(default);
            result.Year.ShouldBe(2020);
            result.Month.ShouldBe(12);
            result.Day.ShouldBe(31);
            result.Hour.ShouldBe(23);
            result.Minute.ShouldBe(58);
            result.Second.ShouldBe(59);
            result.ShouldBe(new DateTime(2020, 12, 31, 23, 58, 59));
        }

        [Fact]
        public void DateTimeProvider_UtcNow_ShouldReturnActualUtcNow()
        {
            var now = DateTime.UtcNow;
            var dateTimeProvider = new DateTimeProvider();

            var result = dateTimeProvider.UtcNow;

            result.ShouldNotBe(new DateTime());
            result.ShouldNotBe(default);
            result.ShouldBeInRange(now, now.AddSeconds(1));
        }
    }
}