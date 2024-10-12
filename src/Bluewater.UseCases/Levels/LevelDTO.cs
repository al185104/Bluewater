namespace Bluewater.UseCases.Levels;
public record LevelDTO()
{
    public Guid Id { get; init; }
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public bool IsActive { get; set; } = true;

    public LevelDTO(Guid id, string name, string value, bool isActive = true) : this()
    {
        Id = id;
        Name = name;
        Value  = value;
        IsActive = isActive;
    }
}