using Bluewater.Core.Serialization;
using Bluewater.Core.UserAggregate;
using Bluewater.Core.UserAggregate.Enum;
using Bluewater.Infrastructure.Data;
using Bluewater.UseCases.Users.Update;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Bluewater.FunctionalTests.Infrastructure;

public class GuidStorageNormalizationTests
{
  [Fact]
  public async Task StoresGuidValuesInUpperCaseAndFindsExistingRowsCaseInsensitively()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
      .UseSqlite(connection)
      .Options;

    Guid userId = Guid.Parse("a3f92280-2350-4b26-b12c-96b1796fcc1a");

    await using (var seedContext = new AppDbContext(options, null))
    {
      await seedContext.Database.EnsureCreatedAsync();
      await seedContext.Database.ExecuteSqlRawAsync(
        "INSERT INTO AppUsers (Id, Username, PasswordHash, Credential, SupervisedGroup, IsGlobalSupervisor, CreatedDate, CreateBy, UpdatedDate, UpdateBy) VALUES ({0}, {1}, {2}, {3}, NULL, {4}, {5}, {6}, {7}, {8})",
        userId.ToString("D").ToLowerInvariant(),
        "mixedcase.user",
        "hash",
        1,
        false,
        DateTime.UtcNow,
        Guid.Empty.ToString("D").ToLowerInvariant(),
        DateTime.UtcNow,
        Guid.Empty.ToString("D").ToLowerInvariant());
    }

    await using (var queryContext = new AppDbContext(options, null))
    {
      var repository = new EfRepository<AppUser>(queryContext);
      AppUser? existingUser = await repository.GetByIdAsync(userId);

      existingUser.Should().NotBeNull();
      existingUser!.Id.Should().Be(userId);
    }

    Guid newUserId = Guid.Parse("b4cb8175-7a4f-4de7-8af7-d4d5f6b6a4cc");

    await using (var writeContext = new AppDbContext(options, null))
    {
      AppUser newUser = new("writer.user", "hash", Core.UserAggregate.Enum.Credential.Admin, null, false);
      typeof(AppUser).GetProperty(nameof(AppUser.Id))!.SetValue(newUser, newUserId);
      writeContext.AppUsers.Add(newUser);
      await writeContext.SaveChangesAsync();
    }

    await using (var verifyContext = new AppDbContext(options, null))
    {
      string storedId = (await verifyContext.Database
        .SqlQueryRaw<string>("SELECT Id FROM AppUsers WHERE Username = {0}", "writer.user")
        .ToListAsync())
        .Single();
      storedId.Should().Be(newUserId.ToString("D").ToUpperInvariant());
    }
  }

  [Fact]
  public async Task UpdateHandlerFindsExistingUserWhenPayloadGuidUsesDifferentLetterCase()
  {
    await using var connection = new SqliteConnection("Data Source=:memory:");
    await connection.OpenAsync();

    DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
      .UseSqlite(connection)
      .Options;

    Guid userId = Guid.Parse("f8c603c4-71a9-4f96-a44e-f4cc04c90e8b");

    await using (var seedContext = new AppDbContext(options, null))
    {
      await seedContext.Database.EnsureCreatedAsync();
      await seedContext.Database.ExecuteSqlRawAsync(
        "INSERT INTO AppUsers (Id, Username, PasswordHash, Credential, SupervisedGroup, IsGlobalSupervisor, CreatedDate, CreateBy, UpdatedDate, UpdateBy) VALUES ({0}, {1}, {2}, {3}, NULL, {4}, {5}, {6}, {7}, {8})",
        userId.ToString("D").ToLowerInvariant(),
        "mixedcase.user",
        "hash",
        1,
        false,
        DateTime.UtcNow,
        Guid.Empty.ToString("D").ToLowerInvariant(),
        DateTime.UtcNow,
        Guid.Empty.ToString("D").ToLowerInvariant());
    }

    await using (var updateContext = new AppDbContext(options, null))
    {
      EfRepository<AppUser> repository = new(updateContext);
      UpdateUserHandler handler = new(repository);

      var result = await handler.Handle(
        new UpdateUserCommand(userId, "updated.user", "newhash", Credential.Admin, null, true),
        CancellationToken.None);

      result.IsSuccess.Should().BeTrue();
      result.Value.Username.Should().Be("updated.user");
      result.Value.Id.Should().Be(userId);
    }
  }

  [Fact]
  public void SerializesGuidValuesAsUpperCaseWhileAcceptingMixedCaseInput()
  {
    JsonSerializerOptions options = new JsonSerializerOptions().UseUpperCaseGuids();
    Guid guid = Guid.Parse("a3f92280-2350-4b26-b12c-96b1796fcc1a");

    string json = JsonSerializer.Serialize(new GuidPayload(guid, guid), options);
    GuidPayload? payload = JsonSerializer.Deserialize<GuidPayload>("""{"Id":"a3f92280-2350-4b26-b12c-96b1796fcc1a","OptionalId":"A3F92280-2350-4B26-B12C-96B1796FCC1A"}""", options);

    json.Should().Contain(guid.ToString("D").ToUpperInvariant());
    payload.Should().NotBeNull();
    payload!.Id.Should().Be(guid);
    payload.OptionalId.Should().Be(guid);
  }

  private sealed record GuidPayload(Guid Id, Guid? OptionalId);
}
