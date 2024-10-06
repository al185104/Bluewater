using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Sections.List;
public record ListSectionsQuery (int? skip, int? take) : IQuery<Result<IEnumerable<SectionDTO>>>;
