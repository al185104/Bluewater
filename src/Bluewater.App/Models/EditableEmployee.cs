using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.UserAggregate.Enum;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Bluewater.App.Models;

public partial class EditableEmployee : ObservableObject
{
  public Guid Id { get; set; }

  [ObservableProperty]
  public partial string FirstName { get; set; } = string.Empty;

  [ObservableProperty]
  public partial string LastName { get; set; } = string.Empty;

  [ObservableProperty]
  public partial string? MiddleName { get; set; }

  [ObservableProperty]
  public partial DateTime? DateOfBirth { get; set; }

  [ObservableProperty]
  public partial Gender Gender { get; set; } = Gender.NotSet;

  [ObservableProperty]
  public partial CivilStatus CivilStatus { get; set; } = CivilStatus.NotSet;

  [ObservableProperty]
  public partial BloodType BloodType { get; set; } = BloodType.NotSet;

  [ObservableProperty]
  public partial Status Status { get; set; } = Status.NotSet;

  [ObservableProperty]
  public partial decimal? Height { get; set; }

  [ObservableProperty]
  public partial decimal? Weight { get; set; }

  [ObservableProperty]
  public partial string? Remarks { get; set; }

  [ObservableProperty]
  public partial int MealCredits { get; set; }

  [ObservableProperty]
  public partial Tenant Tenant { get; set; } = Tenant.Maribago;

  [ObservableProperty]
  public partial string? Position { get; set; }

  [ObservableProperty]
  public partial string? Department { get; set; }

  [ObservableProperty]
  public partial string? Section { get; set; }

  [ObservableProperty]
  public partial string? Type { get; set; }

  [ObservableProperty]
  public partial string? Level { get; set; }

  [ObservableProperty]
  public partial string? Image { get; set; }

  [ObservableProperty]
  public partial bool IsDeleted { get; set; }

  [ObservableProperty]
  public partial Guid? UserId { get; set; }

  [ObservableProperty]
  public partial Guid? PositionId { get; set; }

  [ObservableProperty]
  public partial Guid? PayId { get; set; }

  [ObservableProperty]
  public partial Guid? TypeId { get; set; }

  [ObservableProperty]
  public partial Guid? LevelId { get; set; }

  [ObservableProperty]
  public partial Guid? ChargingId { get; set; }

  [ObservableProperty]
  public partial DateTime CreatedDate { get; set; } = DateTime.UtcNow;

  [ObservableProperty]
  public partial Guid CreateBy { get; set; } = Guid.Empty;

  [ObservableProperty]
  public partial DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

  [ObservableProperty]
  public partial Guid UpdateBy { get; set; } = Guid.Empty;

  [ObservableProperty]
  public partial string? Email { get; set; }

  [ObservableProperty]
  public partial string? TelNumber { get; set; }

  [ObservableProperty]
  public partial string? MobileNumber { get; set; }

  [ObservableProperty]
  public partial string? Address { get; set; }

  [ObservableProperty]
  public partial string? ProvincialAddress { get; set; }

  [ObservableProperty]
  public partial string? MothersMaidenName { get; set; }

  [ObservableProperty]
  public partial string? FathersName { get; set; }

  [ObservableProperty]
  public partial string? EmergencyContact { get; set; }

  [ObservableProperty]
  public partial string? RelationshipContact { get; set; }

  [ObservableProperty]
  public partial string? AddressContact { get; set; }

  [ObservableProperty]
  public partial string? TelNoContact { get; set; }

  [ObservableProperty]
  public partial string? MobileNoContact { get; set; }

  [ObservableProperty]
  public partial EducationalAttainment EducationalAttainment { get; set; } = EducationalAttainment.NotSet;

  [ObservableProperty]
  public partial string? CourseGraduated { get; set; }

  [ObservableProperty]
  public partial string? UniversityGraduated { get; set; }

  [ObservableProperty]
  public partial DateTime? DateHired { get; set; }

  [ObservableProperty]
  public partial DateTime? DateRegularized { get; set; }

  [ObservableProperty]
  public partial DateTime? DateResigned { get; set; }

  [ObservableProperty]
  public partial DateTime? DateTerminated { get; set; }

  [ObservableProperty]
  public partial string? TinNo { get; set; }

  [ObservableProperty]
  public partial string? SssNo { get; set; }

  [ObservableProperty]
  public partial string? HdmfNo { get; set; }

  [ObservableProperty]
  public partial string? PhicNo { get; set; }

