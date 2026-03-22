using Ardalis.Result;
using Ardalis.SharedKernel;
using Bluewater.Core.AttendanceAggregate;
using Bluewater.Core.AttendanceAggregate.Specifications;
using Bluewater.Core.Forms.LeaveAggregate;
using Bluewater.Core.ShiftAggregate;
using Bluewater.Core.TimesheetAggregate;
using Bluewater.UserCases.Forms.Enum;
using Bluewater.UseCases.Employees;
using Bluewater.UseCases.Schedules;
using Bluewater.UseCases.Schedules.Get;
using Bluewater.UseCases.Shifts;
using Bluewater.UseCases.Timesheets;
using Bluewater.UseCases.Timesheets.Get;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ApplicationStatus = Bluewater.Core.Forms.Enum.ApplicationStatus;

namespace Bluewater.UseCases.Attendances.List;

internal class ListAttendanceHandler(IRepository<Attendance> _repository, IRepository<Leave> _leaveRepository, IServiceScopeFactory serviceScopeFactory) : IQueryHandler<ListAttendanceQuery, Result<IEnumerable<AttendanceDTO>>>
{
    public async Task<Result<IEnumerable<AttendanceDTO>>> Handle(ListAttendanceQuery request, CancellationToken cancellationToken)
    {
        var spec = new AttendanceByEmpIdAndDatesSpec(request.empId, request.startDate, request.endDate);
        var attendances = await _repository.ListAsync(spec, cancellationToken);
        if (attendances == null) return Result<IEnumerable<AttendanceDTO>>.NotFound();

        // get employee by employee id
        EmployeeDTO emp = default!;
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var ret = await mediator.Send(new GetEmployeeQuery(request.empId));
            if (ret.IsSuccess)
                emp = ret.Value;
        }

        Dictionary<Guid, ApplicationStatusDTO> leaveStatuses = new();
        IEnumerable<Guid> leaveIds = attendances
          .Where(attendance => attendance.LeaveId.HasValue && attendance.LeaveId != Guid.Empty)
          .Select(attendance => attendance.LeaveId!.Value)
          .Distinct();

        foreach (Guid leaveId in leaveIds)
        {
            Leave? leave = await _leaveRepository.GetByIdAsync(leaveId, cancellationToken);
            if (leave is not null)
            {
                leaveStatuses[leaveId] = (ApplicationStatusDTO)leave.Status;
            }
        }

        DateTime rangeStart = request.startDate.ToDateTime(TimeOnly.MinValue);
        DateTime rangeEnd = request.endDate.ToDateTime(TimeOnly.MaxValue);
        List<Leave> overlappingLeaves = await _leaveRepository.ListAsync(cancellationToken);
        Dictionary<DateOnly, Leave> approvedLeavesByDate = overlappingLeaves
          .Where(leave =>
            leave.EmployeeId == request.empId
            && leave.Status == ApplicationStatus.Approved
            && leave.StartDate.Date <= rangeEnd.Date
            && leave.EndDate.Date >= rangeStart.Date)
          .SelectMany(ExpandLeaveDates)
          .GroupBy(item => item.date)
          .ToDictionary(group => group.Key, group => group.First().leave);

        List<AttendanceDTO> results = new();
        for (var date = request.startDate; date <= request.endDate; date = date.AddDays(1))
        {
            approvedLeavesByDate.TryGetValue(date, out Leave? approvedLeave);
            var attendance = attendances!.FirstOrDefault(s => s.EntryDate == date);
            if (attendance == null)
            {
                TimesheetDTO timesheet = default!;
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var ret = await mediator.Send(new GetTimesheetByEmpIdAndDateQuery(request.empId, date));
                    if (ret.IsSuccess)
                        timesheet = ret.Value;
                }

                // get shift by schedule
                ScheduleDTO schedule = default!;
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var ret = await mediator.Send(new GetScheduleByEmpIdAndDateQuery(request.empId, date));
                    if (ret.IsSuccess)
                        schedule = ret.Value;
                }

                if (schedule == null || schedule.Id == Guid.Empty) // if no schedule found, then try the default schedule.
                {
                    using (var scope = serviceScopeFactory.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var ret = await mediator.Send(new GetDefaultScheduleByEmpIdAndDayQuery(request.empId, date.DayOfWeek));
                        if (ret.IsSuccess)
                            schedule = ret.Value;
                    }
                }

                // new attendance to calculate
                if (approvedLeave is not null && timesheet == null)
                {
                    var _timesheet = ApprovedLeaveWorkHoursCalculator.CreateSyntheticTimesheet(emp.Id, date, schedule?.Shift);
                    timesheet = _timesheet!;
                }

