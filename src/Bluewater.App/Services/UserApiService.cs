using System;
using System.Collections.Generic;
using System.Linq;
using Bluewater.App.Interfaces;
using Bluewater.App.Models;

namespace Bluewater.App.Services;

public class UserApiService(IApiClient apiClient) : IUserApiService
{
  public async Task<IReadOnlyList<UserRecordDto>> GetUsersAsync(
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    string requestUri = BuildRequestUri(skip, take);

    UserListResponseDto? response = await apiClient
      .GetAsync<UserListResponseDto>(requestUri, cancellationToken)
      .ConfigureAwait(false);

    if (response?.Users is not { Count: > 0 })
    {
      return Array.Empty<UserRecordDto>();
    }

    return response.Users
      .Where(user => user is not null)
      .ToList();
  }

  public async Task<UserRecordDto?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    if (userId == Guid.Empty)
    {
      throw new ArgumentException("User ID must be provided", nameof(userId));
    }

    GetUserByIdResponseDto? response = await apiClient
      .GetAsync<GetUserByIdResponseDto>($"Users/{userId}", cancellationToken)
      .ConfigureAwait(false);

    return response?.User;
  }

  public async Task<UserRecordDto?> CreateUserAsync(UserRecordDto user, CancellationToken cancellationToken = default)
  {
    if (user is null)
    {
      throw new ArgumentNullException(nameof(user));
    }

    CreateUserRequestDto request = new()
    {
      Username = user.Username,
      PasswordHash = user.PasswordHash,
      Credential = user.Credential,
      SupervisedGroup = user.SupervisedGroup,
      IsGlobalSupervisor = user.IsGlobalSupervisor
    };

    CreateUserResponseDto? response = await apiClient
      .PostAsync<CreateUserRequestDto, CreateUserResponseDto>(CreateUserRequestDto.Route, request, cancellationToken)
      .ConfigureAwait(false);

    return response is null
      ? null
      : await GetUserByIdAsync(response.Id, cancellationToken).ConfigureAwait(false);
  }

  public async Task<UserRecordDto?> UpdateUserAsync(UserRecordDto user, CancellationToken cancellationToken = default)
  {
    if (user is null)
    {
      throw new ArgumentNullException(nameof(user));
    }

    if (user.Id == Guid.Empty)
    {
      throw new ArgumentException("User ID must be provided", nameof(user));
    }

    UpdateUserRequestDto request = new()
    {
      Id = user.Id,
      Username = user.Username,
      PasswordHash = user.PasswordHash,
      Credential = user.Credential,
      SupervisedGroup = user.SupervisedGroup,
      IsGlobalSupervisor = user.IsGlobalSupervisor
    };

    UpdateUserResponseDto? response = await apiClient
      .PutAsync<UpdateUserRequestDto, UpdateUserResponseDto>(UpdateUserRequestDto.Route, request, cancellationToken)
      .ConfigureAwait(false);

    return response?.User;
  }

  public Task<bool> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    if (userId == Guid.Empty)
    {
      throw new ArgumentException("User ID must be provided", nameof(userId));
    }

    return apiClient.DeleteAsync($"Users/{userId}", cancellationToken);
  }

  private static string BuildRequestUri(int? skip, int? take)
  {
    List<string> parameters = [];

    if (skip.HasValue)
    {
      parameters.Add($"skip={skip.Value}");
    }

    if (take.HasValue)
    {
      parameters.Add($"take={take.Value}");
    }

    if (parameters.Count == 0)
    {
      return "Users";
    }

    string query = string.Join('&', parameters);
    return $"Users?{query}";
  }
}
