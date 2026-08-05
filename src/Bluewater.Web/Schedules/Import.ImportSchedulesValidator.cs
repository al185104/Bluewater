using FastEndpoints;
using FluentValidation;

namespace Bluewater.Web.Schedules;

public class ImportSchedulesValidator : Validator<ImportSchedulesRequest>
{
  public ImportSchedulesValidator()
  {
    RuleFor(request => request.Entries)
      .NotNull()
      .Must(entries => entries.Count > 0)
      .WithMessage("At least one schedule entry is required.");

    RuleForEach(request => request.Entries).ChildRules(entry =>
    {
      entry.RuleFor(item => item.Barcode)
        .NotEmpty()
        .WithMessage("Employee barcode is required.");

      entry.RuleFor(item => item.ScheduleDate)
        .NotEmpty()
        .WithMessage("Schedule date is required.");
    });
  }
}
