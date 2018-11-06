using System;

namespace SIS.HTTP.Exceptions
{
    public class InteralServerErrorException : Exception
    {
        private const string DefaultMessage = "The Server has encountered an error.";

        public InteralServerErrorException()
            : base(DefaultMessage)
        {

        }

    }
}
