using Bluewater.Core.EmployeeAggregate.Enum;
using Bluewater.Core.UserAggregate.Enum;
using Bluewater.UseCases.Schedules;
using Bluewater.UseCases.Shifts;

namespace Bluewater.Server.Components.Pages.Employee;

public record EmployeeFormDTO()
{
    public Guid Id { get; init; }
    
    //personal info
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

    //pay info
    public Guid? PayId { get; set; }
    public decimal? BasicPay { get; set; }
    public decimal? DailyRate { get; set; }
    public decimal? HourlyRate { get; set; }
    public decimal? HDMF_Con { get; set; } = 200;
    public decimal? HDMF_Er { get; set; } = 200;

    //user info
    public Guid? UserId { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public Credential Credential { get; set; }
    public Guid? SupervisedGroup { get; set; }
    public bool IsGlobalSupervisor { get; set; }

    //external info
    public Guid PositionId { get; set; }
    public Guid SectionId { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid DivisionId { get; set; }
    public Guid TypeId { get; set; }
    public Guid LevelId { get; set; }
    public Guid ChargingId { get; set; }

    // meal credits
    public int MealCredits { get; set; } = 0;
    public Tenant Tenant { get; set; } = Tenant.Maribago;

    // weekly schedule
    public ScheduleDTO? SundaySchedule { get; set; } 
    public ScheduleDTO? MondaySchedule { get; set; }
    public ScheduleDTO? TuesdaySchedule { get; set; }
    public ScheduleDTO? WednesdaySchedule { get; set; }
    public ScheduleDTO? ThursdaySchedule { get; set; }
    public ScheduleDTO? FridaySchedule { get; set; }
    public ScheduleDTO? SaturdaySchedule { get; set; }
    private ShiftDTO? _sundayShift;
    public ShiftDTO? SundayShift
    {
        get => SundaySchedule?.Shift ?? _sundayShift;
        set => _sundayShift = value;
    }
    private ShiftDTO? _mondayShift;
    public ShiftDTO? MondayShift
    {
        get => MondaySchedule?.Shift ?? _mondayShift;
        set => _mondayShift = value;
    }
    private ShiftDTO? _tuesdayShift;
    public ShiftDTO? TuesdayShift
    {
        get => TuesdaySchedule?.Shift ?? _tuesdayShift;
        set => _tuesdayShift = value;
    }
    private ShiftDTO? _wednesdayShift;
    public ShiftDTO? WednesdayShift
    {
        get => WednesdaySchedule?.Shift ?? _wednesdayShift;
        set => _wednesdayShift = value;
    }
    private ShiftDTO? _thursdayShift;
    public ShiftDTO? ThursdayShift
    {
        get => ThursdaySchedule?.Shift ?? _thursdayShift;
        set => _thursdayShift = value;
    }
    private ShiftDTO? _fridayShift;
    public ShiftDTO? FridayShift
    {
        get => FridaySchedule?.Shift ?? _fridayShift;
        set => _fridayShift = value;
    }
    private ShiftDTO? _saturdayShift;
    public ShiftDTO? SaturdayShift
    {
        get => SaturdaySchedule?.Shift ?? _saturdayShift;
        set => _saturdayShift = value;
    }

    public IEnumerable<ShiftDTO> DefaultShifts { get { return new List<ShiftDTO> { SundayShift!, MondayShift!, TuesdayShift!, WednesdayShift!, ThursdayShift!, FridayShift!, SaturdayShift! }; } }
    public IEnumerable<ScheduleDTO> DefaultSchedules { get { return new List<ScheduleDTO> { SundaySchedule!, MondaySchedule!, TuesdaySchedule!, WednesdaySchedule!, ThursdaySchedule!, FridaySchedule!, SaturdaySchedule! }; } }

    public EmployeeFormDTO(Guid id, string? firstName, string? lastName, string? middleName, DateTime? dateOfBirth, Gender gender, CivilStatus civilStatus, BloodType bloodType, Status status, decimal? height, decimal? weight, byte[]? imageUrl, string? remarks, 
        string? email, string? telNumber, string? mobileNumber, string? address, string? provincialAddress, string? mothersMaidenName, string? fathersName, string? emergencyContact, string? relationshipContact, string? addressContact, string? telNoContact, string? mobileNoContact, 
        EducationalAttainment educationalAttainment, string? courseGraduated, string? universityGraduated, 
        DateTime? dateHired, DateTime? dateRegularized, DateTime? dateResigned, DateTime? dateTerminated, string? tinNo, string? sssNo, string? hdmfNo, string? phicNo, string? bankAccount, bool hasServiceCharge,
        Guid? payId, decimal? basicPay, decimal? dailyRate, decimal? hourlyRate, decimal? hdmf_con, decimal? hdmf_er, 
        Guid? userId, string? username, string? password, Credential credential, Guid? supervisedGroup, bool isGlobalSupervisor,
        Guid positionId, Guid sectionId, Guid departmentId, Guid divisionId, Guid typeId, Guid levelId, Guid chargingId, int mealCredits, Tenant tenant,
        ScheduleDTO? sunday = null, ScheduleDTO? monday = null, ScheduleDTO? tuesday = null, ScheduleDTO? wednesday = null, ScheduleDTO? thursday = null, ScheduleDTO? friday = null, ScheduleDTO? saturday = null) : this()
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
        PayId = payId;
        BasicPay = basicPay;
        DailyRate = dailyRate;
        HourlyRate = hourlyRate;        
        HDMF_Con = hdmf_con;
        HDMF_Er = hdmf_er;
        UserId = userId;
        Username = username;
        Password = password;
        Credential = credential;
        SupervisedGroup = supervisedGroup;
        IsGlobalSupervisor = isGlobalSupervisor;
        PositionId = positionId;
        SectionId = sectionId;
        DepartmentId = departmentId;
        DivisionId = divisionId;
        TypeId = typeId;
        LevelId = levelId;
        ChargingId = chargingId;
        SundaySchedule = sunday;
        MondaySchedule = monday;
        TuesdaySchedule = tuesday;
        WednesdaySchedule = wednesday;
        ThursdaySchedule = thursday;
        FridaySchedule = friday;
        SaturdaySchedule = saturday;
        MealCredits = mealCredits;
        Tenant = tenant;
    }
}
