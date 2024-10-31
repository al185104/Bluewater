using System.ComponentModel;

namespace Bluewater.Core.Forms.Enum;
public enum OtherEarningType
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
