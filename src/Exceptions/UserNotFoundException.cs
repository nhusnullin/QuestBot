using CoreBot.Domain;
using System;
using Core.Domain;

namespace CoreBot.Exceptions
{
    class UserNotFoundException: Exception
    {
        public UserNotFoundException(UserId userId)
        {
            UserId = userId;
        }

        public UserId UserId { get; }
    }
}
