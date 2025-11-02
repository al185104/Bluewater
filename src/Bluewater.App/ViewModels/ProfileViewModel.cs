using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Extensions;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class ProfileViewModel : BaseViewModel
{
  private bool hasInitialized;

  public ProfileViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    EditableProfile = CreateNewProfile();
  }

  public ObservableCollection<ProfileDetail> Profiles { get; } = new();

  [ObservableProperty]
  public partial ProfileDetail? SelectedProfile { get; set; }

  [ObservableProperty]
  public partial ProfileDetail EditableProfile { get; set; }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    EnsureSeedProfiles();
  }

  [RelayCommand]
  private void BeginCreateProfile()
  {
    EditableProfile = CreateNewProfile();
    SelectedProfile = null;
  }

  [RelayCommand]
  private void BeginEditProfile(ProfileDetail? profile)
  {
    if (profile is null)
    {
      return;
    }

    SelectedProfile = profile;
    EditableProfile = profile with { };
  }

  [RelayCommand]
  private async Task SaveProfileAsync()
  {
    if (string.IsNullOrWhiteSpace(EditableProfile.DisplayName))
    {
      return;
    }

    bool isNew = Profiles.All(item => item.Id != EditableProfile.Id);

    ProfileDetail profile = EditableProfile;

    if (isNew)
    {
      profile = EditableProfile with { Id = Guid.NewGuid(), RowIndex = Profiles.Count };
      Profiles.Add(profile);
    }
    else
    {
      int index = FindProfileIndex(profile.Id);
      if (index >= 0)
      {
        Profiles[index] = profile with { RowIndex = index };
      }
      else
      {
        profile = profile with { RowIndex = Profiles.Count };
        Profiles.Add(profile);
      }
    }

    UpdateProfileRowIndexes();
    EditableProfile = profile;
    await TraceCommandAsync(nameof(SaveProfileAsync), profile.Id);
  }

  [RelayCommand]
  private async Task DeleteProfileAsync(ProfileDetail? profile)
  {
    if (profile is null)
    {
      return;
    }

    if (Profiles.Remove(profile))
    {
      UpdateProfileRowIndexes();
      await TraceCommandAsync(nameof(DeleteProfileAsync), profile.Id);
    }
  }

  private void EnsureSeedProfiles()
  {
    if (Profiles.Count > 0)
    {
      return;
    }

    Profiles.Add(new ProfileDetail(Guid.NewGuid(), "Miranda Lawson", "miranda.lawson@bluewater.local", "Operations"));
    Profiles.Add(new ProfileDetail(Guid.NewGuid(), "Jacob Taylor", "jacob.taylor@bluewater.local", "Security"));
    Profiles.Add(new ProfileDetail(Guid.NewGuid(), "Kasumi Goto", "kasumi.goto@bluewater.local", "Logistics"));
    UpdateProfileRowIndexes();
  }

  private static ProfileDetail CreateNewProfile()
  {
    return new ProfileDetail(Guid.NewGuid(), string.Empty, string.Empty, string.Empty);
  }

  private int FindProfileIndex(Guid profileId)
  {
    for (int i = 0; i < Profiles.Count; i++)
    {
      if (Profiles[i].Id == profileId)
      {
        return i;
      }
    }

    return -1;
  }

  private void UpdateProfileRowIndexes()
  {
    Profiles.UpdateRowIndexes();
  }
}

public record ProfileDetail(Guid Id, string DisplayName, string Email, string Department) : IRowIndexed
{
  public int RowIndex { get; set; }
}
