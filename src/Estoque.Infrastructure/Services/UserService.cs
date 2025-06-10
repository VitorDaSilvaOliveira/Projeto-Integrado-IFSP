using Estoque.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;

namespace Estoque.Infrastructure.Services;

public class UserService(EstoqueDbContext context, IWebHostEnvironment env)
{
    public byte[]? GetUserAvatarBytes(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return null;

        var avatarFileName = context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.AvatarFileName) 
            .FirstOrDefault();

        if (string.IsNullOrEmpty(avatarFileName))
            return null;

        var avatarPath = Path.Combine(env.WebRootPath, "uploads", "avatars", avatarFileName);

        if (!File.Exists(avatarPath))
            return null;

        return File.ReadAllBytes(avatarPath);
    }
}