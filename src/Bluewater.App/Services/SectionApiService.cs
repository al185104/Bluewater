using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class SectionApiService(IApiClient apiClient) : ISectionApiService
{
  public async Task<IReadOnlyList<SectionSummary>> GetSectionsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    SectionListResponseDto? response = await apiClient
      .GetAsync<SectionListResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Sections is not { Count: > 0 })
    {
      return Array.Empty<SectionSummary>();
    }

    return response.Sections
      .Where(dto => dto is not null)
      .Select(dto => MapToSummary(dto!))
      .ToList();
  }

  public async Task<SectionSummary?> GetSectionByIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
  {
    if (sectionId == Guid.Empty)
    {
      throw new ArgumentException("Section ID must be provided", nameof(sectionId));
    }

    SectionDto? dto = await apiClient
      .GetAsync<SectionDto>($"Sections/{sectionId}", cancellationToken)
      .ConfigureAwait(false);
    return dto is null ? null : MapToSummary(dto);
  }

  public async Task<SectionSummary?> CreateSectionAsync(SectionSummary section, CancellationToken cancellationToken = default)
  {
    if (section is null)
    {
      throw new ArgumentNullException(nameof(section));
    }

    CreateSectionRequestDto request = new()
    {
      Name = section.Name,
      Description = section.Description,
      Approved1Id = section.Approved1Id,
      Approved2Id = section.Approved2Id,
      Approved3Id = section.Approved3Id,
      DepartmentId = section.DepartmentId
    };

    SectionDto? response = await apiClient
      .PostAsync<CreateSectionRequestDto, SectionDto>("Sections", request, cancellationToken)
      .ConfigureAwait(false);
    return response is null ? null : MapToSummary(response);
  }

  public async Task<SectionSummary?> UpdateSectionAsync(SectionSummary section, CancellationToken cancellationToken = default)
  {
    if (section is null)
    {
      throw new ArgumentNullException(nameof(section));
    }

    if (section.Id == Guid.Empty)
    {
      throw new ArgumentException("Section ID must be provided", nameof(section));
    }

    UpdateSectionRequestDto request = new()
    {
      SectionId = section.Id,
      Id = section.Id,
      Name = section.Name,
      Description = section.Description,
      Approved1Id = section.Approved1Id,
      Approved2Id = section.Approved2Id,
      Approved3Id = section.Approved3Id,
      DepartmentId = section.DepartmentId
    };

    UpdateSectionResponseDto? response = await apiClient
      .PutAsync<UpdateSectionRequestDto, UpdateSectionResponseDto>(
      UpdateSectionRequestDto.BuildRoute(section.Id),
      request,
      cancellationToken)
      .ConfigureAwait(false);

    return response?.Section is null ? null : MapToSummary(response.Section);
  }

  public Task<bool> DeleteSectionAsync(Guid sectionId, CancellationToken cancellationToken = default)
  {
    if (sectionId == Guid.Empty)
    {
      throw new ArgumentException("Section ID must be provided", nameof(sectionId));
    }

    return apiClient.DeleteAsync($"Sections/{sectionId}", cancellationToken);
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
      return "Sections";
    }

    string query = string.Join('&', parameters);
    return $"Sections?{query}";
  }

  private static SectionSummary MapToSummary(SectionDto dto)
  {
    return new SectionSummary
    {
      Id = dto.Id,
      Name = dto.Name,
      Description = dto.Description,
      Approved1Id = dto.Approved1Id,
      Approved2Id = dto.Approved2Id,
      Approved3Id = dto.Approved3Id,
      DepartmentId = dto.DepartmentId
    };
  }
}
