using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class LeaveApiService(IApiClient apiClient) : ILeaveApiService
{
  public async Task<IReadOnlyList<LeaveSummary>> GetLeavesAsync(
    int? skip = null,
    int? take = null,
    TenantDto tenant = TenantDto.Maribago,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take, tenant);

    LeaveListResponseDto? response = await apiClient.GetAsync<LeaveListResponseDto>(requestUri, cancellationToken);

    if (response?.Leaves is not { Count: > 0 })
    {
      return Array.Empty<LeaveSummary>();
    }

    return response.Leaves
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<LeaveSummary?> GetLeaveByIdAsync(Guid leaveId, CancellationToken cancellationToken = default)
  {
    if (leaveId == Guid.Empty)
    {
      throw new ArgumentException("Leave ID must be provided", nameof(leaveId));
    }

    LeaveDto? dto = await apiClient.GetAsync<LeaveDto>($"Leaves/{leaveId}", cancellationToken);
    return dto is null ? null : MapToSummary(dto);
  }

  public async Task<LeaveSummary?> CreateLeaveAsync(LeaveSummary leave, CancellationToken cancellationToken = default)
  {
    if (leave is null)
    {
      throw new ArgumentNullException(nameof(leave));
    }

    if (!leave.EmployeeId.HasValue || leave.EmployeeId.Value == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(leave));
    }

    if (leave.LeaveCreditId == Guid.Empty)
    {
      throw new ArgumentException("Leave credit ID must be provided", nameof(leave));
    }

    CreateLeaveRequestDto request = new()
    {
      StartDate = leave.StartDate,
      EndDate = leave.EndDate,
      IsHalfDay = leave.IsHalfDay,
      EmployeeId = leave.EmployeeId.Value,
      LeaveCreditId = leave.LeaveCreditId
    };

    CreateLeaveResponseDto? response = await apiClient.PostAsync<CreateLeaveRequestDto, CreateLeaveResponseDto>(
      CreateLeaveRequestDto.Route,
      request,
      cancellationToken);

    return response?.Leave is null ? null : MapToSummary(response.Leave);
  }

  public async Task<LeaveSummary?> UpdateLeaveAsync(LeaveSummary leave, CancellationToken cancellationToken = default)
  {
    if (leave is null)
    {
      throw new ArgumentNullException(nameof(leave));
    }

    if (leave.Id == Guid.Empty)
    {
      throw new ArgumentException("Leave ID must be provided", nameof(leave));
    }

    if (!leave.EmployeeId.HasValue || leave.EmployeeId.Value == Guid.Empty)
    {
      throw new ArgumentException("Employee ID must be provided", nameof(leave));
    }

    if (leave.StartDate is null)
    {
      throw new ArgumentException("Start date must be provided", nameof(leave));
    }

    if (leave.EndDate is null)
    {
      throw new ArgumentException("End date must be provided", nameof(leave));
    }

    UpdateLeaveRequestDto request = new()
    {
      LeaveId = leave.Id,
      Id = leave.Id,
      StartDate = leave.StartDate.Value,
      EndDate = leave.EndDate.Value,
      IsHalfDay = leave.IsHalfDay,
      Status = leave.Status,
      EmployeeId = leave.EmployeeId.Value,
      LeaveCreditId = leave.LeaveCreditId
    };

    UpdateLeaveResponseDto? response = await apiClient.PutAsync<UpdateLeaveRequestDto, UpdateLeaveResponseDto>(
      UpdateLeaveRequestDto.BuildRoute(leave.Id),
      request,
      cancellationToken);

    return response?.Leave is null ? null : MapToSummary(response.Leave);
  }

  public Task<bool> DeleteLeaveAsync(Guid leaveId, CancellationToken cancellationToken = default)
  {
    if (leaveId == Guid.Empty)
    {
      throw new ArgumentException("Leave ID must be provided", nameof(leaveId));
    }

    return apiClient.DeleteAsync($"Leaves/{leaveId}", cancellationToken);
  }

  private static string BuildRequestUri(int? skip, int? take, TenantDto tenant)
  {
    List<string> parameters = [];

    if (skip.HasValue)
    {
      parameters.Add($"skip={skip.Value}");
    }

    if (take.HasValue)
    {
      parameters.Add($"take={take.Value}");
    }

    parameters.Add($"tenant={tenant}");

    if (parameters.Count == 0)
    {
      return "Leaves";
    }

    string query = string.Join('&', parameters);
    return $"Leaves?{query}";
  }

  private static LeaveSummary MapToSummary(LeaveDto dto)
  {
    return new LeaveSummary
    {
      Id = dto.Id,
      StartDate = dto.StartDate,
      EndDate = dto.EndDate,
      IsHalfDay = dto.IsHalfDay,
      Status = dto.Status,
      EmployeeId = dto.EmployeeId,
      LeaveCreditId = dto.LeaveCreditId,
      EmployeeName = dto.EmployeeName,
      LeaveCreditName = dto.LeaveCreditName
    };
  }
}
