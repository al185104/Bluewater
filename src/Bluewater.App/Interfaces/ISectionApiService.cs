using System;
using System.Collections.Generic;
using Bluewater.App.Models;

namespace Bluewater.App.Interfaces;

public interface ISectionApiService
{
  Task<IReadOnlyList<SectionSummary>> GetSectionsAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default);

  Task<SectionSummary?> GetSectionByIdAsync(Guid sectionId, CancellationToken cancellationToken = default);

  Task<SectionSummary?> CreateSectionAsync(SectionSummary section, CancellationToken cancellationToken = default);

  Task<SectionSummary?> UpdateSectionAsync(SectionSummary section, CancellationToken cancellationToken = default);

  Task<bool> DeleteSectionAsync(Guid sectionId, CancellationToken cancellationToken = default);
}
