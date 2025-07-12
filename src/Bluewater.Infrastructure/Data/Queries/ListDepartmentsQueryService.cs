using Bluewater.UseCases.Departments;
using Bluewater.UseCases.Departments.List;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Infrastructure.Data.Queries;
public class ListDepartmentsQueryService(AppDbContext _db) : IListDepartmentsQueryService
{
  public async Task<IEnumerable<DepartmentDTO>> ListAsync()
  {
    try
    {
      var ret = await _db.Departments.Select(d => new DepartmentDTO(d.Id, d.Name, d.Description ?? string.Empty, d.DivisionId))
        .ToListAsync();

      return ret.OrderBy(i => i.Name);
    }
    catch (DbUpdateConcurrencyException)
    {
      throw;
    }
  }
}
