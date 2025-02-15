
namespace Bluewater.UseCases.LeaveCredits;

public record LeaveCreditDTO
{
    public Guid Id { get; set; }
    public string Code { get; private set; }
    public string Description { get; private set; }
    public decimal DefaultCredits { get; private set; }

    public LeaveCreditDTO(Guid id, string leaveCode, string leaveDescription, decimal defaultCredits)
    {
        Id = id;
        Code = leaveCode;
        Description = leaveDescription;
        DefaultCredits = defaultCredits;
    }
}