                attendance = new Attendance(emp.Id, schedule?.ShiftId, timesheet?.Id, approvedLeave?.Id, date, null, null, null, null, null, isLocked: false);
                if (schedule != null && schedule.Shift != null)
                    attendance.Shift = new Shift(schedule?.Shift.Name ?? string.Empty, schedule?.Shift.ShiftStartTime, schedule?.Shift.ShiftBreakTime, schedule?.Shift.ShiftBreakEndTime, schedule?.Shift.ShiftEndTime, schedule?.Shift.BreakHours);
                if (timesheet != null)
                    attendance.Timesheet = new Timesheet(Guid.Empty, timesheet.TimeIn1, timesheet.TimeOut1, timesheet.TimeIn2, timesheet.TimeOut2, date);

                //attendance.CalculateWorkHours();
                attendance.CalculateWorkHours();

                results.Add(new AttendanceDTO(Guid.Empty, emp.Id, schedule?.ShiftId, timesheet?.Id, approvedLeave?.Id, date, attendance.WorkHrs, attendance.LateHrs, attendance.UnderHrs, attendance.OverbreakHrs, attendance.NightShiftHours, leaveStatus: approvedLeave is not null ? ApplicationStatusDTO.Approved : null, isLocked: false, schedule?.Shift, timesheet));
            }
            else
            {
                if (approvedLeave is not null
                  && attendance.Shift != null
                  && (!attendance.TimesheetId.HasValue || attendance.TimesheetId == Guid.Empty)
                  && attendance.Timesheet == null)
                {
                    TimesheetDTO? leaveTimesheet = ApprovedLeaveWorkHoursCalculator.CreateSyntheticTimesheet(
                      emp.Id,
                      attendance.EntryDate ?? date,
                      new ShiftDTO(attendance.Shift.Id, attendance.Shift.Name, attendance.Shift.ShiftStartTime, attendance.Shift.ShiftBreakTime, attendance.Shift.ShiftBreakEndTime, attendance.Shift.ShiftEndTime, attendance.Shift.BreakHours));

                    if (leaveTimesheet is not null)
                    {
                        attendance.Timesheet = new Timesheet(Guid.Empty, leaveTimesheet.TimeIn1, leaveTimesheet.TimeOut1, leaveTimesheet.TimeIn2, leaveTimesheet.TimeOut2, attendance.EntryDate ?? date);
                    }
                }

                //attendance.CalculateWorkHours();
                attendance.CalculateWorkHours();

                bool includeApprovedLeaveWithSyntheticHours = approvedLeave is not null
                  && attendance.Timesheet != null
                  && attendance.Shift != null;

                if ((attendance.Timesheet == null || attendance.Timesheet.Id == Guid.Empty ||
                  attendance.Shift == null || attendance.Shift.Id == Guid.Empty)
                  && !includeApprovedLeaveWithSyntheticHours)
                {
                    continue;
                }

                Guid? leaveId = attendance.LeaveId ?? approvedLeave?.Id;
                ApplicationStatusDTO? leaveStatus = null;
                if (leaveId.HasValue)
                {
                    leaveStatus = leaveStatuses.TryGetValue(leaveId.Value, out var status) ? status : null;

                    if (leaveStatus is null && approvedLeave is not null)
                    {
                        leaveStatus = ApplicationStatusDTO.Approved;
                    }
                }

                results.Add(new AttendanceDTO(attendance.Id, emp.Id, attendance.ShiftId, attendance.TimesheetId, leaveId, attendance.EntryDate, attendance.WorkHrs, attendance.LateHrs, attendance.UnderHrs, attendance.OverbreakHrs, attendance.NightShiftHours, leaveStatus, attendance.IsLocked,
                new ShiftDTO(attendance!.Shift!.Id, attendance.Shift.Name, attendance.Shift.ShiftStartTime, attendance.Shift.ShiftBreakTime, attendance.Shift.ShiftBreakEndTime, attendance.Shift.ShiftEndTime, attendance.Shift.BreakHours),
                new TimesheetDTO(attendance.Timesheet?.Id ?? Guid.Empty, emp.Id, attendance.Timesheet?.TimeIn1, attendance.Timesheet?.TimeOut1, attendance.Timesheet?.TimeIn2, attendance.Timesheet?.TimeOut2, attendance.Timesheet?.EntryDate, attendance.Timesheet?.IsEdited ?? false)));
            }
        }

        IEnumerable<AttendanceDTO> orderedResults = results.OrderByDescending(i => i.EntryDate);
        if (request.skip.HasValue)
            orderedResults = orderedResults.Skip(request.skip.Value);
        if (request.take.HasValue)
            orderedResults = orderedResults.Take(request.take.Value);

        return Result.Success(orderedResults.AsEnumerable());
    }

    private static IEnumerable<(DateOnly date, Leave leave)> ExpandLeaveDates(Leave leave)
    {
        DateOnly currentDate = DateOnly.FromDateTime(leave.StartDate);
        DateOnly endDate = DateOnly.FromDateTime(leave.EndDate);

        while (currentDate <= endDate)
        {
            yield return (currentDate, leave);
            currentDate = currentDate.AddDays(1);
        }
    }
}
