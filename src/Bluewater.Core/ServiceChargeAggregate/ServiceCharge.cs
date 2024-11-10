using Ardalis.SharedKernel;
using Bluewater.Core.EmployeeAggregate;

namespace Bluewater.Core.ServiceChargeAggregate;
public class ServiceCharge(string username, decimal amount, DateOnly date) : EntityBase<Guid>, IAggregateRoot
{
    public string Username { get; private set; } = username;
    public decimal Amount { get; private set; } = amount;
    public DateOnly Date { get; private set; } = date;

    public DateTime CreatedDate { get; private set; } = DateTime.Now;
    public Guid CreateBy { get; private set; } = Guid.Empty;
    public DateTime UpdatedDate { get; private set; } = DateTime.Now;
    public Guid UpdateBy { get; private set; } = Guid.Empty;    

    public virtual ICollection<Employee>? Employees { get; private set; }

    public ServiceCharge() : this(string.Empty, 0, DateOnly.MinValue) { }

    public void UpdateServiceCharge(string username, decimal amount, DateOnly date)
    {
        Username = username;
        Amount = amount;
        Date = date;

        UpdatedDate = DateTime.Now;
    }
}
