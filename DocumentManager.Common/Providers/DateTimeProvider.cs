using System;
using DocumentManager.Common.Interfaces;

namespace DocumentManager.Common.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }
}
