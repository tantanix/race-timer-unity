using System;

namespace Tcs.RaceTimer.Exceptions
{
    public class AutoTimingException : Exception
    {
        public AutoTimingException() { }

        public AutoTimingException(string message) : base(message) { }
    }
}
