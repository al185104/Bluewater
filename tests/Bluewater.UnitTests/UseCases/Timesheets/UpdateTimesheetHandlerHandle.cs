using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.UseCases.Timesheets.Update;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Bluewater.UnitTests.UseCases.Timesheets;

public class UpdateTimesheetHandlerHandle
{
  private readonly IRepository<Timesheet> _repository = Substitute.For<IRepository<Timesheet>>();
  private readonly UpdateTimesheetHandler _handler;

  public UpdateTimesheetHandlerHandle()
  {
    _handler = new UpdateTimesheetHandler(_repository);
  }

  [Fact]
  public async Task ReturnsNotFoundWhenIdIsProvidedButTimesheetDoesNotExist()
  {
    UpdateTimesheetCommand command = new(
      Guid.NewGuid(),
      Guid.NewGuid(),
      DateTime.UtcNow,
      null,
      null,
      null,
      DateOnly.FromDateTime(DateTime.UtcNow),
      false);

    _repository.GetByIdAsync(command.TimesheetId, Arg.Any<CancellationToken>())
      .Returns((Timesheet?)null);

    Result<TimesheetDTO> result = await _handler.Handle(command, CancellationToken.None);

    result.Status.Should().Be(ResultStatus.NotFound);
    await _repository.DidNotReceive().AddAsync(Arg.Any<Timesheet>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task CreatesTimesheetWhenIdIsEmptyAndNoExistingTimesheetForEntryDate()
  {
    Guid employeeId = Guid.NewGuid();
    DateOnly entryDate = DateOnly.FromDateTime(DateTime.UtcNow);

    UpdateTimesheetCommand command = new(
      Guid.Empty,
      employeeId,
      DateTime.UtcNow,
      DateTime.UtcNow.AddHours(8),
      null,
      null,
      entryDate,
      false);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Timesheet>>(), Arg.Any<CancellationToken>())
      .Returns((Timesheet?)null);

    Result<TimesheetDTO> result = await _handler.Handle(command, CancellationToken.None);

    result.IsSuccess.Should().BeTrue();
    result.Value.EmployeeId.Should().Be(employeeId);
    result.Value.EntryDate.Should().Be(entryDate);
    await _repository.Received(1).AddAsync(Arg.Any<Timesheet>(), Arg.Any<CancellationToken>());
    await _repository.DidNotReceive().UpdateAsync(Arg.Any<Timesheet>(), Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task UpdatesExistingTimesheetWhenIdIsEmptyButEntryAlreadyExists()
  {
    Guid employeeId = Guid.NewGuid();
    DateOnly entryDate = DateOnly.FromDateTime(DateTime.UtcNow);
    Timesheet existing = new(employeeId, null, null, null, null, entryDate);

    UpdateTimesheetCommand command = new(
      Guid.Empty,
      employeeId,
      DateTime.UtcNow,
      DateTime.UtcNow.AddHours(8),
      null,
      null,
      entryDate,
      true);

    _repository.FirstOrDefaultAsync(Arg.Any<ISpecification<Timesheet>>(), Arg.Any<CancellationToken>())
      .Returns(existing);

    Result<TimesheetDTO> result = await _handler.Handle(command, CancellationToken.None);

    result.IsSuccess.Should().BeTrue();
    result.Value.EmployeeId.Should().Be(employeeId);
    result.Value.EntryDate.Should().Be(entryDate);
    await _repository.Received(1).UpdateAsync(existing, Arg.Any<CancellationToken>());
    await _repository.DidNotReceive().AddAsync(Arg.Any<Timesheet>(), Arg.Any<CancellationToken>());
  }
}
