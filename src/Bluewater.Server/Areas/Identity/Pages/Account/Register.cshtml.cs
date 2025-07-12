using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

public class RegisterModel : PageModel
{
  private readonly UserManager<IdentityUser> _userManager;
  private readonly SignInManager<IdentityUser> _signInManager;
  private readonly RoleManager<IdentityRole> _roleManager;
  private readonly ILogger<RegisterModel> _logger;

  public RegisterModel(
      UserManager<IdentityUser> userManager,
      SignInManager<IdentityUser> signInManager,
      RoleManager<IdentityRole> roleManager,
      ILogger<RegisterModel> logger)
  {
    _userManager = userManager;
    _signInManager = signInManager;
    _roleManager = roleManager;
    _logger = logger;
  }

  [BindProperty]
  public InputModel Input { get; set; } = null!;

  /// <summary>
  /// Used to populate the `<select>` of available roles.
  /// </summary>
  public IList<SelectListItem> RoleOptions { get; private set; } = new List<SelectListItem>();

  public string ReturnUrl { get; set; } = "~/";

  public class InputModel
  {
    [Required]
    public required string Username { get; set; }

    [Required, DataType(DataType.Password)]
    public required string Password { get; set; }

    [DataType(DataType.Password), Compare(nameof(Password))]
    public required string ConfirmPassword { get; set; }

    [Required]
    [Display(Name = "Role")]
    public required string Role { get; set; }
  }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? Url.Content("~/");
        RoleOptions = _roleManager.Roles
            .Select(r => new SelectListItem
            {
                Value = r.Name!,
                Text = r.Name!
            })
            .ToList();

        await Task.CompletedTask; // Ensure all code paths return a Task
    }

  public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
  {
    returnUrl ??= Url.Content("~/");
    if (!ModelState.IsValid)
    {
      await OnGetAsync(returnUrl);
      return Page();
    }

    var user = new IdentityUser { UserName = Input.Username };
    var result = await _userManager.CreateAsync(user, Input.Password);
    if (result.Succeeded)
    {
      // 1) Assign the chosen role
      await _userManager.AddToRoleAsync(user, Input.Role);

      _logger.LogInformation("New user registered in role {Role}.", Input.Role);

      // 2) Sign in and redirect
      await _signInManager.SignInAsync(user, isPersistent: false);
      return LocalRedirect(returnUrl);
    }

    foreach (var error in result.Errors)
      ModelState.AddModelError(string.Empty, error.Description);

    // If we fail, re‑populate the dropdown and show errors
    await OnGetAsync(returnUrl);
    return Page();
  }
}
