using System.Threading.Tasks;
using TwitchLogger.Data.Models;

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
                await ctx.Users.AddAsync(entity);
            }
            else if (entity.Login != user.Login)
            {
                entity.Login = user.Login;
            }

            return entity;
        }
    }
}
