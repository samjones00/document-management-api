using System;

namespace DocumentManager.Common.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow();
    }
}