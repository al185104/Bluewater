namespace Bluewater.UseCases.Pays;

public record PayDTO()
{
    public Guid Id { get; init; }
    public decimal? BasicPay { get; set; } 
    public decimal? DailyRate { get; set; } 
    public decimal? HourlyRate { get; set; } 
    public decimal? HDMF_Con { get; set; } 
    public decimal? HDMF_Er { get; set; } 

    public PayDTO(Guid id, decimal? basicPay, decimal? dailyRate, decimal? hourlyRate, decimal? hdmfCon, decimal? hdmfEr) : this()
    {
        Id = id;
        BasicPay = basicPay;
        DailyRate = dailyRate;
        HourlyRate = hourlyRate;
        HDMF_Con = hdmfCon;
        HDMF_Er = hdmfEr;
    }
}