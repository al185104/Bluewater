using System.ComponentModel;

namespace Bluewater.UserCases.Forms.Enum;
public enum OtherEarningTypeDTO
{
    [Description("Not Set")]
    NotSet,
    [Description("Monthly Allowance")]
    MONAL,
    [Description("Salary Underpayment")]
    SALUN,        
    [Description("Refund Absences")]
    REFABS,
    [Description("Refund Undertime")]
    REFUT,
    [Description("Refund Overtime")]
    REFOT
}
