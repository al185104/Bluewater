using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class LevelListResponseDto
{
  public List<LevelDto?> Levels { get; set; } = new();
}

public class LevelDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public bool IsActive { get; set; }
}

public class CreateLevelRequestDto
{
  public string Name { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public bool IsActive { get; set; } = true;
}

public class UpdateLevelRequestDto
{
  public Guid LevelId { get; set; }
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string Value { get; set; } = string.Empty;
  public bool IsActive { get; set; }

  public static string BuildRoute(Guid levelId) => $"Levels/{levelId}";
}

public class UpdateLevelResponseDto
{
  public LevelDto Level { get; set; } = new();
}
