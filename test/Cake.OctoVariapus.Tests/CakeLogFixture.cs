using System.Collections.Generic;

using Cake.Core.Diagnostics;

namespace Cake.OctoVariapus.Tests
{
    public class CakeLogFixture : ICakeLog
    {
        public CakeLogFixture()
        {
            Messages = new List<MessageWrapper>();
        }

        public List<MessageWrapper> Messages { get; }

        public void Write(Verbosity verbosity, LogLevel level, string format, params object[] args)
        {
            Messages.Add(new MessageWrapper(verbosity, level, format, args));
        }

        public Verbosity Verbosity { get; set; }
    }

    public class MessageWrapper
    {
        public MessageWrapper(Verbosity verbosity, LogLevel logLevel, string format, object[] arguments)
        {
            Verbosity = verbosity;
            LogLevel = logLevel;
            Format = format;
            Arguments = arguments;
        }

        public object[] Arguments { get; }

        public string Format { get; }

        public LogLevel LogLevel { get; }

        public Verbosity Verbosity { get; }
    }
}
