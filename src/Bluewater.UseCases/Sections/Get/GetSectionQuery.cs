using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Sections.Get;
public record class GetSectionQuery(Guid SectionId) : IQuery<Result<SectionDTO>>;
