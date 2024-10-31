using System.ComponentModel;

namespace Bluewater.Core.Forms.Enum;
public enum FailureInOutReason
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
