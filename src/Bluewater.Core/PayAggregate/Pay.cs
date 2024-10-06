using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;

namespace Bluewater.Core.PayAggregate;
public class Pay(decimal basicPay, decimal dailyRate, decimal hourlyRate) : EntityBase<Guid>, IAggregateRoot
{
  public decimal BasicPay { get; private set; } = basicPay;
  public decimal DailyRate { get; private set; } = dailyRate;
  public decimal HourlyRate { get; private set; } = hourlyRate;
  public decimal HDMF_Con { get; private set; } = 200;
  public decimal HDMF_Er { get; private set; } = 200;

  public DateTime CreatedDate { get; private set; } = DateTime.Now;
  public Guid CreateBy { get; private set; } = Guid.Empty;
  public DateTime UpdatedDate { get; private set; } = DateTime.Now;
  public Guid UpdateBy { get; private set; } = Guid.Empty;
  
  // Navigation Properties
  public virtual ICollection<Employee>? Employees { get; private set; }

  public Pay() : this(0, 0, 0) { }
}
