namespace Bluewater.UseCases.Divisions;
public record DivisionDTO()
{
    public Guid Id { get; init; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public DivisionDTO(Guid id, string name, string? description) : this()
    {
        Id = id;
        Name = name;
        Description = description;
    }
}


