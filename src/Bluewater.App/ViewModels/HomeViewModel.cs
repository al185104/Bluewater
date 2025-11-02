using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Interfaces;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
  private bool hasInitialized;

  public HomeViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    EditableAnnouncement = CreateNewAnnouncement();
  }

  public ObservableCollection<HomeAnnouncement> Announcements { get; } = new();

  [ObservableProperty]
  public partial HomeAnnouncement? SelectedAnnouncement { get; set; }

  [ObservableProperty]
  public partial HomeAnnouncement EditableAnnouncement { get; set; }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    EnsureSeedAnnouncements();
  }

  [RelayCommand]
  private void BeginCreateAnnouncement()
  {
    EditableAnnouncement = CreateNewAnnouncement();
    SelectedAnnouncement = null;
  }

  [RelayCommand]
  private void BeginEditAnnouncement(HomeAnnouncement? announcement)
  {
    if (announcement is null)
    {
      return;
    }

    SelectedAnnouncement = announcement;
    EditableAnnouncement = announcement with { };
  }

  [RelayCommand]
  private async Task SaveAnnouncementAsync()
  {
    if (string.IsNullOrWhiteSpace(EditableAnnouncement.Title))
    {
      return;
    }

    bool isNew = Announcements.All(item => item.Id != EditableAnnouncement.Id);

    HomeAnnouncement announcement = EditableAnnouncement;

    if (isNew)
    {
      announcement = EditableAnnouncement with { Id = Guid.NewGuid() };
      Announcements.Add(announcement);
    }
    else
    {
      int index = FindAnnouncementIndex(announcement.Id);
      if (index >= 0)
      {
        Announcements[index] = announcement;
      }
      else
      {
        Announcements.Add(announcement);
      }
    }

    EditableAnnouncement = announcement;
    await TraceCommandAsync(nameof(SaveAnnouncementAsync), announcement.Id);
  }

  [RelayCommand]
  private async Task DeleteAnnouncementAsync(HomeAnnouncement? announcement)
  {
    if (announcement is null)
    {
      return;
    }

    if (Announcements.Remove(announcement))
    {
      await TraceCommandAsync(nameof(DeleteAnnouncementAsync), announcement.Id);
    }
  }

  private void EnsureSeedAnnouncements()
  {
    if (Announcements.Count > 0)
    {
      return;
    }

    Announcements.Add(new HomeAnnouncement(Guid.NewGuid(), "Welcome to Bluewater", "Stay up-to-date with your workforce."));
    Announcements.Add(new HomeAnnouncement(Guid.NewGuid(), "Latest Payroll", "Review the latest payroll summaries."));
    Announcements.Add(new HomeAnnouncement(Guid.NewGuid(), "Attendance Alerts", "Check today's attendance variances."));
  }

  private static HomeAnnouncement CreateNewAnnouncement()
  {
    return new HomeAnnouncement(Guid.NewGuid(), string.Empty, string.Empty);
  }
  private int FindAnnouncementIndex(Guid announcementId)
  {
    for (int i = 0; i < Announcements.Count; i++)
    {
      if (Announcements[i].Id == announcementId)
      {
        return i;
      }
    }

    return -1;
  }
}

public record HomeAnnouncement(Guid Id, string Title, string Description);
