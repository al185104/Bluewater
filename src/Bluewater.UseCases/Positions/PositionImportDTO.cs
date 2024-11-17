namespace Bluewater.UseCases.Positions;
public record PositionImportDTO()  :  PositionDTO
{
  public string Section { get; set; } = null!;
}
