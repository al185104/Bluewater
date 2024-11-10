namespace Bluewater.UseCases.ServiceCharges;
public record ServiceChargeDTO()
{
    public Guid Id { get; init; }
    public string Username { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }
    public ServiceChargeDTO(Guid id, string username, decimal amount, DateOnly date) : this()
    {
        Id = id;
        Username = username;
        Amount = amount;
        Date = date;
    }
}
