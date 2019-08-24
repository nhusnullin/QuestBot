//using CoreBot.Domain;
//using CoreBot.Exceptions;
//using CoreBot.Properties;
//using CoreBot.Service;
//using System.Threading.Tasks;
//
//namespace CoreBot
//{
//    static class AuthorizationUtils
//    {
//        public static async Task<User> ValidateCaptainPermission(UserId userId, IUserService userService, ITeamService teamService)
//        {
//            var user = await userService.ValidateUser(userId);
//            await ValidateCaptainPermission(user, userService, teamService);
//            return user;
//        }
//
//        private static Task ValidateCaptainPermission(User user, IUserService userService, ITeamService teamService)
//        {
//            if (teamService.TryGetTeamId(user) == null)
//                throw new AuthorizationException(Resources.ChoicePlayMode);
//            if (!user.IsCaptain)
//                throw new AuthorizationException(Resources.CaptainRequiredPermission);
//            return Task.CompletedTask;
//        }
//
//    }
//}