  [ObservableProperty]
  public partial string? BankAccount { get; set; }

  [ObservableProperty]
  public partial bool HasServiceCharge { get; set; }

  [ObservableProperty]
  public partial string Username { get; set; } = string.Empty;

  [ObservableProperty]
  public partial string PasswordHash { get; set; } = string.Empty;

  [ObservableProperty]
  public partial Credential Credential { get; set; } = Credential.None;

  [ObservableProperty]
  public partial bool IsGlobalSupervisor { get; set; }

  [ObservableProperty]
  public partial decimal? BasicPay { get; set; }

  [ObservableProperty]
  public partial decimal? DailyRate { get; set; }

  [ObservableProperty]
  public partial decimal? HourlyRate { get; set; }

  [ObservableProperty]
  public partial decimal? HdmfCon { get; set; }

  [ObservableProperty]
  public partial decimal? HdmfEr { get; set; }

  [ObservableProperty]
  public partial string? SupervisedGroup { get; set; }

  public IReadOnlyList<Gender> GenderOptions { get; } = Enum.GetValues<Gender>();
  public IReadOnlyList<CivilStatus> CivilStatusOptions { get; } = Enum.GetValues<CivilStatus>();
  public IReadOnlyList<BloodType> BloodTypeOptions { get; } = Enum.GetValues<BloodType>();
  public IReadOnlyList<Status> StatusOptions { get; } = Enum.GetValues<Status>();
  public IReadOnlyList<Tenant> TenantOptions { get; } = Enum.GetValues<Tenant>();
  public IReadOnlyList<EducationalAttainment> EducationalAttainmentOptions { get; } = Enum.GetValues<EducationalAttainment>();
  public IReadOnlyList<Credential> CredentialOptions { get; } = Enum.GetValues<Credential>();

  public string FullName
  {
    get
    {
      return string.Join(' ', new[] { FirstName, MiddleName, LastName }
        .Where(part => !string.IsNullOrWhiteSpace(part)));
    }
  }

  public static EditableEmployee FromSummary(EmployeeSummary summary)
  {
    return new EditableEmployee
    {
      Id = summary.Id,
      FirstName = summary.FirstName,
      LastName = summary.LastName,
      MiddleName = summary.MiddleName,
      DateOfBirth = summary.DateOfBirth,
      Gender = summary.Gender,
      CivilStatus = summary.CivilStatus,
      BloodType = summary.BloodType,
      Status = summary.Status,
      Height = summary.Height,
      Weight = summary.Weight,
      Remarks = summary.Remarks,
      MealCredits = summary.MealCredits,
      Tenant = summary.Tenant,
      Position = summary.Position,
      Department = summary.Department,
      Section = summary.Section,
      Type = summary.Type,
      Level = summary.Level,
      Image = summary.Image,
      IsDeleted = summary.IsDeleted,
      UserId = summary.UserId,
      PositionId = summary.PositionId,
      PayId = summary.PayId,
      TypeId = summary.TypeId,
      LevelId = summary.LevelId,
      ChargingId = summary.ChargingId,
      CreatedDate = summary.CreatedDate,
      CreateBy = summary.CreateBy,
      UpdatedDate = summary.UpdatedDate,
      UpdateBy = summary.UpdateBy,
      Email = summary.ContactInfo.Email,
      TelNumber = summary.ContactInfo.TelNumber,
      MobileNumber = summary.ContactInfo.MobileNumber,
      Address = summary.ContactInfo.Address,
      ProvincialAddress = summary.ContactInfo.ProvincialAddress,
      MothersMaidenName = summary.ContactInfo.MothersMaidenName,
      FathersName = summary.ContactInfo.FathersName,
      EmergencyContact = summary.ContactInfo.EmergencyContact,
      RelationshipContact = summary.ContactInfo.RelationshipContact,
      AddressContact = summary.ContactInfo.AddressContact,
      TelNoContact = summary.ContactInfo.TelNoContact,
      MobileNoContact = summary.ContactInfo.MobileNoContact,
      EducationalAttainment = summary.EducationInfo.EducationalAttainment,
      CourseGraduated = summary.EducationInfo.CourseGraduated,
      UniversityGraduated = summary.EducationInfo.UniversityGraduated,
      DateHired = summary.EmploymentInfo.DateHired,
      DateRegularized = summary.EmploymentInfo.DateRegularized,
      DateResigned = summary.EmploymentInfo.DateResigned,
      DateTerminated = summary.EmploymentInfo.DateTerminated,
      TinNo = summary.EmploymentInfo.TinNo,
      SssNo = summary.EmploymentInfo.SssNo,
      HdmfNo = summary.EmploymentInfo.HdmfNo,
      PhicNo = summary.EmploymentInfo.PhicNo,
      BankAccount = summary.EmploymentInfo.BankAccount,
      HasServiceCharge = summary.EmploymentInfo.HasServiceCharge,
      Username = summary.User.Username,
      PasswordHash = summary.User.PasswordHash,
      Credential = summary.User.Credential,
      SupervisedGroup = summary.User.SupervisedGroup?.ToString(),
      IsGlobalSupervisor = summary.User.IsGlobalSupervisor,
      BasicPay = summary.Pay.BasicPay,
      DailyRate = summary.Pay.DailyRate,
      HourlyRate = summary.Pay.HourlyRate,
      HdmfCon = summary.Pay.HdmfCon,
      HdmfEr = summary.Pay.HdmfEr
    };
  }

