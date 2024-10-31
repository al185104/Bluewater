using System.ComponentModel;

namespace Bluewater.Core.Forms.Enum;
public enum ApplicationStatus
{
  [Description("Not Set")]
  NotSet,
  [Description("Pending")]
  Pending,
  [Description("Approve")]
  Approved,
  [Description("Reject")]
  Rejected
}
