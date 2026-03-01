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
		ValidateHeaders(headers);

		DateOnly[] columnDates = ResolveColumnDates(headers, weekStart);
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
				shiftsByDate[columnDates[i]] = GetValue(values, 4 + i);
			}

			rows.Add(new ScheduleMatrixCsvRow(lineNumber, barcode, shiftsByDate));
		}

		return rows;
	}

	private static void ValidateHeaders(string[] headers)
	{
		if (headers.Length < 11)
		{
			throw new FormatException("The CSV file must include Barcode, Employee, Section, Charging, and 7 day columns.");
		}

		string[] required = ["Barcode", "Employee", "Section", "Charging"];
		for (int i = 0; i < required.Length; i++)
		{
			if (!string.Equals(headers[i], required[i], StringComparison.OrdinalIgnoreCase))
			{
				throw new FormatException($"Expected column {i + 1} to be '{required[i]}'.");
			}
		}
	}

	private static DateOnly[] ResolveColumnDates(string[] headers, DateOnly weekStart)
	{
		var dates = new DateOnly[7];
		for (int i = 0; i < 7; i++)
		{
			string header = i + 4 < headers.Length ? headers[i + 4] : string.Empty;
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

		string[] formats = ["ddd MMM d", "ddd MMM dd", "MMMM d", "MMM d"];
		foreach (var format in formats)
		{
			if (DateTime.TryParseExact(value.Trim(), format, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out var parsed) ||
				DateTime.TryParseExact(value.Trim(), format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out parsed))
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
