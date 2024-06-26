using System.Net;
using BlockInfrastructure.Common;
using BlockInfrastructure.Common.Models.Errors;
using BlockInfrastructure.Common.Models.Internal;
using BlockInfrastructure.Common.Services;
using BlockInfrastructure.Core.Models.Requests;
using BlockInfrastructure.Core.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace BlockInfrastructure.Core.Services;

public interface IUserService
{
    Task<MeProjection> GetMeAsync(ContextUser contextUser);
    Task UpdatePreferenceAsync(ContextUser contextUser, UpdateUserPreferenceRequest updateUserPreferenceRequest);
    Task<List<UserSearchResponse>> SearchUserAsync(string query);
    Task DeleteUserAsync(string userId);
    Task<List<AuditLog>> GetAuditLogAsync(string userId);
}

public class UserService(DatabaseContext databaseContext, ICacheService cacheService) : IUserService
{
    public async Task<MeProjection> GetMeAsync(ContextUser contextUser)
    {
        var data = await cacheService.GetOrSetAsync(CacheKeys.UserMeProjectionKey(contextUser.UserId), async () =>
        {
            // Get User
            var user = await databaseContext.Users
                                            .Include(a => a.ChannelPermissionList)
                                            .ThenInclude(a => a.Channel)
                                            .Where(a => a.Id == contextUser.UserId)
                                            .SingleOrDefaultAsync() ?? throw new ApiException(HttpStatusCode.NotFound,
                "Unknown error: Cannot find user!", UserError.UserNotFound);
            return MeProjection.FromUser(user);
        }, TimeSpan.FromDays(10));

        return data!;
    }

    public async Task UpdatePreferenceAsync(ContextUser contextUser, UpdateUserPreferenceRequest updateUserPreferenceRequest)
    {
        // Get User
        var user = await databaseContext.Users.FindAsync(contextUser.UserId) ?? throw new ApiException(HttpStatusCode.NotFound,
            "Unknown error: Cannot find user!", UserError.UserNotFound);

        // Update User
        user.Name = updateUserPreferenceRequest.Name;
        user.Email = updateUserPreferenceRequest.Email;
        user.ProfilePictureImageUrl = updateUserPreferenceRequest.ProfilePictureImageUrl;
        await databaseContext.SaveChangesAsync(); // Invalidation of Cache is controlled by DBContext.
    }

    public async Task<List<UserSearchResponse>> SearchUserAsync(string query)
    {
        var userList = await databaseContext.Users
                                            .Where(a => a.Email.Contains(query) || a.Name.Contains(query))
                                            .ToListAsync();

        return userList.Select(UserSearchResponse.FromUser).ToList();
    }

    public async Task DeleteUserAsync(string userId)
    {
        var user = await databaseContext.Users.FindAsync(userId) ?? throw new ApiException(HttpStatusCode.NotFound,
            "Unknown error: Cannot find user!", UserError.UserNotFound);

        databaseContext.Users.Remove(user);
        await databaseContext.SaveChangesAsync();
    }

    public async Task<List<AuditLog>> GetAuditLogAsync(string userId)
    {
        return await databaseContext.UserActions
                                    .Where(a => a.UserId == userId)
                                    .Select(a => new AuditLog
                                    {
                                        AuditName = a.ActionName,
                                        AuditTime = a.ActedAt
                                    })
                                    .ToListAsync();
    }
}