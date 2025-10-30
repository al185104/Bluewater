using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.App.Services;

public class EmployeeApiService(IApiClient apiClient) : IEmployeeApiService
{
  public async Task<IReadOnlyList<EmployeeSummary>> GetEmployeesAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    EmployeeListResponseDto? response = await apiClient.GetAsync<EmployeeListResponseDto>(requestUri, cancellationToken);

    if (response?.Employees is not { Count: > 0 })
    {
      return Array.Empty<EmployeeSummary>();
    }

    return response.Employees
      .Where(dto => dto is not null)
      .Select(MapToSummary)
      .ToList();
  }

  public async Task<EmployeeSummary?> UpdateEmployeeAsync(
    UpdateEmployeeRequestDto request,
    EmployeeSummary existingSummary,
    CancellationToken cancellationToken = default)
  {
    Guid? userId = request.UserId ?? existingSummary.UserId;

    if (!userId.HasValue && request.User is not null && !string.IsNullOrWhiteSpace(request.User.Username))
    {
      userId = await CreateUserAsync(request.User, cancellationToken).ConfigureAwait(false);
    }

    Guid? payId = request.PayId ?? existingSummary.PayId;

    if (!payId.HasValue && request.Pay is not null)
    {
      payId = await CreatePayAsync(request.Pay, cancellationToken).ConfigureAwait(false);
    }

    Guid? positionId = request.PositionId ?? existingSummary.PositionId;
    Guid? typeId = request.TypeId ?? existingSummary.TypeId;
    Guid? levelId = request.LevelId ?? existingSummary.LevelId;
    Guid? chargingId = request.ChargingId ?? existingSummary.ChargingId;

    if (userId is null || payId is null || positionId is null || typeId is null || levelId is null || chargingId is null)
    {
      return null;
    }

    request.UserId = userId;
    request.PayId = payId;
    request.PositionId = positionId;
    request.TypeId = typeId;
    request.LevelId = levelId;
    request.ChargingId = chargingId;
    request.User = null;
    request.Pay = null;

    UpdateEmployeeResponseDto? response = await apiClient
      .PutAsync<UpdateEmployeeRequestDto, UpdateEmployeeResponseDto>(UpdateEmployeeRequestDto.Route, request, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Employee is null)
    {
      return null;
    }

    return MapUpdatedEmployee(response.Employee, request, existingSummary);
  }

  private async Task<Guid?> CreateUserAsync(UpdateEmployeeUserDto user, CancellationToken cancellationToken)
  {
    CreateUserRequestDto request = new()
    {
      Username = user.Username,
      PasswordHash = user.PasswordHash,
      Credential = user.Credential,
      SupervisedGroup = user.SupervisedGroup,
      IsGlobalSupervisor = user.IsGlobalSupervisor
    };

    CreateUserResponseDto? response = await apiClient
      .PostAsync<CreateUserRequestDto, CreateUserResponseDto>(CreateUserRequestDto.Route, request, cancellationToken)
      .ConfigureAwait(false);

    return response?.Id;
  }

  private async Task<Guid?> CreatePayAsync(UpdateEmployeePayDto pay, CancellationToken cancellationToken)
  {
    CreatePayRequestDto request = new()
    {
      BasicPay = pay.BasicPay,
      DailyRate = pay.DailyRate,
      HourlyRate = pay.HourlyRate,
      HdmfCon = pay.HdmfCon,
      HdmfEr = pay.HdmfEr
    };

    CreatePayResponseDto? response = await apiClient
      .PostAsync<CreatePayRequestDto, CreatePayResponseDto>(CreatePayRequestDto.Route, request, cancellationToken)
      .ConfigureAwait(false);

    return response?.Pay?.Id;
  }

  private static string BuildRequestUri(int? skip, int? take)
  {
    List<string> parameters = [];

    if (skip.HasValue)
    {
      parameters.Add($"skip={skip.Value}");
    }

    if (take.HasValue)
    {
      parameters.Add($"take={take.Value}");
    }

    if (parameters.Count == 0)
    {
      return "Employees";
    }

    string query = string.Join('&', parameters);
    return $"Employees?{query}";
  }

  private static EmployeeSummary MapToSummary(EmployeeDto dto)
  {
    return new EmployeeSummary
    {
      Id = dto.Id,
      FirstName = dto.FirstName,
      LastName = dto.LastName,
      MiddleName = dto.MiddleName,
      DateOfBirth = dto.DateOfBirth,
      Gender = dto.Gender,
      CivilStatus = dto.CivilStatus,
      BloodType = dto.BloodType,
      Status = dto.Status,
      Height = dto.Height,
      Weight = dto.Weight,
      Remarks = dto.Remarks,
      MealCredits = dto.MealCredits,
      Tenant = dto.Tenant,
      Department = dto.Department,
      Section = dto.Section,
      Position = dto.Position,
      Type = dto.Type,
      Level = dto.Level,
      Image = dto.Image,
      IsDeleted = dto.IsDeleted,
      UserId = dto.UserId,
      PositionId = dto.PositionId,
      PayId = dto.PayId,
      TypeId = dto.TypeId,
      LevelId = dto.LevelId,
      ChargingId = dto.ChargingId,
      CreatedDate = dto.CreatedDate,
      CreateBy = dto.CreateBy,
      UpdatedDate = dto.UpdatedDate,
      UpdateBy = dto.UpdateBy,
      ContactInfo = dto.ContactInfo is null
        ? new ContactInfoSummary()
        : new ContactInfoSummary
        {
          Email = dto.ContactInfo.Email,
          TelNumber = dto.ContactInfo.TelNumber,
          MobileNumber = dto.ContactInfo.MobileNumber,
          Address = dto.ContactInfo.Address,
          ProvincialAddress = dto.ContactInfo.ProvincialAddress,
          MothersMaidenName = dto.ContactInfo.MothersMaidenName,
          FathersName = dto.ContactInfo.FathersName,
          EmergencyContact = dto.ContactInfo.EmergencyContact,
          RelationshipContact = dto.ContactInfo.RelationshipContact,
          AddressContact = dto.ContactInfo.AddressContact,
          TelNoContact = dto.ContactInfo.TelNoContact,
          MobileNoContact = dto.ContactInfo.MobileNoContact
        },
      EducationInfo = dto.EducationInfo is null
        ? new EducationInfoSummary()
        : new EducationInfoSummary
        {
          EducationalAttainment = dto.EducationInfo.EducationalAttainment,
          CourseGraduated = dto.EducationInfo.CourseGraduated,
          UniversityGraduated = dto.EducationInfo.UniversityGraduated
        },
      EmploymentInfo = dto.EmploymentInfo is null
        ? new EmploymentInfoSummary()
        : new EmploymentInfoSummary
        {
          DateHired = dto.EmploymentInfo.DateHired,
          DateRegularized = dto.EmploymentInfo.DateRegularized,
          DateResigned = dto.EmploymentInfo.DateResigned,
          DateTerminated = dto.EmploymentInfo.DateTerminated,
          TinNo = dto.EmploymentInfo.TinNo,
          SssNo = dto.EmploymentInfo.SssNo,
          HdmfNo = dto.EmploymentInfo.HdmfNo,
          PhicNo = dto.EmploymentInfo.PhicNo,
          BankAccount = dto.EmploymentInfo.BankAccount,
          HasServiceCharge = dto.EmploymentInfo.HasServiceCharge
        },
      User = dto.User is null
        ? new UserSummary()
        : new UserSummary
        {
          Username = dto.User.Username,
          PasswordHash = dto.User.PasswordHash,
          Credential = dto.User.Credential,
          SupervisedGroup = dto.User.SupervisedGroup,
          IsGlobalSupervisor = dto.User.IsGlobalSupervisor
        },
      Pay = dto.Pay is null
        ? new PaySummary()
        : new PaySummary
        {
          BasicPay = dto.Pay.BasicPay,
          DailyRate = dto.Pay.DailyRate,
          HourlyRate = dto.Pay.HourlyRate,
          HdmfCon = dto.Pay.HdmfCon,
          HdmfEr = dto.Pay.HdmfEr
        }
    };
  }

  private static EmployeeSummary MapUpdatedEmployee(EmployeeRecordDto record, UpdateEmployeeRequestDto request, EmployeeSummary existingSummary)
  {
    ContactInfoSummary contactInfo = record.ContactInfo is null
      ? existingSummary.ContactInfo
      : new ContactInfoSummary
      {
        Email = record.ContactInfo.Email,
        TelNumber = record.ContactInfo.TelNumber,
        MobileNumber = record.ContactInfo.MobileNumber,
        Address = record.ContactInfo.Address,
        ProvincialAddress = record.ContactInfo.ProvincialAddress,
        MothersMaidenName = record.ContactInfo.MothersMaidenName,
        FathersName = record.ContactInfo.FathersName,
        EmergencyContact = record.ContactInfo.EmergencyContact,
        RelationshipContact = record.ContactInfo.RelationshipContact,
        AddressContact = record.ContactInfo.AddressContact,
        TelNoContact = record.ContactInfo.TelNoContact,
        MobileNoContact = record.ContactInfo.MobileNoContact
      };

    EducationInfoSummary educationInfo = record.EducationInfo is null
      ? existingSummary.EducationInfo
      : new EducationInfoSummary
      {
        EducationalAttainment = record.EducationInfo.EducationalAttainment,
        CourseGraduated = record.EducationInfo.CourseGraduated,
        UniversityGraduated = record.EducationInfo.UniversityGraduated
      };

    EmploymentInfoSummary employmentInfo = record.EmploymentInfo is null
      ? existingSummary.EmploymentInfo
      : new EmploymentInfoSummary
      {
        DateHired = record.EmploymentInfo.DateHired,
        DateRegularized = record.EmploymentInfo.DateRegularized,
        DateResigned = record.EmploymentInfo.DateResigned,
        DateTerminated = record.EmploymentInfo.DateTerminated,
        TinNo = record.EmploymentInfo.TinNo,
        SssNo = record.EmploymentInfo.SssNo,
        HdmfNo = record.EmploymentInfo.HdmfNo,
        PhicNo = record.EmploymentInfo.PhicNo,
        BankAccount = record.EmploymentInfo.BankAccount,
        HasServiceCharge = record.EmploymentInfo.HasServiceCharge
      };

    UserSummary user = record.User is null
      ? existingSummary.User
      : new UserSummary
      {
        Username = record.User.Username,
        PasswordHash = record.User.PasswordHash,
        Credential = record.User.Credential,
        SupervisedGroup = record.User.SupervisedGroup,
        IsGlobalSupervisor = record.User.IsGlobalSupervisor
      };

    PaySummary pay = record.Pay is null
      ? existingSummary.Pay
      : new PaySummary
      {
        BasicPay = record.Pay.BasicPay ?? existingSummary.Pay.BasicPay,
        DailyRate = record.Pay.DailyRate ?? existingSummary.Pay.DailyRate,
        HourlyRate = record.Pay.HourlyRate ?? existingSummary.Pay.HourlyRate,
        HdmfCon = record.Pay.HdmfEmployeeContribution ?? existingSummary.Pay.HdmfCon,
        HdmfEr = record.Pay.HdmfEmployerContribution ?? existingSummary.Pay.HdmfEr
      };

    string? image = record.Image is { Length: > 0 }
      ? Convert.ToBase64String(record.Image)
      : existingSummary.Image;

    return new EmployeeSummary
    {
      Id = record.Id,
      FirstName = record.FirstName,
      LastName = record.LastName,
      MiddleName = record.MiddleName,
      DateOfBirth = record.DateOfBirth,
      Gender = record.Gender,
      CivilStatus = record.CivilStatus,
      BloodType = record.BloodType,
      Status = record.Status,
      Height = record.Height,
      Weight = record.Weight,
      Remarks = record.Remarks,
      MealCredits = record.MealCredits,
      Tenant = record.Tenant,
      Department = record.Department ?? existingSummary.Department,
      Section = record.Section ?? existingSummary.Section,
      Position = record.Position ?? existingSummary.Position,
      Type = record.Type ?? existingSummary.Type,
      Level = record.Level ?? existingSummary.Level,
      Image = image,
      IsDeleted = existingSummary.IsDeleted,
      UserId = request.UserId ?? existingSummary.UserId,
      PositionId = request.PositionId ?? existingSummary.PositionId,
      PayId = request.PayId ?? existingSummary.PayId,
      TypeId = request.TypeId ?? existingSummary.TypeId,
      LevelId = request.LevelId ?? existingSummary.LevelId,
      ChargingId = request.ChargingId ?? existingSummary.ChargingId,
      CreatedDate = existingSummary.CreatedDate,
      CreateBy = existingSummary.CreateBy,
      UpdatedDate = existingSummary.UpdatedDate,
      UpdateBy = existingSummary.UpdateBy,
      ContactInfo = contactInfo,
      EducationInfo = educationInfo,
      EmploymentInfo = employmentInfo,
      User = user,
      Pay = pay,
      RowIndex = existingSummary.RowIndex
    };
  }
}