  public EmployeeSummary ToSummary(int rowIndex)
  {
    Guid? supervisedGroupId = Guid.TryParse(SupervisedGroup, out Guid parsedSupervisedGroup)
      ? parsedSupervisedGroup
      : null;

    return new EmployeeSummary
    {
      Id = Id,
      FirstName = FirstName,
      LastName = LastName,
      MiddleName = MiddleName,
      DateOfBirth = DateOfBirth,
      Gender = Gender,
      CivilStatus = CivilStatus,
      BloodType = BloodType,
      Status = Status,
      Height = Height,
      Weight = Weight,
      Remarks = Remarks,
      MealCredits = MealCredits,
      Tenant = Tenant,
      Position = Position,
      Department = Department,
      Section = Section,
      Type = Type,
      Level = Level,
      Image = Image,
      IsDeleted = IsDeleted,
      UserId = UserId,
      PositionId = PositionId,
      PayId = PayId,
      TypeId = TypeId,
      LevelId = LevelId,
      ChargingId = ChargingId,
      CreatedDate = CreatedDate,
      CreateBy = CreateBy,
      UpdatedDate = UpdatedDate,
      UpdateBy = UpdateBy,
      ContactInfo = new ContactInfoSummary
      {
        Email = Email,
        TelNumber = TelNumber,
        MobileNumber = MobileNumber,
        Address = Address,
        ProvincialAddress = ProvincialAddress,
        MothersMaidenName = MothersMaidenName,
        FathersName = FathersName,
        EmergencyContact = EmergencyContact,
        RelationshipContact = RelationshipContact,
        AddressContact = AddressContact,
        TelNoContact = TelNoContact,
        MobileNoContact = MobileNoContact
      },
      EducationInfo = new EducationInfoSummary
      {
        EducationalAttainment = EducationalAttainment,
        CourseGraduated = CourseGraduated,
        UniversityGraduated = UniversityGraduated
      },
      EmploymentInfo = new EmploymentInfoSummary
      {
        DateHired = DateHired,
        DateRegularized = DateRegularized,
        DateResigned = DateResigned,
        DateTerminated = DateTerminated,
        TinNo = TinNo,
        SssNo = SssNo,
        HdmfNo = HdmfNo,
        PhicNo = PhicNo,
        BankAccount = BankAccount,
        HasServiceCharge = HasServiceCharge
      },
      User = new UserSummary
      {
        Username = Username,
        PasswordHash = PasswordHash,
        Credential = Credential,
        SupervisedGroup = supervisedGroupId,
        IsGlobalSupervisor = IsGlobalSupervisor
      },
      Pay = new PaySummary
      {
        BasicPay = BasicPay ?? 0m,
        DailyRate = DailyRate ?? 0m,
        HourlyRate = HourlyRate ?? 0m,
        HdmfCon = HdmfCon ?? 0m,
        HdmfEr = HdmfEr ?? 0m
      },
      RowIndex = rowIndex
    };
  }

