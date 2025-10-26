using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.Core.EmployeeAggregate.Enum;
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

  public IReadOnlyList<Gender> GenderOptions { get; } = Enum.GetValues<Gender>();
  public IReadOnlyList<CivilStatus> CivilStatusOptions { get; } = Enum.GetValues<CivilStatus>();
  public IReadOnlyList<BloodType> BloodTypeOptions { get; } = Enum.GetValues<BloodType>();
  public IReadOnlyList<Status> StatusOptions { get; } = Enum.GetValues<Status>();
  public IReadOnlyList<Tenant> TenantOptions { get; } = Enum.GetValues<Tenant>();
  public IReadOnlyList<EducationalAttainment> EducationalAttainmentOptions { get; } = Enum.GetValues<EducationalAttainment>();

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
      HasServiceCharge = summary.EmploymentInfo.HasServiceCharge
    };
  }

  public EmployeeSummary ToSummary(int rowIndex)
  {
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
      RowIndex = rowIndex
    };
  }
}
