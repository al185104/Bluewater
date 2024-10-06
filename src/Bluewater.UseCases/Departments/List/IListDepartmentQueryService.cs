namespace Bluewater.UseCases.Departments.List;
public interface IListDepartmentsQueryService
{
  Task<IEnumerable<DepartmentDTO>> ListAsync();
}
