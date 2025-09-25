namespace Bluewater.Web.Sections;

public record SectionRecord(
  Guid Id,
  string Name,
  string? Description,
  string? Approved1Id,
  string? Approved2Id,
  string? Approved3Id,
  Guid DepartmentId
);
