using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Graphics;

namespace Bluewater.App.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
  private static readonly Color[] ChartPalette =
  [
    Color.FromArgb("#2563EB"),
    Color.FromArgb("#10B981"),
    Color.FromArgb("#F97316"),
    Color.FromArgb("#EF4444"),
    Color.FromArgb("#8B5CF6"),
    Color.FromArgb("#F59E0B"),
    Color.FromArgb("#14B8A6"),
    Color.FromArgb("#0EA5E9"),
  ];

  private readonly IDashboardApiService dashboardApiService;
  private bool hasInitialized;

  public HomeViewModel(
    IDashboardApiService dashboardApiService,
    IActivityTraceService activityTraceService,
    IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    this.dashboardApiService = dashboardApiService;
  }

  public ObservableCollection<DashboardChartSegment> AttendanceSummarySegments { get; } = new();

  public ObservableCollection<DashboardTrendPoint> WeeklyAttendanceTrend { get; } = new();

  public ObservableCollection<DashboardChartSegment> LeaveDistributionSegments { get; } = new();

  public ObservableCollection<DashboardTrendPoint> MonthlyAbsenceTrend { get; } = new();

  public ObservableCollection<DashboardLeaderboardEntry> PerfectAttendanceLeaders { get; } = new();

  [ObservableProperty]
  public partial DateTime? DashboardGeneratedAtUtc { get; set; }

  public string LastUpdatedDisplay => DashboardGeneratedAtUtc.HasValue
    ? DashboardGeneratedAtUtc.Value.ToLocalTime().ToString("MMM d, yyyy h:mm tt")
    : "—";

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    await LoadDashboardAsync().ConfigureAwait(false);
  }

  [RelayCommand]
  private async Task RefreshAsync()
  {
    await TraceCommandAsync(nameof(RefreshAsync));
    await LoadDashboardAsync().ConfigureAwait(false);
  }

  private async Task LoadDashboardAsync()
  {
    try
    {
      IsBusy = true;

      HomeDashboardSummary? dashboard = await dashboardApiService
        .GetHomeDashboardAsync()
        .ConfigureAwait(false);

      UpdateCollections(dashboard);
      DashboardGeneratedAtUtc = dashboard?.GeneratedAtUtc;
    }
    catch (Exception ex)
    {
      ExceptionHandlingService.Handle(ex, "Loading dashboard");
    }
    finally
    {
      IsBusy = false;
    }
  }

  private void UpdateCollections(HomeDashboardSummary? dashboard)
  {
    MainThread.BeginInvokeOnMainThread(() =>
    {
      UpdateSegments(AttendanceSummarySegments, dashboard?.AttendanceSummary);
      UpdateTrend(WeeklyAttendanceTrend, dashboard?.WeeklyAttendanceTrend);
      UpdateSegments(LeaveDistributionSegments, dashboard?.LeaveDistribution);
      UpdateTrend(MonthlyAbsenceTrend, dashboard?.MonthlyAbsenceTrend);
      UpdateLeaders(PerfectAttendanceLeaders, dashboard?.PerfectAttendanceLeaders);
    });
  }

  private void UpdateSegments(
    ObservableCollection<DashboardChartSegment> target,
    IEnumerable<ChartSegmentSummary>? source)
  {
    target.Clear();

    if (source is null)
    {
      return;
    }

    List<ChartSegmentSummary> segments = source
      .Where(segment => segment is not null)
      .Select(segment => new ChartSegmentSummary
      {
        Label = segment!.Label ?? string.Empty,
        Value = Math.Max(segment.Value, 0d)
      })
      .ToList();

    double total = segments.Sum(segment => segment.Value);
    int index = 0;

    foreach (ChartSegmentSummary segment in segments)
    {
      double percentage = total > 0 ? segment.Value / total : 0d;
      Color color = ChartPalette[index % ChartPalette.Length];

      target.Add(new DashboardChartSegment
      {
        Label = segment.Label,
        Value = segment.Value,
        Percentage = percentage,
        Color = color
      });

      index++;
    }
  }

  private void UpdateTrend(
    ObservableCollection<DashboardTrendPoint> target,
    IEnumerable<TrendPointSummary>? source)
  {
    target.Clear();

    if (source is null)
    {
      return;
    }

    foreach (TrendPointSummary point in source.Where(point => point is not null))
    {
      target.Add(new DashboardTrendPoint
      {
        Label = point.Label ?? string.Empty,
        Value = Math.Max(point.Value, 0d)
      });
    }
  }

  private void UpdateLeaders(
    ObservableCollection<DashboardLeaderboardEntry> target,
    IEnumerable<PerfectAttendanceLeaderboardEntry>? source)
  {
    target.Clear();

    if (source is null)
    {
      return;
    }

    int rank = 1;
    foreach (PerfectAttendanceLeaderboardEntry entry in source.Where(entry => entry is not null))
    {
      target.Add(new DashboardLeaderboardEntry
      {
        Rank = rank++,
        EmployeeName = entry.EmployeeName ?? string.Empty,
        PerfectDays = entry.PerfectDays,
        TotalTrackedDays = entry.TotalTrackedDays,
        AttendanceRate = entry.AttendanceRate,
        LateDays = entry.LateDays,
        Absences = entry.Absences
      });
    }
  }

  partial void OnDashboardGeneratedAtUtcChanged(DateTime? value)
  {
    OnPropertyChanged(nameof(LastUpdatedDisplay));
  }
}

public class DashboardChartSegment
{
  public string Label { get; set; } = string.Empty;

  public double Value { get; set; }

  public double Percentage { get; set; }

  public Color Color { get; set; } = Colors.Transparent;
}

public class DashboardTrendPoint
{
  public string Label { get; set; } = string.Empty;

  public double Value { get; set; }
}

public class DashboardLeaderboardEntry
{
  public int Rank { get; set; }

  public string EmployeeName { get; set; } = string.Empty;

  public int PerfectDays { get; set; }

  public int TotalTrackedDays { get; set; }

  public double AttendanceRate { get; set; }

  public int LateDays { get; set; }

  public int Absences { get; set; }
}
