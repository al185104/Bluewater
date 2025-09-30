using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class PositionListResponseDto
{
  public List<PositionDto?> Positions { get; set; } = new();
}

public class PositionDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public Guid SectionId { get; set; }
}

public class CreatePositionRequestDto
{
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Guid SectionId { get; set; }
}

public class UpdatePositionRequestDto
{
  public Guid PositionId { get; set; }
  public Guid Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Guid SectionId { get; set; }

  public static string BuildRoute(Guid positionId) => $"Positions/{positionId}";
}

public class UpdatePositionResponseDto
{
  public PositionDto? Position { get; set; }
}
