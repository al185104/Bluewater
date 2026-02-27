using System;
using System.Globalization;

namespace Bluewater.App.Models;

public class ShiftSummary : IRowIndexed
{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string? ShiftStartTime { get; set; }
		public string? ShiftBreakTime { get; set; }
		public string? ShiftBreakEndTime { get; set; }
		public string? ShiftEndTime { get; set; }
		public decimal BreakHours { get; set; }

		public string BreakHoursDisplay => BreakHours.ToString("0.##");

		public decimal BreakMinutesDisplay => BreakHours * 60;


		public int RowIndex { get; set; }

		public TimeSpan ShiftStartTimeValue
		{
				get => ParseTimeSpan(ShiftStartTime);
				set => ShiftStartTime = FormatTimeSpan(value);
		}

		public TimeSpan ShiftBreakTimeValue
		{
				get => ParseTimeSpan(ShiftBreakTime);
				set => ShiftBreakTime = FormatTimeSpan(value);
		}

		public TimeSpan ShiftBreakEndTimeValue
		{
				get => ParseTimeSpan(ShiftBreakEndTime);
				set => ShiftBreakEndTime = FormatTimeSpan(value);
		}

		public TimeSpan ShiftEndTimeValue
		{
				get => ParseTimeSpan(ShiftEndTime);
				set => ShiftEndTime = FormatTimeSpan(value);
		}

		private static TimeSpan ParseTimeSpan(string? value)
		{
				if (string.IsNullOrWhiteSpace(value))
				{
						return TimeSpan.Zero;
				}

				if (TimeSpan.TryParseExact(value, "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan result))
				{
						return result;
				}

				if (TimeOnly.TryParse(value, out TimeOnly timeOnly))
				{
						return timeOnly.ToTimeSpan();
				}

				return TimeSpan.Zero;
		}

		private static string FormatTimeSpan(TimeSpan value)
		{
				return value.ToString("hh\\:mm", CultureInfo.InvariantCulture);
		}
}
