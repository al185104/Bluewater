using System.ComponentModel;

namespace Bluewater.UserCases.Forms.Enum;
public enum ApplicationStatusDTO
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