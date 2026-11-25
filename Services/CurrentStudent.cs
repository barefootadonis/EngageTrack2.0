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
            // LOGIN IMPLEMENTATION:
            // When signing in, add this claim to the authenticated identity:
            // new Claim("EngageTrackUserId", databaseUser.UserId.ToString())
            //
            // databaseUser must be the Users record matched by your login code.
            // Use the integer UserId from EngageTrack's Users table.
            var value = user.FindFirst("EngageTrackUserId")?.Value;

            return int.TryParse(value, out var userId) && userId > 0
                ? userId
                : null;
        }

        // Remove this development fallback once login is connected.
        return _environment.IsDevelopment() ? 1004 : null;
    }

    public int? GetStaffUserId(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated == true)
        {
            // Login supplies the same EngageTrackUserId claim for students and staff.
            return GetUserId(user);
        }

        // Local development only: Samir Ahmed, seeded supervisor.
        // Remove this fallback when staff login is connected.
        return _environment.IsDevelopment() ? 2002 : null;
    }
}