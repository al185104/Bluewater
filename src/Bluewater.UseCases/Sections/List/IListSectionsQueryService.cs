namespace Bluewater.UseCases.Sections.List;
public interface IListSectionsQueryService
{
  Task<IEnumerable<SectionDTO>> ListAsync();
}
