using Ardalis.Result;
using Ardalis.SharedKernel;

namespace Bluewater.UseCases.Sections.Update;
public record UpdateSectionCommand(Guid SectionId, string NewName, string? Description, string? approved1id, string? approved2id, string? approved3id, Guid divisionId) : ICommand<Result<SectionDTO>>;
