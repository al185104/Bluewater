using System;
using System.Net.Http.Json;
using Bluewater.Web.MealCredits;
using Xunit;

namespace Bluewater.FunctionalTests.ApiEndpoints;
[Collection("Sequential")]
public class MealCreditsPost(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task CreatesMealCreditSuccessfully()
  {
    // Arrange
    var employeeId = Guid.NewGuid(); // Replace with a valid seeded or test employee ID
    var request = new CreateMealCreditRequest
    {
      employeeId = employeeId,
      entryDate = DateOnly.FromDateTime(DateTime.Now),
      count = 5
    };

    // Act

    var response = await _client.PostAsJsonAsync(CreateMealCreditRequest.Route, request);
    response.EnsureSuccessStatusCode();
    var createdMealCredit = await response.Content.ReadFromJsonAsync<MealCreditRecord>();

    // Assert
    Assert.NotNull(createdMealCredit);
    Assert.Equal(request.employeeId, createdMealCredit.EmployeeId);
    Assert.Equal(request.entryDate, createdMealCredit.Date);
    Assert.Equal(request.count, createdMealCredit.Count);
  }

  [Fact]
  public async Task FailsToCreateMealCreditWithInvalidData()
  {
    // Arrange
    var request = new CreateMealCreditRequest
    {
      employeeId = Guid.Empty, // Invalid GUID
      entryDate = null,        // Null entry date
      count = -1               // Invalid count
    };

    // Act
    var response = await _client.PostAsJsonAsync(CreateMealCreditRequest.Route, request);

    // Assert
    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
  }
}
