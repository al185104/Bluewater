
using Bluewater.Core.Interfaces;
using Bluewater.Core.UserAggregate.Enum;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Bluewater.Server.Global;
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
  private readonly IEmployeeAuthService authService;
  private readonly ILocalStorageService localStorage;

  // private readonly IHttpContextAccessor _httpContextAccessor;
  // public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
  // {
  //     _httpContextAccessor = httpContextAccessor;
  // }

  public CustomAuthenticationStateProvider(IEmployeeAuthService _authService, ILocalStorageService _localStorage)
    {
        authService = _authService;
        localStorage = _localStorage;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Start with an anonymous state by default
        // var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        // return Task.FromResult(new AuthenticationState(anonymous));
        return Task.FromResult(new AuthenticationState(_currentUser));
    }

    public async Task MarkUserAsAuthenticated(string username, Credential credential)
    {
        await Task.Delay(1);

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, credential.ToString())
            // Additional claims can be added here if needed
        }, CookieAuthenticationDefaults.AuthenticationScheme);

        _currentUser = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }    

    public async Task LoginAsync(string username, string password)
    {        
        var ret = await authService.SignInAsync(username, password);

        //await localStorage.SetItemAsync("authToken", $"{username}_{password}_{DateTime.Now.ToLongTimeString()}");            
        await MarkUserAsAuthenticated(username, Credential.SuperAdmin);
        // // Temporarily approve any username/password
        // var identity = new ClaimsIdentity(new[]
        // {
        //     new Claim(ClaimTypes.Name, username),
        //     new Claim(ClaimTypes.Role, Credential.Admin.ToString())
        // }, "apiauth_type");

        // _currentUser = new ClaimsPrincipal(identity);
        // await Task.Delay(1);
        // // Sign in with the specified authentication scheme
        // // await _httpContextAccessor.HttpContext.SignInAsync(
        // //     CookieAuthenticationDefaults.AuthenticationScheme, user);

        // NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public async Task LogoutAsync()
    {
        // Clear the identity to log out the user
        // NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        await Task.CompletedTask;
    }
}




// public class CustomAuthenticationStateProvider : AuthenticationStateProvider
// {
//     private readonly IEmployeeAuthService _authService;
//     private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
//     public CustomAuthenticationStateProvider(IEmployeeAuthService authService)
//     {
//         _authService = authService;
//     }

//     public override Task<AuthenticationState> GetAuthenticationStateAsync()
//     {
//         return Task.FromResult(new AuthenticationState(_currentUser));
//     }

//   public async Task<bool> LoginAsync(string username, string password)
//     {
//         var ret = await _authService.SignInAsync(username, password);

//         if (ret != null && ret.IsSuccess)
//         {
//             var _username = ret.Value.Item1;
//             var _credentials = ret.Value.Item2;

//             var claims = new List<Claim>
//             {
//                 new Claim(ClaimTypes.Name, _username),
//                 new Claim(ClaimTypes.Role, _credentials.ToString())
//             };

//             var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
//             _currentUser = new ClaimsPrincipal(identity);

//             //await HttpContext.SignInAsync("Cookies", claimsPrincipal);
//             NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
//             return true;
//         }

//         return false;
//     }

//     public Task LogoutAsync()
//     {
//         _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
//         NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
//         return Task.CompletedTask;
//     }
// }
