using System.ComponentModel.DataAnnotations;

namespace Bluewater.Web.Pays;

public class PayListRequest
{
  [Range(0, int.MaxValue)]
  public int? Skip { get; set; }

  [Range(1, int.MaxValue)]
  public int? Take { get; set; }
}
