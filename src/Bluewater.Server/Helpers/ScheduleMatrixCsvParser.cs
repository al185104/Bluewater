using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Bluewater.UseCases.Schedules;
using CsvHelper;
using CsvHelper.Configuration;

namespace Bluewater.Server.Helpers;

internal static class ScheduleMatrixCsvParser
{
    private static readonly string[] DateFormats =
    [
        "yyyy-MM-dd",
        "MM/dd/yyyy",
        "M/d/yyyy",
        "dd/MM/yyyy",
        "d/M/yyyy",
        "MM/dd/yy",
        "M/d/yy",
        "dd/MM/yy",
        "d/M/yy"
    ];

    private static readonly HashSet<string> SkippedShiftCodes = new(StringComparer.InvariantCultureIgnoreCase)
    {
        string.Empty,
        "R",
        "REST",
        "RESTDAY",
        "REST DAY",
        "RD"
    };

    public static async Task<(bool IsMatch, List<ScheduleImportDTO>? Records, string Message)> TryImportAsync(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null,
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        if (!await csv.ReadAsync().ConfigureAwait(false))
        {
            return (true, new List<ScheduleImportDTO>(), "The schedule CSV file is empty.");
        }

        csv.ReadHeader();
        //await csv.ReadHeaderAsync().ConfigureAwait(false);
        string[]? headers = csv.HeaderRecord;

        if (headers is null)
        {
            return (true, null, "CSV file does not contain headers.");
        }

        if (headers.Length < 3 || !IsNameHeader(headers[0]) || !IsIdHeader(headers[1]))
        {
            return (false, null, string.Empty);
        }

        if (!TryParseDateHeaders(headers.AsSpan(2), out List<(string Header, DateOnly Date)> dateHeaders, out string? error))
        {
            return (true, null, error ?? "Unable to parse date headers.");
        }

        var records = new List<ScheduleImportDTO>();

        while (await csv.ReadAsync().ConfigureAwait(false))
        {
            string employeeId = csv.GetField(headers[1])?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(employeeId))
            {
                continue;
            }

            foreach ((string header, DateOnly date) in dateHeaders)
            {
                string shiftValue = csv.GetField(header)?.Trim() ?? string.Empty;

                if (ShouldSkipShift(shiftValue))
                {
                    continue;
                }

                string formattedDate = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                records.Add(new ScheduleImportDTO
                {
                    Id = employeeId,
                    Username = employeeId,
                    ScheduleDate = formattedDate,
                    Date = formattedDate,
                    Shift = shiftValue,
                    ShiftCode = shiftValue,
                });
            }
        }

        return (true, records, string.Empty);
    }

    private static bool TryParseDateHeaders(ReadOnlySpan<string> headers, out List<(string Header, DateOnly Date)> dateHeaders, out string? error)
    {
        dateHeaders = new List<(string Header, DateOnly Date)>(headers.Length);
        error = null;

        for (int i = 0; i < headers.Length; i++)
        {
            string header = headers[i];

            if (string.IsNullOrWhiteSpace(header))
            {
                continue;
            }

            if (!TryParseDate(header, out DateOnly date))
            {
                error = $"Unable to parse date header '{header}'.";
                return false;
            }

            dateHeaders.Add((header, date));
        }

        if (dateHeaders.Count == 0)
        {
            error = "No schedule date columns were found after the NAME and ID columns.";
            return false;
        }

        return true;
    }

    private static bool TryParseDate(string value, out DateOnly date)
    {
        foreach (string format in DateFormats)
        {
            if (DateOnly.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return true;
            }
        }

        return DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
    }

    private static bool ShouldSkipShift(string value)
    {
        return SkippedShiftCodes.Contains(value?.Trim() ?? string.Empty);
    }

    private static bool IsNameHeader(string header)
    {
        return string.Equals(header, "Name", StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsIdHeader(string header)
    {
        return string.Equals(header, "Id", StringComparison.InvariantCultureIgnoreCase) ||
               string.Equals(header, "EmployeeId", StringComparison.InvariantCultureIgnoreCase) ||
               string.Equals(header, "Employee Id", StringComparison.InvariantCultureIgnoreCase);
    }
}
