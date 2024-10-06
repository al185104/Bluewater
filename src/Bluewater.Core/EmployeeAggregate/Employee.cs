using Ardalis.SharedKernel;
using Bluewater.Core.ChargingAggregate;
using Bluewater.Core.DependentAggregate;
using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.EmployeeTypeAggregate;
using Bluewater.Core.LeaveCreditAggregate;
using Bluewater.Core.LevelAggregate;
using Bluewater.Core.PayAggregate;
using Bluewater.Core.PositionAggregate;
using Bluewater.Core.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Bluewater.Core.EmployeeAggregate;
public class Employee : EntityBase<Guid>, IAggregateRoot
{
  // Personal Information
  public string FirstName { get; private set; } = null!;
  public string LastName { get; private set; } = null!;
  public string? MiddleName { get; private set; }
  public DateTime? DateOfBirth { get; private set; }
  public Gender Gender { get; private set; } = Gender.NotSet;
  public CivilStatus CivilStatus { get; private set; } = CivilStatus.NotSet;
  public BloodType BloodType { get; private set; } = BloodType.NotSet;
  public Status Status { get; private set; } = Status.NotSet;
  public decimal? Height { get; private set; }
  public decimal? Weight { get; private set; }
  public bool IsDeleted { get; private set; } = false;
  public byte[]? ImageUrl { get; private set; }  
  public string? Remarks { get; private set; }
  
  public ContactInfo? ContactInfo { get; private set; }
  public EducationInfo? EducationInfo { get; private set; }
  public EmploymentInfo? EmploymentInfo { get; private set; }

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  // Foreign Keys
  public Guid PositionId { get; private set; }
  public Guid PayId { get; private set; }
  public Guid TypeId { get; private set; }
  public Guid LevelId { get; private set; }
  public Guid ChargingId { get; private set; }

  // Navigation Properties
  public virtual User? User { get; private set; }
  
  public virtual Position? Position { get; private set; }
  public virtual Pay? Pay { get; private set; }
  public virtual EmployeeType? Type { get; private set; }
  public virtual Level? Level { get; private set; }
  public virtual Charging? Charging { get; private set; }

  
  public virtual ICollection<LeaveCredit>? LeaveCredits { get; private set; }
  public virtual ICollection<Dependent>? Dependents { get; private set; }
}

#region ContactInfo
[Owned]
public class ContactInfo : ValueObject
{
    public string Email { get; private set; }
    public string TelNumber { get; private set; }
    public string MobileNumber { get; private set; }
    public string Address { get; private set; }
    public string ProvincialAddress { get; private set; }
    public string? MothersMaidenName { get; private set; }
    public string? FathersName { get; private set; }
    public string? EmergencyContact { get; private set; }
    public string? RelationshipContact { get; private set; }
    public string? AddressContact { get; private set; }
    public string? TelNoContact { get; private set; }
    public string? MobileNoContact { get; private set; }

    public ContactInfo(string email,
                       string telNumber,
                       string mobileNumber,
                       string address,
                       string provincialAddress,
                       string? mothersMaidenName,
                       string? fathersName,
                       string? emergencyContact,
                       string? relationshipContact,
                       string? addressContact,
                       string? telNoContact,
                       string? mobileNoContact)
    {
        Email = email;
        TelNumber = telNumber;
        MobileNumber = mobileNumber;
        Address = address;
        ProvincialAddress = provincialAddress;
        MothersMaidenName = mothersMaidenName;
        FathersName = fathersName;
        EmergencyContact = emergencyContact;
        RelationshipContact = relationshipContact;
        AddressContact = addressContact;
        TelNoContact = telNoContact;
        MobileNoContact = mobileNoContact;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Email;
        yield return TelNumber;
        yield return MobileNumber;
        yield return Address;
        yield return ProvincialAddress;
        yield return MothersMaidenName ?? string.Empty;
        yield return FathersName ?? string.Empty;
        yield return EmergencyContact ?? string.Empty;
        yield return RelationshipContact ?? string.Empty;
        yield return AddressContact ?? string.Empty;
        yield return TelNoContact ?? string.Empty;
        yield return MobileNoContact ?? string.Empty;
    }

