using CoreBot.Domain;
using CoreBot.Exceptions;
using CoreBot.Properties;
using CoreBot.Service;
using System.Threading.Tasks;

namespace CoreBot
{
    static class AuthorizationUtils
    {
        public static async Task<User> ValidateCaptainPermission(UserId userId, IUserService userService, ITeamService teamService)
        {
            var user = await userService.GetByAsync(userId.ChannelId, userId.Id);
            if (teamService.TryGetTeamId(user) == null)
                throw new AuthorizationException(Resources.ChoicePlayMode);
            if (!user.IsCaptain)
                throw new AuthorizationException(Resources.CaptainRequiredPermission);
            return user;
        }

    }
}
