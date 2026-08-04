using System.Globalization;
using System.Text;

namespace Bluewater.App.Helpers;

public static class ScheduleMatrixCsvImporter
{
	public static async Task<IReadOnlyList<ScheduleMatrixCsvRow>> ParseAsync(
		Stream stream,
		DateOnly weekStart,
		CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(stream);

		using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, leaveOpen: true);
		string? headerLine = await reader.ReadLineAsync().ConfigureAwait(false);

		if (string.IsNullOrWhiteSpace(headerLine))
		{
			return Array.Empty<ScheduleMatrixCsvRow>();
		}

		string[] headers = SplitCsvLine(headerLine);
		int firstDayColumnIndex = ValidateHeaders(headers, weekStart);

		DateOnly[] columnDates = ResolveColumnDates(headers, weekStart, firstDayColumnIndex);
		var rows = new List<ScheduleMatrixCsvRow>();

		string? line;
		int lineNumber = 1;
		while ((line = await reader.ReadLineAsync().ConfigureAwait(false)) is not null)
		{
			cancellationToken.ThrowIfCancellationRequested();
			lineNumber++;

			if (string.IsNullOrWhiteSpace(line))
			{
				continue;
			}

			string[] values = SplitCsvLine(line);
			string barcode = GetValue(values, 0);
			if (string.IsNullOrWhiteSpace(barcode))
			{
				continue;
			}

			var shiftsByDate = new Dictionary<DateOnly, string>();
			for (int i = 0; i < columnDates.Length; i++)
			{
				shiftsByDate[columnDates[i]] = GetValue(values, firstDayColumnIndex + i);
			}

			rows.Add(new ScheduleMatrixCsvRow(lineNumber, barcode, shiftsByDate));
		}

		return rows;
	}

	private static int ValidateHeaders(string[] headers, DateOnly weekStart)
	{
		if (headers.Length < 3)
		{
			throw new FormatException("The CSV file must include Barcode, Employee, and at least one day column.");
		}

		string[] required = ["Barcode", "Employee"];
		for (int i = 0; i < required.Length; i++)
		{
			if (!string.Equals(headers[i], required[i], StringComparison.OrdinalIgnoreCase))
			{
				throw new FormatException($"Expected column {i + 1} to be '{required[i]}'.");
			}
		}

		int firstDayColumnIndex = FindFirstDayColumnIndex(headers, weekStart);
		if (firstDayColumnIndex < 0)
		{
			throw new FormatException("The CSV file must include at least one day column after Employee.");
		}

		return firstDayColumnIndex;
	}

	private static int FindFirstDayColumnIndex(string[] headers, DateOnly weekStart)
	{
		for (int i = 2; i < headers.Length; i++)
		{
			if (TryParseHeaderDate(headers[i], weekStart) is not null)
			{
				return i;
			}
		}

		return -1;
	}

	private static DateOnly[] ResolveColumnDates(string[] headers, DateOnly weekStart, int firstDayColumnIndex)
	{
		int dayColumnCount = headers.Length - firstDayColumnIndex;
		var dates = new DateOnly[dayColumnCount];
		for (int i = 0; i < dayColumnCount; i++)
		{
			string header = headers[firstDayColumnIndex + i];
			dates[i] = TryParseHeaderDate(header, weekStart) ?? weekStart.AddDays(i);
		}

		return dates;
	}

	private static DateOnly? TryParseHeaderDate(string value, DateOnly weekStart)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return null;
		}

		string trimmed = value.Trim();
		string[] fullDateFormats =
		[
			"yyyy-MM-dd",
			"M/d/yyyy",
			"MM/dd/yyyy",
			"M-d-yyyy",
			"MM-dd-yyyy",
			"MMM d yyyy",
			"MMM d, yyyy",
			"MMMM d yyyy",
			"MMMM d, yyyy",
			"ddd MMM d yyyy",
			"ddd MMM d, yyyy",
			"ddd MMM dd yyyy",
			"ddd MMM dd, yyyy"
		];

		foreach (var format in fullDateFormats)
		{
			if (DateTime.TryParseExact(trimmed, format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out var parsed) ||
				DateTime.TryParseExact(trimmed, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsed))
			{
				return DateOnly.FromDateTime(parsed);
			}
		}

		string[] weekRelativeFormats = ["ddd MMM d", "ddd MMM dd", "MMMM d", "MMM d"];
		foreach (var format in weekRelativeFormats)
		{
			if (DateTime.TryParseExact(trimmed, format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out var parsed) ||
				DateTime.TryParseExact(trimmed, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsed))
			{
				int year = weekStart.Year;
				var date = new DateOnly(year, parsed.Month, parsed.Day);
				if (date < weekStart.AddDays(-7)) date = date.AddYears(1);
				if (date > weekStart.AddDays(13)) date = date.AddYears(-1);
				return date;
			}
		}

		return null;
	}

	private static string GetValue(string[] values, int index)
	{
		return index >= 0 && index < values.Length ? values[index].Trim() : string.Empty;
	}

	private static string[] SplitCsvLine(string line)
	{
		var values = new List<string>();
		var current = new StringBuilder();
		bool inQuotes = false;

		for (int i = 0; i < line.Length; i++)
		{
			char c = line[i];

			if (c == '"')
			{
				if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
				{
					current.Append('"');
					i++;
				}
				else
				{
					inQuotes = !inQuotes;
				}
			}
			else if (c == ',' && !inQuotes)
			{
				values.Add(current.ToString());
				current.Clear();
			}
			else
			{
				current.Append(c);
			}
		}

		values.Add(current.ToString());
		return values.ToArray();
	}
}

public sealed record class ScheduleMatrixCsvRow(int LineNumber, string Barcode, IReadOnlyDictionary<DateOnly, string> ShiftsByDate);
