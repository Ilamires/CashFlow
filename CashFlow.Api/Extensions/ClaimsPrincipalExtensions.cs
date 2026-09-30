using System.Security.Claims;

namespace CashFlow.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal user)
        => long.Parse(user.FindFirstValue("sub")!);
}