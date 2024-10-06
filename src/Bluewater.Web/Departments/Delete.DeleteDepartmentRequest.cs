namespace Bluewater.Web.Departments;

public class DeleteDepartmentRequest
{
  public const string Route = "/Departments/{DepartmentId:guid}";
  public static string BuildRoute(Guid DepartmentId) => Route.Replace("{DepartmentId:guid}", DepartmentId.ToString());

  public Guid DepartmentId { get; set; }
}
