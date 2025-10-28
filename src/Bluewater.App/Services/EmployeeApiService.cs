using System;
using System.Collections.Generic;
using System.Linq;
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

}
