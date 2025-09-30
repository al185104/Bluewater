using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class DivisionListResponseDto
{
  public List<DivisionDto?> Divisions { get; set; } = new();
}

public class DivisionDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
}

public class CreateDivisionRequestDto
{
  public string? Name { get; set; }
  public string? Description { get; set; }
}

public class UpdateDivisionRequestDto
{
  public Guid DivisionId { get; set; }
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }

  public static string BuildRoute(Guid divisionId) => $"Divisions/{divisionId}";
}

public class UpdateDivisionResponseDto
{
  public DivisionDto? Division { get; set; }
}
