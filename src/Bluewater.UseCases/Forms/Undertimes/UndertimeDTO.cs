using Bluewater.Core.Forms.Enum;
namespace Bluewater.UseCases.Forms.Undertimes;
public record UndertimeDTO(Guid Id, Guid empId, decimal inclusiveTime, string reason, DateOnly date, ApplicationStatus status);