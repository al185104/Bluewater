using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.PayrollAggregate;
using Bluewater.Core.PayrollAggregate.Specifications;

namespace Bluewater.UseCases.Payrolls.List;

internal class ListGroupedPayrollHandler(IRepository<Payroll> _repository) : IQueryHandler<ListGroupedPayrollQuery, Result<IEnumerable<PayrollSummaryDTO>>>
{
  public async Task<Result<IEnumerable<PayrollSummaryDTO>>> Handle(ListGroupedPayrollQuery request, CancellationToken cancellationToken)
  {
    var spec = new PayrollAllSpec();
    var ret = await _repository.ListAsync(cancellationToken);
    // var result = (await _repository.ListAsync(cancellationToken)).Select(s => new PayrollDTO(s.Id, s.Name, s.Description, s.Date, s.IsRegular));
    // return Result.Success(result);
    // Group by generation date and get the count for each date
    var summary = ret
        .GroupBy(payroll => payroll.Date) // Adjust GeneratedDate to your actual date field
        .Select(group => new PayrollSummaryDTO
        {
            Date = group.Key,
            Count = group.Count(),
            TotalServiceCharge = group.Sum(p => p.SvcCharge),
            TotalAbsences = group.Sum(p => p.Absences),
            TotalLeaves = group.Sum(p => p.Leaves),
            TotalLates = group.Sum(p => p.Lates),
            TotalUndertimes = group.Sum(p => p.Undertime),
            TotalOverbreak = group.Sum(p => p.Overbreak),
            TotalTaxDeductions = group.Sum(p => p.TaxDeductions),
            TotalNetAmount = group.Sum(p => p.NetAmount)
        })
        .ToList();

    return summary;
  }
}
