namespace Bluewater.UseCases.Attendances;
// the properties are named this way to handle the current export coming from maribago
public record AttendanceImportDTO()
{
    public AttendanceImportDTO(string emp_ID, string emp_FirstName, string emp_LastName, string divisionCode,
                             string dateSched, string shiftCode, string aTimeIn1D, string aTimeIn1, string aTimeOut1D,
                             string aTimeOut1, string aTimeIn2D, string aTimeIn2, string aTimeOut2D, string aTimeOut2,
                             string empTypeDesc) : this()
    {
        Emp_ID = emp_ID;
        Emp_FirstName = emp_FirstName;
        Emp_LastName = emp_LastName;
        DivisionCode = divisionCode;
        DateSched = dateSched;
        ShiftCode = shiftCode;
        ATimeIn1D = aTimeIn1D;
        ATimeIn1 = aTimeIn1;
        ATimeOut1D = aTimeOut1D;
        ATimeOut1 = aTimeOut1;
        ATimeIn2D = aTimeIn2D;
        ATimeIn2 = aTimeIn2;
        ATimeOut2D = aTimeOut2D;
        ATimeOut2 = aTimeOut2;
        EmpTypeDesc = empTypeDesc;
    }

    public string Emp_ID { get; init; } = string.Empty;
    public string Emp_FirstName { get; init; } = string.Empty;
    public string Emp_LastName { get; init; } = string.Empty;
    public string DivisionCode { get; init; } = string.Empty;
    public string DateSched { get; init; } = string.Empty;
    public string ShiftCode { get; init; } = string.Empty;
    public string ATimeIn1D { get; init; } = string.Empty;
    public string ATimeIn1 { get; init; } = string.Empty;
    public string ATimeOut1D { get; init; } = string.Empty;
    public string ATimeOut1 { get; init; } = string.Empty;
    public string ATimeIn2D { get; init; } = string.Empty;
    public string ATimeIn2 { get; init; } = string.Empty;
    public string ATimeOut2D { get; init; } = string.Empty;
    public string ATimeOut2 { get; init; } = string.Empty;
    public string EmpTypeDesc { get; init; } = string.Empty;
}
