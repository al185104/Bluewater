using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.UserAggregate.Enum;

namespace Bluewater.Server.Components.Pages.Employee;

public record EmployeeFormDTO()
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public Gender Gender { get; set; }
    public CivilStatus CivilStatus { get; set; }
    public BloodType BloodType { get; set; }
    public Status Status { get; set; }
    public decimal? Height { get; set; }
    public decimal? Weight { get; set; }
    public byte[]? ImageUrl { get; set; }
    public string? Remarks { get; set; }

    //contact info
    public string? Email { get; set; }
    public string? TelNumber { get; set; }
    public string? MobileNumber { get; set; }
    public string? Address { get; set; }
    public string? ProvincialAddress { get; set; }
    public string? MothersMaidenName { get; set; }
    public string? FathersName { get; set; }
    public string? EmergencyContact { get; set; }
    public string? RelationshipContact { get; set; }
    public string? AddressContact { get; set; }
    public string? TelNoContact { get; set; }
    public string? MobileNoContact { get; set; }

    //education info
    public EducationalAttainment EducationalAttainment { get; set; }
    public string? CourseGraduated { get; set; }
    public string? UniversityGraduated { get; set; }

    //employment info
    public DateTime? DateHired { get; set; }
    public DateTime? DateRegularized { get; set; }
    public DateTime? DateResigned { get; set; }
    public DateTime? DateTerminated { get; set; }
    public string? TinNo { get; set; }
    public string? SssNo { get; set; }
    public string? HdmfNo { get; set; }
    public string? PhicNo { get; set; }
    public string? BankAccount { get; set; }
    public bool HasServiceCharge { get; set; }

    //user info
    public string? Username { get; set; }
    public string? Password { get; set; }
    public Credential Credential { get; set; }
    public Guid? SupervisedGroup { get; set; }

    //external info
    public Guid PositionId { get; set; }
    public Guid PayId { get; set; }
    public Guid TypeId { get; set; }
    public Guid LevelId { get; set; }
    public Guid ChargingId { get; set; }

    public EmployeeFormDTO(string? firstName, string? lastName, string? middleName, DateTime? dateOfBirth, Gender gender, CivilStatus civilStatus, BloodType bloodType, Status status, decimal? height, decimal? weight, byte[]? imageUrl, string? remarks, 
        string? email, string? telNumber, string? mobileNumber, string? address, string? provincialAddress, string? mothersMaidenName, string? fathersName, string? emergencyContact, string? relationshipContact, string? addressContact, string? telNoContact, string? mobileNoContact, 
        EducationalAttainment educationalAttainment, string? courseGraduated, string? universityGraduated, 
        DateTime? dateHired, DateTime? dateRegularized, DateTime? dateResigned, DateTime? dateTerminated, string? tinNo, string? sssNo, string? hdmfNo, string? phicNo, string? bankAccount, bool hasServiceCharge, 
        string? username, string? password, Credential credential, Guid? supervisedGroup, 
        Guid positionId, Guid payId, Guid typeId, Guid levelId, Guid chargingId) : this()
    {
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
        EducationalAttainment = educationalAttainment;
        CourseGraduated = courseGraduated;
        UniversityGraduated = universityGraduated;
        DateHired = dateHired;
        DateRegularized = dateRegularized;
        DateResigned = dateResigned;
        DateTerminated = dateTerminated;
        TinNo = tinNo;
        SssNo = sssNo;
        HdmfNo = hdmfNo;
        PhicNo = phicNo;
        BankAccount = bankAccount;
        HasServiceCharge = hasServiceCharge;
        Username = username;
        Password = password;
        Credential = credential;
        SupervisedGroup = supervisedGroup;
        PositionId = positionId;
        PayId = payId;
        TypeId = typeId;
        LevelId = levelId;
        ChargingId = chargingId;
    }
}