    private ContactInfo()
    {
        Email = string.Empty;
        TelNumber = string.Empty;
        MobileNumber = string.Empty;
        Address = string.Empty;
        ProvincialAddress = string.Empty;
    }
}
#endregion

#region EmploymentInfo
[Owned]
public class EmploymentInfo : ValueObject
{
  public DateTime? DateHired { get; private set; }
  public DateTime? DateRegularized { get; private set; }
  public DateTime? DateResigned { get; private set; }
  public DateTime? DateTerminated { get; private set; }
  public string? TINNo { get; private set; }
  public string? SSSNo { get; private set; }
  public string? HDMFNo { get; private set; }
  public string? PHICNo { get; private set; }
  public string? BankAccount { get; private set; }
  public bool HasServiceCharge { get; private set; }

  private EmploymentInfo(){}

  public EmploymentInfo(DateTime? dateHired,
                        DateTime? dateRegularized,
                        DateTime? dateResigned,
                        DateTime? dateTerminated,
                        string? tinNo,
                        string? sssNo,
                        string? hdmfNo,
                        string? phicNo,
                        string? bankAccount,
                        bool hasServiceCharge)
  {
    DateHired = dateHired;
    DateRegularized = dateRegularized;
    DateResigned = dateResigned;
    DateTerminated = dateTerminated;
    TINNo = tinNo;
    SSSNo = sssNo;
    HDMFNo = hdmfNo;
    PHICNo = phicNo;
    BankAccount = bankAccount;
    HasServiceCharge = hasServiceCharge;
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return DateHired ?? DateTime.MinValue;
    yield return DateRegularized ?? DateTime.MinValue;
    yield return DateResigned ?? DateTime.MinValue;
    yield return DateTerminated ?? DateTime.MinValue;
    yield return TINNo ?? string.Empty;
    yield return SSSNo ?? string.Empty;
    yield return HDMFNo ?? string.Empty;
    yield return PHICNo ?? string.Empty;
    yield return BankAccount ?? string.Empty;
    yield return HasServiceCharge;
  }
}
#endregion

#region Education Info
[Owned]
public class EducationInfo : ValueObject
{
  public string PrimarySchool { get; private set; }
  public string SecondarySchool { get; private set; }
  public string TertiarySchool { get; private set; }
  public string VocationalSchool { get; private set; }
  public string? PrimaryDegree { get; private set; }
  public string? SecondaryDegree { get; private set; }
  public string? TertiaryDegree { get; private set; }
  public string? VocationalDegree { get; private set; }

  public EducationInfo(string primarySchool, string secondarySchool, string tertiarySchool, string vocationalSchool, string? primaryDegree = null, string? secondaryDegree = null, string? tertiaryDegree = null, string? vocationalDegree = null)
  {
      PrimarySchool = primarySchool;
      SecondarySchool = secondarySchool;
      TertiarySchool = tertiarySchool;
      VocationalSchool = vocationalSchool;
      PrimaryDegree = primaryDegree;
      SecondaryDegree = secondaryDegree;
      TertiaryDegree = tertiaryDegree;
      VocationalDegree = vocationalDegree;
  }

  private EducationInfo() {
    PrimarySchool = string.Empty;
    SecondarySchool = string.Empty;
    TertiarySchool = string.Empty;
    VocationalSchool = string.Empty;
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
      yield return PrimarySchool;
      yield return SecondarySchool;
      yield return TertiarySchool;
      yield return VocationalSchool;
      yield return PrimaryDegree ?? string.Empty;
      yield return SecondaryDegree ?? string.Empty;
      yield return TertiaryDegree ?? string.Empty;
      yield return VocationalDegree ?? string.Empty;
  }
}
#endregion
