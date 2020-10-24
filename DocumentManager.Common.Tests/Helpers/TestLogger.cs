using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DocumentManager.Core.Tests.Helpers
{
    public class TestLogger<T> : ILogger<T>
    {
        public IList<string> Logs;

        public TestLogger()
        {
            this.Logs = new List<string>();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel) => false;


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            this.Logs.Add(message);
        }
    }
}

