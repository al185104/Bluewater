using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class ChargingListResponseDto
{
  public List<ChargingDto?> Chargings { get; set; } = new();
}

public class ChargingDto
{
  public Guid Id { get; set; }
  public string Name { get; set; } = string.Empty;
  public string? Description { get; set; }
  public Guid? DepartmentId { get; set; }
}

public class CreateChargingRequestDto
{
  public const string Route = "Chargings";

  public string? Name { get; set; }
  public string? Description { get; set; }
  public Guid? DepartmentId { get; set; }
}

public class UpdateChargingRequestDto
{
  public const string Route = "Chargings";

  public Guid Id { get; set; }
  public string? Name { get; set; }
  public string? Description { get; set; }
  public Guid? DepartmentId { get; set; }
}

public class UpdateChargingResponseDto
{
  public ChargingDto? Charging { get; set; }
}
