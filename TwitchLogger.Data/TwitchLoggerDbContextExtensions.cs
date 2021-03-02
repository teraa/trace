using System.Threading.Tasks;
using TwitchLogger.Data.Models.Twitch;

namespace TwitchLogger.Data
{
    public static class TwitchLoggerDbContextExtensions
    {
        public static async Task<User> CreateOrUpdateUserAsync(this TwitchLoggerDbContext ctx, User user)
        {
            var entity = await ctx.Users.FindAsync(user.Id);
            if (entity is null)
            {
                entity = user;
                ctx.Users.Add(entity);
            }
            else if (entity.Login != user.Login)
            {
                entity.Login = user.Login;
            }

            return entity;
        }
    }
}
