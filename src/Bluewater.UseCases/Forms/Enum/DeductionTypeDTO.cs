using System.ComponentModel;

namespace Bluewater.UserCases.Forms.Enum;
public enum DeductionsTypeDTO
{
    [Description("Not Set")]
    NotSet,
    [Description("SSS Salary Loan")]
    SSS_SL,
    [Description("SSS Calamity Loan")]
    SSS_CL,        
    [Description("HDMF Multi-Purpose Loan")]
    HDMF_MPL,
    [Description("HDMF Calamity Loan")]
    HDMF_CL,
    [Description("RCBC Loan")]
    RCBC,
    [Description("Hospitalization")]
    HOSP,
    [Description("HDMF MP2 Contribution")]
    MP2,
    [Description("Cash Advance")]
    CASH_ADV,
    [Description("Union Dues")]
    UNDUE,
    [Description("Salary Overpayment")]
    SALOV,
    [Description("Eyeglasses")]
    EYE,
    [Description("Overtime Overpayment")]
    OTOV,
    [Description("Others")]
    Others           
}