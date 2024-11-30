namespace Bluewater.UseCases.Employees;

public record EmployeeShortDTO(Guid Id, string Barcode, string Name, string Department, string Section, string Charging);