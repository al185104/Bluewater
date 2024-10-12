using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;
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

namespace Bluewater.UseCases.Employees.Update;
public class UpdateEmployeeHandler(IRepository<Employee> _repository, IServiceScopeFactory _serviceScopeFactory) : ICommandHandler<UpdateEmployeeCommand, Result<EmployeeDTO>>
{
  public async Task<Result<EmployeeDTO>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
  {
    var existingEmployee = await _repository.GetByIdAsync(request.Id, cancellationToken);
    if (existingEmployee == null) return Result.NotFound();

    existingEmployee.UpdateEmployee(request.FirstName, request.LastName, request.MiddleName, request.DateOfBirth, request.Gender, request.CivilStatus, request.BloodType, request.Status, request.Height, request.Weight, request.ImageUrl, request.Remarks);

    if(request.ContactInfo != null)
    {
        existingEmployee.SetContactInfo(new Core.EmployeeAggregate.ContactInfo(
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

    if(request.EducationInfo != null)
    {
        existingEmployee.SetEducationInfo(new Core.EmployeeAggregate.EducationInfo(
            request.EducationInfo.PrimarySchool,
            request.EducationInfo.SecondarySchool,
            request.EducationInfo.TertiarySchool,
            request.EducationInfo.VocationalSchool,
            request.EducationInfo.PrimaryDegree,
            request.EducationInfo.SecondaryDegree,
            request.EducationInfo.TertiaryDegree,
            request.EducationInfo.VocationalDegree
        ));
    }

    if(request.EmploymentInfo != null)
    {
        existingEmployee.SetEmploymentInfo(new Core.EmployeeAggregate.EmploymentInfo(
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

    if(request.PositionId != Guid.Empty && request.PayId != Guid.Empty && request.TypeId != Guid.Empty && request.LevelId != Guid.Empty && request.ChargingId != Guid.Empty)
        existingEmployee.SetExternalKeys(request.PositionId, request.PayId, request.TypeId, request.LevelId, request.ChargingId);

    await _repository.UpdateAsync(existingEmployee, cancellationToken);

    var contract = new ContactInfoDTO(
        existingEmployee.ContactInfo!.Email,
        existingEmployee.ContactInfo!.TelNumber,
        existingEmployee.ContactInfo!.MobileNumber,
        existingEmployee.ContactInfo!.Address,
        existingEmployee.ContactInfo!.ProvincialAddress,
        existingEmployee.ContactInfo!.MothersMaidenName,
        existingEmployee.ContactInfo!.FathersName,
        existingEmployee.ContactInfo!.EmergencyContact,
        existingEmployee.ContactInfo!.RelationshipContact,
        existingEmployee.ContactInfo!.AddressContact,
        existingEmployee.ContactInfo!.TelNoContact,
        existingEmployee.ContactInfo!.MobileNoContact
    );

    var educationInfo = new EducationInfoDTO(
        existingEmployee.EducationInfo!.PrimarySchool,
        existingEmployee.EducationInfo!.SecondarySchool,
        existingEmployee.EducationInfo!.TertiarySchool,
        existingEmployee.EducationInfo!.VocationalSchool,
        existingEmployee.EducationInfo!.PrimaryDegree,
        existingEmployee.EducationInfo!.SecondaryDegree,
        existingEmployee.EducationInfo!.TertiaryDegree,
        existingEmployee.EducationInfo!.VocationalDegree
    );

    var employeeInfo = new EmploymentInfoDTO(
        existingEmployee.EmploymentInfo!.DateHired,
        existingEmployee.EmploymentInfo!.DateRegularized,
        existingEmployee.EmploymentInfo!.DateResigned,
        existingEmployee.EmploymentInfo!.DateTerminated,
        existingEmployee.EmploymentInfo!.TINNo,
        existingEmployee.EmploymentInfo!.SSSNo,
        existingEmployee.EmploymentInfo!.HDMFNo,
        existingEmployee.EmploymentInfo!.PHICNo
    );

    var user = new UserDTO(
        existingEmployee.User!.Username, 
        existingEmployee.User.PasswordHash, 
        existingEmployee.User!.Credential.ToString(),
        existingEmployee.User!.SupervisedGroup);

    PositionDTO? position = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetPositionQuery(existingEmployee.PositionId), cancellationToken);
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
        var result = await mediator.Send(new GetChargingQuery(existingEmployee.ChargingId), cancellationToken);
        if (result.IsSuccess)
            charging = new ChargingDTO(result.Value.Id, result.Value.Name, result.Value.Description ?? string.Empty);
        else
            charging = new ChargingDTO(Guid.Empty, string.Empty, string.Empty);
    }

    var pay = new PayDTO(
        existingEmployee.Pay!.BasicPay,
        existingEmployee.Pay!.DailyRate,
        existingEmployee.Pay!.HourlyRate,
        existingEmployee.Pay!.HDMF_Con,
        existingEmployee.Pay!.HDMF_Er
    );

    EmployeeTypeDTO? type = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetEmployeeTypeQuery(existingEmployee.TypeId), cancellationToken);
        if (result.IsSuccess)
            type = new EmployeeTypeDTO(result.Value.Id, result.Value.Name, result.Value.Value ?? string.Empty, result.Value.IsActive);
        else
            type = new EmployeeTypeDTO(Guid.Empty, string.Empty, string.Empty);
    }

    LevelDTO? level = null;
    using (var scope = _serviceScopeFactory.CreateScope())
    {
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new GetLevelQuery(existingEmployee.LevelId), cancellationToken);
        if (result.IsSuccess)
            level = new LevelDTO(result.Value.Id, result.Value.Name, result.Value.Value ?? string.Empty, result.Value.IsActive);
        else
            level = new LevelDTO(Guid.Empty, string.Empty, string.Empty);
    }

    return Result.Success(
        new EmployeeDTO(
            existingEmployee.Id,
            $"{existingEmployee.LastName} {existingEmployee.FirstName}",
            existingEmployee.MiddleName,
            existingEmployee.DateOfBirth,
            existingEmployee.Gender,
            existingEmployee.CivilStatus,
            existingEmployee.BloodType,
            existingEmployee.Status,
            existingEmployee.Height,
            existingEmployee.Weight,
            existingEmployee.ImageUrl,
            existingEmployee.Remarks,
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
        )
    );
  }

}

