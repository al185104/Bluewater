using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.UseCases.Pays;
using Bluewater.UseCases.Users;

namespace Bluewater.UseCases.Employees;

public record EmployeeDTO()
{
    public Guid Id { get; init; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set;}
    public Gender Gender { get; set; }
    public CivilStatus CivilStatus { get; set; }
    public BloodType BloodType { get; set; }
    public Status Status { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public byte[]? ImageUrl { get; set; }
    public string? Remarks { get; set; }
    public ContactInfoDTO? ContactInfo { get; set; }
    public EducationInfoDTO? EducationInfo { get; set; }
    public EmploymentInfoDTO? EmploymentInfo { get; set; }
    public UserDTO? User { get; set; }
    public string? Position { get; set; }
    public string? Section { get; set; }
    public string? Department { get; set; }
    public string? Division { get; set; }
    public string? Charging { get; set; }
    public PayDTO? Pay { get; set; }
    public string? Type { get; set; }
    public string? Level { get; set; }
    public int MealCredits { get; set; }
    public Guid? UserId { get; set; }
    public Guid? PositionId { get; set; }
    public Guid? PayId { get; set; }
    public Guid? TypeId { get; set; }
    public Guid? LevelId { get; set; }
    public Guid? ChargingId { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid CreateBy { get; set; }
    public DateTime UpdatedDate { get; set; }
    public Guid UpdateBy { get; set; }
    public Tenant Tenant { get; set; } = Tenant.Maribago;

    public EmployeeDTO(Guid id, string firstName, string lastName, string? middleName, DateTime? dateOfBirth, Gender gender, CivilStatus civilStatus, BloodType bloodType, Status status, decimal? height, decimal? weight, byte[]? imageUrl, string? remarks, ContactInfoDTO? contactInfo, EducationInfoDTO? educationInfo, EmploymentInfoDTO? employmentInfo, UserDTO? user, string? position, string? section, string? department, string? division, string? charging, PayDTO? pay, string? type, string? level, int? mealCredit = 0, Guid? userId = null, Guid? positionId = null, Guid? payId = null, Guid? typeId = null, Guid? levelId = null, Guid? chargingId = null, DateTime? createdDate = null, Guid? createBy = null, DateTime? updatedDate = null, Guid? updateBy = null, Tenant? tenant = Tenant.Maribago) : this()
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
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
        ContactInfo = contactInfo;
        EducationInfo = educationInfo;
        EmploymentInfo = employmentInfo;
        User = user;
        Position = position;
        Section = section;
        Department = department;
        Division = division;
        Charging = charging;
        Pay = pay;
        Type = type;
        Level = level;
        MealCredits = mealCredit ?? 0;
        UserId = userId;
        PositionId = positionId;
        PayId = payId;
        TypeId = typeId;
        LevelId = levelId;
        ChargingId = chargingId;
        CreatedDate = createdDate ?? default;
        CreateBy = createBy ?? Guid.Empty;
        UpdatedDate = updatedDate ?? default;
        UpdateBy = updateBy ?? Guid.Empty;
        Tenant = tenant ?? Tenant.Maribago;
  }
}

public record ContactInfoDTO(
    string? Email,
    string? TelNumber,
    string? MobileNumber,
    string? Address,
    string? ProvincialAddress,
    string? MothersMaidenName,
    string? FathersName,
    string? EmergencyContact,
    string? RelationshipContact,
    string? AddressContact,
    string? TelNoContact,
    string? MobileNoContact
);

public record EducationInfoDTO()
{
    public EducationalAttainment EducationalAttainment { get; set; }
    public string? CourseGraduated { get; set; }
    public string? UniversityGraduated { get; set; }

    public EducationInfoDTO(EducationalAttainment educationalAttainment, string? courseGraduated, string? universityGraduated) : this()
    {
        EducationalAttainment = educationalAttainment;
        CourseGraduated = courseGraduated;
        UniversityGraduated = universityGraduated;
    }
}

public record EmploymentInfoDTO(
    DateTime? DateHired,
    DateTime? DateRegularized,
    DateTime? DateResigned,
    DateTime? DateTerminated,
    string? TinNo,
    string? SssNo,
    string? PagibigNo,
    string? PhilHealthNo,
    string? BankAccount,
    bool HasServiceCharge
);
