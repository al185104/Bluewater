namespace Bluewater.UseCases.EmployeeTypes;

public record EmployeeTypeDTO()
{
    public Guid Id { get; init; }
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public EmployeeTypeDTO(Guid id, string name, string value, bool isActive = true) : this()
    {
        Id = id;
        Name = name;
        Value  = value;
        IsActive = isActive;
    }
}