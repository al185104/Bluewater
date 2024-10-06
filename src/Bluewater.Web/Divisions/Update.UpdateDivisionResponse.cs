namespace Bluewater.Web.Divisions;

public class UpdateDivisionResponse(DivisionRecord division)
{
  public DivisionRecord Division { get; set; } = division;
}
