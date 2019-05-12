using System;

namespace CoreBot.Exceptions
{
    class AuthorizationException : Exception
    {
        public AuthorizationException(string message)
            : base (message)
        {

        }
    }
}
