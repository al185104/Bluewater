using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;

namespace Bluewater.Core.PayAggregate;
public class Pay(decimal basicPay, decimal dailyRate, decimal hourlyRate, decimal hdmfCon = 200, decimal hdmfEf = 200) : EntityBase<Guid>, IAggregateRoot
{
  public decimal BasicPay { get; private set; } = basicPay;
  public decimal DailyRate { get; private set; } = dailyRate;
  public decimal HourlyRate { get; private set; } = hourlyRate;
  public decimal HDMF_Con { get; private set; } = hdmfCon;
  public decimal HDMF_Er { get; private set; } = hdmfEf;
  public decimal Cola { get; private set; } = 0;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;
  
  // Navigation Properties
  public virtual ICollection<Employee>? Employees { get; private set; }

  public Pay() : this(0, 0, 0, 200, 200) { }

  public void UpdatePay(decimal basicPay, decimal dailyRate, decimal hourlyRate, decimal hdmfCon, decimal hdmfEr, decimal cola){
    BasicPay = basicPay;
    DailyRate = dailyRate;
    HourlyRate = hourlyRate;
    HDMF_Con = hdmfCon;
    HDMF_Er = hdmfEr;
    Cola = cola;

    UpdatedDate = DateTime.Now;
  }
}
