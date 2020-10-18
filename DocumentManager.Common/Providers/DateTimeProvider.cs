using System;
using DocumentManager.Core.Interfaces;

namespace DocumentManager.Core.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow() => DateTime.UtcNow;
    }
}
