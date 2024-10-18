using Ardalis.GuardClauses;
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
public class Employee(string firstName, string lastName, string? middleName, DateTime? dateOfBirth, Gender gender, CivilStatus civilStatus, BloodType bloodType, Status status, decimal? height, decimal? weight, byte[]? imageUrl, string? remarks) : EntityBase<Guid>, IAggregateRoot
{
  // Personal Information
  public string FirstName { get; private set; } = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
  public string LastName { get; private set; } = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
  public string? MiddleName { get; private set; } = middleName;
  public DateTime? DateOfBirth { get; private set; } = dateOfBirth;
  public Gender Gender { get; private set; } = gender; 
  public CivilStatus CivilStatus { get; private set; } = civilStatus;
  public BloodType BloodType { get; private set; } = bloodType;
  public Status Status { get; private set; } = status;
  public decimal? Height { get; private set; } = height;
  public decimal? Weight { get; private set; } = weight;
  public bool IsDeleted { get; private set; } = false;
  public byte[]? ImageUrl { get; private set; }  = imageUrl;
  public string? Remarks { get; private set; } = remarks;
  
  public ContactInfo? ContactInfo { get; private set; }
  public EducationInfo? EducationInfo { get; private set; }
  public EmploymentInfo? EmploymentInfo { get; private set; }

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;

  // Foreign Keys
  public Guid? UserId { get; private set; }
  public Guid? PositionId { get; private set; }
  public Guid? PayId { get; private set; }
  public Guid? TypeId { get; private set; }
  public Guid? LevelId { get; private set; }
  public Guid? ChargingId { get; private set; }

  // Navigation Properties
  public virtual User? User { get; private set; }  
  public virtual Position? Position { get; private set; }
  public virtual Pay? Pay { get; private set; }
  public virtual EmployeeType? Type { get; private set; }
  public virtual Level? Level { get; private set; }
  public virtual Charging? Charging { get; private set; }

  public virtual ICollection<LeaveCredit>? LeaveCredits { get; private set; }
  public virtual ICollection<Dependent>? Dependents { get; private set; }

  //public Employee() : this(string.Empty, string.Empty,  null, null, Gender.NotSet, CivilStatus.NotSet, BloodType.NotSet, Status.NotSet, null, null, null, null) { }
  public void SetContactInfo(ContactInfo contactInfo)
  {
    ContactInfo = new ContactInfo(contactInfo.Email,
                                  contactInfo.TelNumber,
                                  contactInfo.MobileNumber,
                                  contactInfo.Address,
                                  contactInfo.ProvincialAddress,
                                  contactInfo.MothersMaidenName,
                                  contactInfo.FathersName,
                                  contactInfo.EmergencyContact,
                                  contactInfo.RelationshipContact,
                                  contactInfo.AddressContact,
                                  contactInfo.TelNoContact,
                                  contactInfo.MobileNoContact);
  }

  public void SetEducationInfo(EducationInfo educationInfo)
  {
    EducationInfo = new EducationInfo(educationInfo.EducationalAttainment,
                                      educationInfo.CourseGraduated,
                                      educationInfo.UniversityGraduated);
  }

  public void SetEmploymentInfo(EmploymentInfo employmentInfo)
  {
    EmploymentInfo = new EmploymentInfo(employmentInfo.DateHired,
                                        employmentInfo.DateRegularized,
                                        employmentInfo.DateResigned,
                                        employmentInfo.DateTerminated,
                                        employmentInfo.TINNo,
                                        employmentInfo.SSSNo,
                                        employmentInfo.HDMFNo,
                                        employmentInfo.PHICNo,
                                        employmentInfo.BankAccount,
                                        employmentInfo.HasServiceCharge);
  }

  public void SetExternalKeys(Guid userId, Guid positionId, Guid payId, Guid typeId, Guid levelId, Guid chargingId)
  {
    UserId = userId;
    PositionId = positionId;
    PayId = payId;
    TypeId = typeId;
    LevelId = levelId;
    ChargingId = chargingId;
  }

  public void UpdateEmployee(string firstName, string lastName, string? middleName, DateTime? dateOfBirth, Gender gender, CivilStatus civilStatus, BloodType bloodType, Status status, decimal? height, decimal? weight, byte[]? imageUrl, string? remarks)
  {
    FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
    LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
    MiddleName = middleName;
    DateOfBirth = dateOfBirth;
    Gender = gender;
    CivilStatus = civilStatus;
    BloodType = bloodType;
    Status = status;
    Height = height;
    Weight = weight;
    ImageUrl = imageUrl;
    Remarks = remarks;
    UpdatedDate = DateTime.Now;
  }
}

#region ContactInfo
[Owned]
public class ContactInfo : ValueObject
{
    public string? Email { get; private set; }
    public string? TelNumber { get; private set; }
    public string? MobileNumber { get; private set; }
    public string? Address { get; private set; }
    public string? ProvincialAddress { get; private set; }
    public string? MothersMaidenName { get; private set; }
    public string? FathersName { get; private set; }
    public string? EmergencyContact { get; private set; }
    public string? RelationshipContact { get; private set; }
    public string? AddressContact { get; private set; }
    public string? TelNoContact { get; private set; }
    public string? MobileNoContact { get; private set; }

    public ContactInfo(string? email,
                       string? telNumber,
                       string? mobileNumber,
                       string? address,
                       string? provincialAddress,
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
        yield return Email ?? string.Empty;
        yield return TelNumber ?? string.Empty;
        yield return MobileNumber ?? string.Empty;
        yield return Address ?? string.Empty;
        yield return ProvincialAddress ?? string.Empty;
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
    public EducationalAttainment EducationalAttainment { get; private set; }
    public string? CourseGraduated { get; private set; }
    public string? UniversityGraduated { get; private set; }  

  public EducationInfo(EducationalAttainment educationalAttainment, string? courseGraduated, string? universityGraduated)
  {
    EducationalAttainment = educationalAttainment;
    CourseGraduated = courseGraduated;
    UniversityGraduated = universityGraduated;
  }

  private EducationInfo() {
    EducationalAttainment = EducationalAttainment.NotSet;
    CourseGraduated = string.Empty;
    UniversityGraduated = string.Empty;
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return EducationalAttainment;
    yield return CourseGraduated ?? string.Empty;
    yield return UniversityGraduated ?? string.Empty;
  }
}
#endregion
