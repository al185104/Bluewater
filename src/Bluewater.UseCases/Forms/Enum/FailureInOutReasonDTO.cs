using System.ComponentModel;

namespace Bluewater.UserCases.Forms.Enum;
public enum FailureInOutReasonDTO
{
    [Description("Not Set")]
    NotSet,    
    [Description("Breakdown of Biometrics or Barcode Reader")]
    BreakdownOfBiometricsOrBarcodeReader,
    [Description("Defective ID Card")]
    DefectiveIdCard,
    [Description("ID Card Declared Lost")]
    IdCardDeclaredLost,
    [Description("Official Business")]
    OfficialBusiness,     
    [Description("Others")]
    Others
}
