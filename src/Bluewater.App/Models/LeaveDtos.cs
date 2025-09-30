using System;
using System.Collections.Generic;

namespace Bluewater.App.Models;

public class LeaveListResponseDto
{
  public List<LeaveDto?> Leaves { get; set; } = new();
}

public class LeaveDto
{
  public Guid Id { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public bool IsHalfDay { get; set; }
  public ApplicationStatusDto Status { get; set; } = ApplicationStatusDto.NotSet;
  public Guid? EmployeeId { get; set; }
  public Guid LeaveCreditId { get; set; }
  public string EmployeeName { get; set; } = string.Empty;
  public string LeaveCreditName { get; set; } = string.Empty;
}

public class CreateLeaveRequestDto
{
  public const string Route = "Leaves";

  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public bool IsHalfDay { get; set; }
  public Guid EmployeeId { get; set; }
  public Guid LeaveCreditId { get; set; }
}

public class CreateLeaveResponseDto
{
  public LeaveDto? Leave { get; set; }
}

public class UpdateLeaveRequestDto
{
  public Guid LeaveId { get; set; }
  public Guid Id { get; set; }
  public DateTime StartDate { get; set; }
  public DateTime EndDate { get; set; }
  public bool IsHalfDay { get; set; }
  public ApplicationStatusDto Status { get; set; } = ApplicationStatusDto.NotSet;
  public Guid EmployeeId { get; set; }
  public Guid LeaveCreditId { get; set; }

  public static string BuildRoute(Guid leaveId) => $"Leaves/{leaveId}";
}

public class UpdateLeaveResponseDto
{
  public LeaveDto? Leave { get; set; }
}

public class LeaveSummary
{
  public Guid Id { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public bool IsHalfDay { get; set; }
  public ApplicationStatusDto Status { get; set; } = ApplicationStatusDto.NotSet;
  public Guid? EmployeeId { get; set; }
  public Guid LeaveCreditId { get; set; }
  public string EmployeeName { get; set; } = string.Empty;
  public string LeaveCreditName { get; set; } = string.Empty;
}
