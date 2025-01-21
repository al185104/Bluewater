using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bluewater.Server.Areas.Pages.Account;

public class LoginModel : PageModel
  {
    private readonly SignInManager<IdentityUser> _signInManager; 
    public LoginModel(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    public async Task<IActionResult> OnPostAsync() {
        ReturnUrl = Url.Content("~/");

        if(ModelState.IsValid) {
            var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, false);

            if (result.Succeeded) { return LocalRedirect(ReturnUrl); }
        }

        return Page();
    }

    [BindProperty]
    public InputMode Input { get; set;} = new();
    public string ReturnUrl { get; set; } = string.Empty;


      public void OnGet()
      {
        ReturnUrl = Url.Content("~/");
      }

      public class InputMode {
        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
      }
  }
