using System;
using System.Runtime.Serialization;

namespace TechTest.ConsoleApp.Exceptions
{
    public class NoConfigurationDataException : Exception
    {
        public NoConfigurationDataException()
        {
        }

        public NoConfigurationDataException(string message) : base(message)
        {
        }

        public NoConfigurationDataException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoConfigurationDataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
