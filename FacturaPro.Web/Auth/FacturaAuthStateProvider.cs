// TP9 — Custom AuthenticationStateProvider
// Bridge between SessionUtilisateur and Blazor's built-in auth system.
//
// How it works:
//   1. Blazor calls GetAuthenticationStateAsync() to know who is logged in.
//   2. We read from SessionUtilisateur (our Scoped session service).
//   3. We convert Utilisateur → ClaimsPrincipal (Blazor's auth currency).
//   4. On login/logout, NotifyAuthenticationStateChanged() re-evaluates
//      all <AuthorizeView> blocks and [Authorize] pages automatically.
//
// Registered in Program.cs:
//   builder.Services.AddScoped<AuthenticationStateProvider, FacturaAuthStateProvider>();

using FacturaPro.Application.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace FacturaPro.Web.Auth;

public class FacturaAuthStateProvider : AuthenticationStateProvider
{
    private readonly SessionUtilisateur _session;

    public FacturaAuthStateProvider(SessionUtilisateur session)
    {
        _session = session;
        // Subscribe: whenever login/logout happens, notify Blazor's auth system
        _session.OnChange += NotifyChanged;
    }

    // TP9: Blazor calls this whenever it needs the current authentication state.
    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal principal;

        if (_session.EstConnecte && _session.Utilisateur != null)
        {
            // TP9: Build a ClaimsPrincipal — the standard .NET identity object.
            // Claims = key/value pairs describing the user.
            var claims = new List<Claim>
            {
                // ClaimTypes.Name   → User.Identity.Name in Razor / controllers
                new(ClaimTypes.Name,           _session.Utilisateur.NomUtilisateur),
                // ClaimTypes.Role   → [Authorize(Roles="Admin")] and <AuthorizeView Roles="Admin">
                new(ClaimTypes.Role,           _session.Utilisateur.Role),
                new(ClaimTypes.NameIdentifier, _session.Utilisateur.Id.ToString()),
                new("NomComplet",              _session.Utilisateur.NomComplet
                                               ?? _session.Utilisateur.NomUtilisateur),
                new("ClientId",                _session.Utilisateur.ClientId?.ToString() ?? "")
            };

            // TP9: Non-null authenticationType → IsAuthenticated = true
            var identity  = new ClaimsIdentity(claims, "FacturaAuth");
            principal     = new ClaimsPrincipal(identity);
        }
        else
        {
            // TP9: Empty ClaimsIdentity (no authenticationType) → anonymous / not logged in
            principal = new ClaimsPrincipal(new ClaimsIdentity());
        }

        return Task.FromResult(new AuthenticationState(principal));
    }

    // Called by SessionUtilisateur.OnChange → triggers re-render of all <AuthorizeView>
    private void NotifyChanged()
        => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
