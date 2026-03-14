using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Specifications;
using Bluewater.UseCases.Employees.Create;

namespace Bluewater.UseCases.Contributors.Create;
public class CreateEmployeeHandler(IRepository<Employee> repository) : ICommandHandler<CreateEmployeeCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
  {
    if (request.UserId.HasValue && request.UserId.Value != Guid.Empty)
    {
      Employee? existingByUserId = await repository.FirstOrDefaultAsync(new EmployeeByUserIdSpec(request.UserId.Value), cancellationToken);
      if (existingByUserId != null)
      {
        if (existingByUserId.IsDeleted)
        {
          ApplyEmployeeData(existingByUserId, request);
          existingByUserId.Restore();
          await repository.UpdateAsync(existingByUserId, cancellationToken);
        }

        return existingByUserId.Id;
      }
    }

    Employee? deletedEmployee = await repository.FirstOrDefaultAsync(
      new DeletedEmployeeByNameSpec(request.FirstName, request.LastName, request.Tenant),
      cancellationToken);

    if (deletedEmployee != null)
    {
      ApplyEmployeeData(deletedEmployee, request);
      deletedEmployee.Restore();
      await repository.UpdateAsync(deletedEmployee, cancellationToken);
      return deletedEmployee.Id;
    }

    var newEmployee = new Employee(
      request.FirstName,
      request.LastName,
      request.MiddleName,
      request.DateOfBirth,
      request.Gender,
      request.CivilStatus,
      request.BloodType,
      request.Status,
      request.Height,
      request.Weight,
      request.ImageUrl,
      request.Remarks,
      request.MealCredits,
      request.Tenant
    );

    ApplyEmployeeData(newEmployee, request);

    Employee createdItem = await repository.AddAsync(newEmployee, cancellationToken);
    return createdItem.Id;
  }

  private static void ApplyEmployeeData(Employee employee, CreateEmployeeCommand request)
  {
    employee.UpdateEmployee(
      request.FirstName,
      request.LastName,
      request.MiddleName,
      request.DateOfBirth,
      request.Gender,
      request.CivilStatus,
      request.BloodType,
      request.Status,
      request.Height,
      request.Weight,
      request.ImageUrl,
      request.Remarks,
      request.MealCredits,
      request.Tenant);

    if (request.ContactInfo != null)
    {
      employee.SetContactInfo(new Core.EmployeeAggregate.ContactInfo(
        request.ContactInfo.Email,
        request.ContactInfo.TelNumber,
        request.ContactInfo.MobileNumber,
        request.ContactInfo.Address,
        request.ContactInfo.ProvincialAddress,
        request.ContactInfo.MothersMaidenName,
        request.ContactInfo.FathersName,
        request.ContactInfo.EmergencyContact,
        request.ContactInfo.RelationshipContact,
        request.ContactInfo.addressContact,
        request.ContactInfo.TelNoContact,
        request.ContactInfo.MobileNoContact
      ));
    }

    if (request.EducationInfo != null)
    {
      employee.SetEducationInfo(new Core.EmployeeAggregate.EducationInfo(
        request.EducationInfo.EducationalAttainment,
        request.EducationInfo.CourseGraduated,
        request.EducationInfo.UniversityGraduated
      ));
    }

    if (request.EmploymentInfo != null)
    {
      employee.SetEmploymentInfo(new Core.EmployeeAggregate.EmploymentInfo(
        request.EmploymentInfo.DateHired,
        request.EmploymentInfo.DateRegularized,
        request.EmploymentInfo.DateResigned,
        request.EmploymentInfo.DateTerminated,
        request.EmploymentInfo.TinNo,
        request.EmploymentInfo.SssNo,
        request.EmploymentInfo.HdmfNo,
        request.EmploymentInfo.PhicNo,
        request.EmploymentInfo.BankAccount,
        request.EmploymentInfo.HasServiceCharge
      ));
    }

    employee.SetExternalKeys(request.UserId, request.PositionId, request.PayId, request.TypeId, request.LevelId, request.ChargingId);
  }
}
