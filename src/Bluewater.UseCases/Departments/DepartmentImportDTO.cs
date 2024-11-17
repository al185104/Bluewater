namespace Bluewater.UseCases.Departments;
public record DepartmentImportDTO() : DepartmentDTO
{
  public string Division { get; set; } = null!;
}
