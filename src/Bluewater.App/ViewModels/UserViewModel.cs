using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Bluewater.App.Extensions;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;
using Bluewater.App.ViewModels.Base;
using Bluewater.Core.UserAggregate.Enum;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Bluewater.App.ViewModels;

public partial class UserViewModel : BaseViewModel
{
  private bool hasInitialized;

  public UserViewModel(IActivityTraceService activityTraceService, IExceptionHandlingService exceptionHandlingService)
    : base(activityTraceService, exceptionHandlingService)
  {
    EditableUser = CreateNewUser();
  }

  public ObservableCollection<UserAccount> Users { get; } = new();

  [ObservableProperty]
  public partial UserAccount? SelectedUser { get; set; }

  [ObservableProperty]
  public partial UserAccount EditableUser { get; set; }

  public override async Task InitializeAsync()
  {
    if (hasInitialized)
    {
      return;
    }

    hasInitialized = true;
    await TraceCommandAsync(nameof(InitializeAsync));
    EnsureSeedUsers();
  }

  [RelayCommand]
  private void BeginCreateUser()
  {
    EditableUser = CreateNewUser();
    SelectedUser = null;
  }

  [RelayCommand]
  private void BeginEditUser(UserAccount? user)
  {
    if (user is null)
    {
      return;
    }

    SelectedUser = user;
    EditableUser = user with { };
  }

  [RelayCommand]
  private async Task SaveUserAsync()
  {
    if (string.IsNullOrWhiteSpace(EditableUser.Username))
    {
      return;
    }

    bool isNew = Users.All(item => item.Id != EditableUser.Id);

    UserAccount user = EditableUser;

    if (isNew)
    {
      user = EditableUser with { Id = Guid.NewGuid(), RowIndex = Users.Count };
      Users.Add(user);
    }
    else
    {
      int index = FindUserIndex(user.Id);
      if (index >= 0)
      {
        Users[index] = user with { RowIndex = index };
      }
      else
      {
        user = user with { RowIndex = Users.Count };
        Users.Add(user);
      }
    }

    UpdateUserRowIndexes();
    EditableUser = user;
    await TraceCommandAsync(nameof(SaveUserAsync), user.Id);
  }

  [RelayCommand]
  private async Task DeleteUserAsync(UserAccount? user)
  {
    if (user is null)
    {
      return;
    }

    if (Users.Remove(user))
    {
      UpdateUserRowIndexes();
      await TraceCommandAsync(nameof(DeleteUserAsync), user.Id);
    }
  }

  private void EnsureSeedUsers()
  {
    if (Users.Count > 0)
    {
      return;
    }

    Users.Add(new UserAccount(Guid.NewGuid(), "admin", Credential.Admin, true));
    Users.Add(new UserAccount(Guid.NewGuid(), "hr.manager", Credential.Manager, false));
    Users.Add(new UserAccount(Guid.NewGuid(), "timekeeper", Credential.Scheduler, false));
    UpdateUserRowIndexes();
  }

  private static UserAccount CreateNewUser()
  {
    return new UserAccount(Guid.NewGuid(), string.Empty, Credential.Employee, false);
  }

  private int FindUserIndex(Guid userId)
  {
    for (int i = 0; i < Users.Count; i++)
    {
      if (Users[i].Id == userId)
      {
        return i;
      }
    }

    return -1;
  }

  private void UpdateUserRowIndexes()
  {
    Users.UpdateRowIndexes();
  }
}

public record UserAccount(Guid Id, string Username, Credential Credential, bool IsGlobalSupervisor) : IRowIndexed
{
  public int RowIndex { get; set; }
}
