using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class DepartmentApiService(IApiClient apiClient) : IDepartmentApiService
{
  public async Task<IReadOnlyList<DepartmentSummary>> GetDepartmentsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    DepartmentListResponseDto? response = await apiClient
      .GetAsync<DepartmentListResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Departments is not { Count: > 0 })
    {
      return Array.Empty<DepartmentSummary>();
    }

    return response.Departments
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<DepartmentSummary?> GetDepartmentByIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
  {
    if (departmentId == Guid.Empty)
    {
      throw new ArgumentException("Department ID must be provided", nameof(departmentId));
    }

    DepartmentDto? dto = await apiClient
      .GetAsync<DepartmentDto>($"Departments/{departmentId}", cancellationToken)
      .ConfigureAwait(false);
    return dto is null ? null : MapToSummary(dto);
  }

  public async Task<DepartmentSummary?> CreateDepartmentAsync(DepartmentSummary department, CancellationToken cancellationToken = default)
  {
    if (department is null)
    {
      throw new ArgumentNullException(nameof(department));
    }

    CreateDepartmentRequestDto request = new()
    {
      Name = department.Name,
      Description = department.Description,
      DivisionId = department.DivisionId
    };

    DepartmentDto? response = await apiClient
      .PostAsync<CreateDepartmentRequestDto, DepartmentDto>("Departments", request, cancellationToken)
      .ConfigureAwait(false);

    return response is null ? null : MapToSummary(response);
  }

  public async Task<DepartmentSummary?> UpdateDepartmentAsync(DepartmentSummary department, CancellationToken cancellationToken = default)
  {
    if (department is null)
    {
      throw new ArgumentNullException(nameof(department));
    }

    if (department.Id == Guid.Empty)
    {
      throw new ArgumentException("Department ID must be provided", nameof(department));
    }

    UpdateDepartmentRequestDto request = new()
    {
      DepartmentId = department.Id,
      Id = department.Id,
      Name = department.Name,
      Description = department.Description,
      DivisionId = department.DivisionId
    };

    UpdateDepartmentResponseDto? response = await apiClient
      .PutAsync<UpdateDepartmentRequestDto, UpdateDepartmentResponseDto>(
      UpdateDepartmentRequestDto.BuildRoute(department.Id),
      request,
      cancellationToken)
      .ConfigureAwait(false);

    return response?.Department is null ? null : MapToSummary(response.Department);
  }

  public Task<bool> DeleteDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
  {
    if (departmentId == Guid.Empty)
    {
      throw new ArgumentException("Department ID must be provided", nameof(departmentId));
    }

    return apiClient.DeleteAsync($"Departments/{departmentId}", cancellationToken);
  }

  private static string BuildRequestUri(int? skip, int? take)
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

    if (parameters.Count == 0)
    {
      return "Departments";
    }

    string query = string.Join('&', parameters);
    return $"Departments?{query}";
  }

  private static DepartmentSummary MapToSummary(DepartmentDto? dto)
  {
    if(dto is null)
    {
      throw new ArgumentNullException(nameof(dto));
    }

    return new DepartmentSummary
    {
      Id = dto.Id,
      Name = dto.Name,
      Description = dto.Description,
      DivisionId = dto.DivisionId
    };
  }
}
