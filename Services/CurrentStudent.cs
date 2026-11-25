using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace EngageTrack.Services;

public sealed class CurrentStudent
{
    private readonly IWebHostEnvironment _environment;

    public CurrentStudent(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public int? GetUserId(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated == true)
        {
            var value = user.FindFirst("EngageTrackUserId")?.Value;

            return int.TryParse(value, out var userId) && userId > 0
                ? userId
                : null;
        }
        return _environment.IsDevelopment() ? 1004 : null;
    }

    public int? GetStaffUserId(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated == true)
        {

            return GetUserId(user);
        }

        return _environment.IsDevelopment() ? 2002 : null;
    }
}
