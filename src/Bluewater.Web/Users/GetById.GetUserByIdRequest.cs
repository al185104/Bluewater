using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Users;

public class GetUserByIdRequest
{
  public const string Route = "/Users/{UserId:guid}";

  [Required]
  public Guid UserId { get; set; }
}
