using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace UserDirectory.Api.Authorization;

public static class ScopeAuthorization
{
    public const string PolicyName = "ApiScope";
    public const string Scope = "access_as_user";

    public static void AddApiScopePolicy(this AuthorizationOptions options)
    {
        options.AddPolicy(PolicyName, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireScope(Scope);
        });
    }
}
