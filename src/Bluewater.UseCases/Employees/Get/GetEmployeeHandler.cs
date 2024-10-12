using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
using Bluewater.Core.EmployeeAggregate.Specifications;
using Bluewater.UseCases.Chargings;
using Bluewater.UseCases.Chargings.Get;
using Bluewater.UseCases.Departments;
using Bluewater.UseCases.Departments.Get;
using Bluewater.UseCases.Divisions;
using Bluewater.UseCases.Divisions.Get;
using Bluewater.UseCases.EmployeeTypes;
using Bluewater.UseCases.EmployeeTypes.Get;
using Bluewater.UseCases.Levels;
using Bluewater.UseCases.Levels.Get;
using Bluewater.UseCases.Positions;
using Bluewater.UseCases.Positions.Get;
using Bluewater.UseCases.Sections;
using Bluewater.UseCases.Sections.Get;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bluewater.UseCases.Employees.Get;

public class GetEmployeeHandler(IRepository<Employee> _repository, IServiceScopeFactory _serviceScopeFactory) : IQueryHandler<GetEmployeeQuery, Result<EmployeeDTO>>
{
  public async Task<Result<EmployeeDTO>> Handle(GetEmployeeQuery request, CancellationToken cancellationToken)
  {
    var spec = new EmployeeByIdSpec(request.EmployeeId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    var contract = new ContactInfoDTO(
        entity.ContactInfo!.Email,
        entity.ContactInfo!.TelNumber,
        entity.ContactInfo!.MobileNumber,
        entity.ContactInfo!.Address,
        entity.ContactInfo!.ProvincialAddress,
        entity.ContactInfo!.MothersMaidenName,
        entity.ContactInfo!.FathersName,
        entity.ContactInfo!.EmergencyContact,
        entity.ContactInfo!.RelationshipContact,
        entity.ContactInfo!.AddressContact,
        entity.ContactInfo!.TelNoContact,
        entity.ContactInfo!.MobileNoContact
    );

    var educationInfo = new EducationInfoDTO(
        entity.EducationInfo!.PrimarySchool,
        entity.EducationInfo!.SecondarySchool,
        entity.EducationInfo!.TertiarySchool,
        entity.EducationInfo!.VocationalSchool,
        entity.EducationInfo!.PrimaryDegree,
        entity.EducationInfo!.SecondaryDegree,
        entity.EducationInfo!.TertiaryDegree,
        entity.EducationInfo!.VocationalDegree
    );

    var employeeInfo = new EmploymentInfoDTO(
        entity.EmploymentInfo!.DateHired,
        entity.EmploymentInfo!.DateRegularized,
        entity.EmploymentInfo!.DateResigned,
        entity.EmploymentInfo!.DateTerminated,
        entity.EmploymentInfo!.TINNo,
        entity.EmploymentInfo!.SSSNo,
        entity.EmploymentInfo!.HDMFNo,
        entity.EmploymentInfo!.PHICNo
    );

    var user = new UserDTO(
        entity.User!.Username, 
        entity.User.PasswordHash, 
        entity.User!.Credential.ToString(),
        entity.User!.SupervisedGroup);

    PositionDTO? position = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetPositionQuery(entity.PositionId), cancellationToken);
        if (result.IsSuccess)
            position = new PositionDTO(result.Value.Id, result.Value.Name, result.Value.Description ?? string.Empty, result.Value.SectionId);
        else
            position = new PositionDTO(Guid.Empty, string.Empty, string.Empty, Guid.Empty);
    }

    SectionDTO? section = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetSectionQuery(position!.SectionId), cancellationToken);
        if (result.IsSuccess)
            section = new SectionDTO(result.Value.Id, result.Value.Name, result.Value.Description ?? string.Empty, result.Value.Approved1Id, result.Value.Approved2Id, result.Value.Approved3Id, result.Value.DepartmentId);
        else
            section = new SectionDTO(Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, Guid.Empty);
    }

    DepartmentDTO? department = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetDepartmentQuery(section!.DepartmentId), cancellationToken);
        if (result.IsSuccess)
            department = new DepartmentDTO(result.Value.Id, result.Value.Name, result.Value.Description ?? string.Empty, result.Value.DivisionId);
        else    
            department = new DepartmentDTO(Guid.Empty, string.Empty, string.Empty, Guid.Empty);
    }

    DivisionDTO? division = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetDivisionQuery(department!.DivisionId), cancellationToken);
        if (result.IsSuccess)
            division = new DivisionDTO(result.Value.Id, result.Value.Name, result.Value.Description ?? string.Empty);
        else
            division = new DivisionDTO(Guid.Empty, string.Empty, string.Empty);
    }

    ChargingDTO? charging = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetChargingQuery(entity.ChargingId), cancellationToken);
        if (result.IsSuccess)
            charging = new ChargingDTO(result.Value.Id, result.Value.Name, result.Value.Description ?? string.Empty);
        else
            charging = new ChargingDTO(Guid.Empty, string.Empty, string.Empty);
    }

    var pay = new PayDTO(
        entity.Pay!.BasicPay,
        entity.Pay!.DailyRate,
        entity.Pay!.HourlyRate,
        entity.Pay!.HDMF_Con,
        entity.Pay!.HDMF_Er
    );

    EmployeeTypeDTO? type = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetEmployeeTypeQuery(entity.TypeId), cancellationToken);
        if (result.IsSuccess)
            type = new EmployeeTypeDTO(result.Value.Id, result.Value.Name, result.Value.Value ?? string.Empty, result.Value.IsActive);
        else
            type = new EmployeeTypeDTO(Guid.Empty, string.Empty, string.Empty);
    }

    LevelDTO? level = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetLevelQuery(entity.LevelId), cancellationToken);
        if (result.IsSuccess)
            level = new LevelDTO(result.Value.Id, result.Value.Name, result.Value.Value ?? string.Empty, result.Value.IsActive);
        else
            level = new LevelDTO(Guid.Empty, string.Empty, string.Empty);
    }

    return new EmployeeDTO(
        entity.Id,
        $"{entity.LastName} {entity.FirstName}",
        entity.MiddleName,
        entity.DateOfBirth,
        entity.Gender,
        entity.CivilStatus,
        entity.BloodType,
        entity.Status,
        entity.Height,
        entity.Weight,
        entity.ImageUrl,
        entity.Remarks,
        contract,
        educationInfo,
        employeeInfo,
        user,
        position?.Name,
        section?.Name,
        department?.Name,
        division?.Name,
        charging?.Name,
        pay,
        type?.Name,
        level?.Name
    );
  }
}
