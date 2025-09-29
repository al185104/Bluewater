using System.Linq;
using System.Threading.Tasks;
using Bluewater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Bluewater.IntegrationTests.Data;

public class EmployeeDataSeederTests : BaseEfRepoTestFixture
{
  [Fact]
  public async Task SeedAsync_PopulatesEmployeesAndRelatedData()
  {
    await EmployeeDataSeeder.SeedAsync(_dbContext);

    var employees = await _dbContext.Employees.ToListAsync();

    Assert.NotEmpty(employees);

    var employee = employees.First();
    Assert.NotNull(employee.ContactInfo);
    Assert.NotNull(employee.EducationInfo);
    Assert.NotNull(employee.EmploymentInfo);
    Assert.True(employee.UserId.HasValue);
    Assert.True(employee.PositionId.HasValue);
    Assert.True(employee.PayId.HasValue);
    Assert.True(employee.TypeId.HasValue);
    Assert.True(employee.LevelId.HasValue);
    Assert.True(employee.ChargingId.HasValue);

    var dependents = await _dbContext.Dependents.Where(d => d.EmployeeId == employee.Id).ToListAsync();
    Assert.NotEmpty(dependents);

    // Subsequent calls should be no-ops
    await EmployeeDataSeeder.SeedAsync(_dbContext);
    var employeeCount = await _dbContext.Employees.CountAsync();
    Assert.Equal(employees.Count, employeeCount);
  }
}
