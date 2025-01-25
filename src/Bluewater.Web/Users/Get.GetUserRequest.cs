using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Users;

public class GetUserRequest
{
  public const string Route = "/Users/{barcode}";

  [Required]
  public string barcode { get; set; } = null!;
}