  public UpdateEmployeeRequestDto ToUpdateRequest(EmployeeSummary existingSummary)
  {
    if (existingSummary is null)
    {
      throw new ArgumentNullException(nameof(existingSummary));
    }

    Guid userId = UserId ?? existingSummary.UserId ?? throw new InvalidOperationException("UserId is required to update an employee.");
    Guid positionId = PositionId ?? existingSummary.PositionId ?? throw new InvalidOperationException("PositionId is required to update an employee.");
    Guid payId = PayId ?? existingSummary.PayId ?? throw new InvalidOperationException("PayId is required to update an employee.");
    Guid typeId = TypeId ?? existingSummary.TypeId ?? throw new InvalidOperationException("TypeId is required to update an employee.");
    Guid levelId = LevelId ?? existingSummary.LevelId ?? throw new InvalidOperationException("LevelId is required to update an employee.");
    Guid chargingId = ChargingId ?? existingSummary.ChargingId ?? throw new InvalidOperationException("ChargingId is required to update an employee.");

    return new UpdateEmployeeRequestDto
    {
      Id = Id,
      FirstName = FirstName,
      LastName = LastName,
      MiddleName = MiddleName,
      DateOfBirth = DateOfBirth,
      Gender = Gender,
      CivilStatus = CivilStatus,
      BloodType = BloodType,
      Status = Status,
      Height = Height,
      Weight = Weight,
      Image = ConvertImageToBytes(Image),
      Remarks = Remarks,
      MealCredits = MealCredits,
      Tenant = Tenant,
      ContactInfo = CreateContactInfoRequest(),
      EducationInfo = CreateEducationInfoRequest(),
      EmploymentInfo = CreateEmploymentInfoRequest(),
      UserId = userId,
      PositionId = positionId,
      PayId = payId,
      TypeId = typeId,
      LevelId = levelId,
      ChargingId = chargingId
    };

    UpdateEmployeeContactInfoDto? CreateContactInfoRequest()
    {
      UpdateEmployeeContactInfoDto contactInfo = new()
      {
        Email = Email,
        TelNumber = TelNumber,
        MobileNumber = MobileNumber,
        Address = Address,
        ProvincialAddress = ProvincialAddress,
        MothersMaidenName = MothersMaidenName,
        FathersName = FathersName,
        EmergencyContact = EmergencyContact,
        RelationshipContact = RelationshipContact,
        AddressContact = AddressContact,
        TelNoContact = TelNoContact,
        MobileNoContact = MobileNoContact
      };

      return IsContactInfoEmpty(contactInfo) ? null : contactInfo;
    }

    UpdateEmployeeEducationInfoDto CreateEducationInfoRequest()
    {
      return new UpdateEmployeeEducationInfoDto
      {
        EducationalAttainment = EducationalAttainment,
        CourseGraduated = CourseGraduated,
        UniversityGraduated = UniversityGraduated
      };
    }

    UpdateEmployeeEmploymentInfoDto CreateEmploymentInfoRequest()
    {
      return new UpdateEmployeeEmploymentInfoDto
      {
        DateHired = DateHired,
        DateRegularized = DateRegularized,
        DateResigned = DateResigned,
        DateTerminated = DateTerminated,
        TinNo = TinNo,
        SssNo = SssNo,
        HdmfNo = HdmfNo,
        PhicNo = PhicNo,
        BankAccount = BankAccount,
        HasServiceCharge = HasServiceCharge
      };
    }
  }

  private static byte[]? ConvertImageToBytes(string? image)
  {
    if (string.IsNullOrWhiteSpace(image))
    {
      return null;
    }

    try
    {
      return Convert.FromBase64String(image);
    }
    catch (FormatException)
    {
      return null;
    }
  }

  private static bool IsContactInfoEmpty(UpdateEmployeeContactInfoDto contactInfo)
  {
    return string.IsNullOrWhiteSpace(contactInfo.Email)
      && string.IsNullOrWhiteSpace(contactInfo.TelNumber)
      && string.IsNullOrWhiteSpace(contactInfo.MobileNumber)
      && string.IsNullOrWhiteSpace(contactInfo.Address)
      && string.IsNullOrWhiteSpace(contactInfo.ProvincialAddress)
      && string.IsNullOrWhiteSpace(contactInfo.MothersMaidenName)
      && string.IsNullOrWhiteSpace(contactInfo.FathersName)
      && string.IsNullOrWhiteSpace(contactInfo.EmergencyContact)
      && string.IsNullOrWhiteSpace(contactInfo.RelationshipContact)
      && string.IsNullOrWhiteSpace(contactInfo.AddressContact)
      && string.IsNullOrWhiteSpace(contactInfo.TelNoContact)
      && string.IsNullOrWhiteSpace(contactInfo.MobileNoContact);
  }
}
