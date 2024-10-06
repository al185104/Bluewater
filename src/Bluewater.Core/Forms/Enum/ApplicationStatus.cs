using System.ComponentModel;

namespace Bluewater.Core.Forms.Enum;
public enum ApplicationStatus
{
  [Description("Pending")]
  Pending,
  [Description("Approve")]
  Approved,
  [Description("Reject")]
  Rejected
}
