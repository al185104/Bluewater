namespace Bluewater.Core.PayrollAggregate.Helpers;
public static class CompensationLookup
{
    private static readonly List<(decimal min, decimal max)> ParsedRanges = new List<(decimal min, decimal max)>();
    private static readonly List<decimal> ERValues = new List<decimal>
    {
        390.00m, 437.50m, 485.00m, 532.50m, 580.00m, 627.50m, 675.00m, 722.50m, 770.00m, 817.50m,
        865.00m, 912.50m, 960.00m, 1007.50m, 1055.00m, 1102.50m, 1150.00m, 1197.50m, 1245.00m, 1292.50m,
        1340.00m, 1387.50m, 1455.00m, 1502.50m, 1550.00m, 1597.50m, 1645.00m, 1692.50m, 1720.00m, 1787.50m,
        1835.00m, 1882.50m, 1930.00m, 1977.50m, 2025.00m, 2072.50m, 2120.00m, 2167.50m, 2215.00m, 2262.50m,
        2310.00m, 2257.50m, 2405.00m, 2452.50m, 2500.00m, 2547.50m, 2595.00m, 2642.50m, 2690.00m, 2737.50m,
        2785.00m, 2832.50m, 2880.00m
    };

    private static readonly List<decimal> EEValues = new List<decimal>
    {
        180.00m, 202.50m, 225.00m, 247.50m, 270.00m, 292.50m, 315.00m, 337.50m, 360.00m, 382.50m,
        405.00m, 427.50m, 450.00m, 472.50m, 495.00m, 517.50m, 540.00m, 562.50m, 585.00m, 607.50m,
        630.00m, 652.50m, 675.00m, 697.50m, 720.00m, 742.50m, 765.00m, 787.50m, 810.00m, 832.50m,
        855.00m, 877.50m, 900.00m, 922.50m, 945.00m, 967.50m, 990.00m, 1012.50m, 1035.00m, 1057.50m,
        1080.00m, 1102.50m, 1125.00m, 1147.00m, 1170.00m, 1192.50m, 1215.00m, 1237.50m, 1260.00m, 1282.50m,
        1305.00m, 1327.50m, 1350.00m
    };

    static CompensationLookup()
    {
        // Parse ranges once and store them in ParsedRanges
        List<string> compensationRanges = new List<string>
        {
            "Below 4,250", "4,250 – 4,749.99", "4,750 – 5,249.99", "5,250 – 5,749.99", "5,750 – 6,249.99",
            "6,250 – 6,749.99", "6,750 – 7,249.99", "7,250 – 7,749.99", "7,750 – 8,249.99", "8,250 – 8,749.99",
            "8,750 – 9,249.99", "9,250 – 9,749.99", "9,750 – 10,249.99", "10,250 – 10,749.99", "10,750 – 11,249.99",
            "11,250 – 11,749.99", "11,750 – 12,249.99", "12,250 – 12,749.99", "12,750 – 13,249.99", "13,250 – 13,749.99",
            "13,750 – 14,249.99", "14,250 – 14,749.99", "14,750 – 15,249.99", "15,250 – 15,749.99", "15,750 – 16,249.99",
            "16,250 – 16,749.99", "16,750 – 17,249.99", "17,250 – 17,749.99", "17,750 – 18,249.99", "18,250 – 18,749.99",
            "18,750 – 19,249.99", "19,250 – 19,749.99", "19,750 – 20,249.99", "20,250 – 20,749.99", "20,750 – 21,249.99",
            "21,250 – 21,749.99", "21,750 – 22,249.99", "22,250 – 22,749.99", "22,750 – 23,249.99", "23,250 – 23,749.99",
            "23,750 – 24,249.99", "24,250 – 24,749.99", "24,750 – 25,249.99", "25,250 – 25,749.99", "25,750 – 26,249.99",
            "26,250 – 26,749.99", "26,750 – 27,249.99", "27,250 – 27,749.99", "27,750 – 28,249.99", "28,250 – 28,749.99",
            "28,750 – 29,249.99", "29,250 – 29,749.99", "Above 29,750"
        };

        foreach (var range in compensationRanges)
        {
            if (range.StartsWith("Below"))
            {
                ParsedRanges.Add((decimal.MinValue, decimal.Parse(range.Split(' ')[1])));
            }
            else if (range.StartsWith("Above"))
            {
                ParsedRanges.Add((decimal.Parse(range.Split(' ')[1]), decimal.MaxValue));
            }
            else
            {
                var parts = range.Split('–');
                var min = decimal.Parse(parts[0].Trim().Replace(",", ""));
                var max = decimal.Parse(parts[1].Trim().Replace(",", ""));
                ParsedRanges.Add((min, max));
            }
        }
    }

    public static (decimal ER, decimal EE) FindValuesByCompensation(decimal compensation)
    {
        int index = ParsedRanges.FindIndex(range => compensation >= range.min && compensation < range.max);

        if (index == -1)
        {
            return (0m, 0m); // Not found
        }

        return (ERValues[index], EEValues[index]);
    }
}