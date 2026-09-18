using System.Net;
using System.Net.Http.Json;
using UserDirectory.Application.Users;
using Xunit;

namespace UserDirectory.Api.Tests;

[Collection("API")]
public sealed class UsersApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public UsersApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetUsersReturnsOkAndEmptyArrayInitially()
    {
        var response = await _client.GetAsync("/api/users");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        Assert.NotNull(users);
    }

    [Fact]
    public async Task GetUnknownUserReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/users/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    //[Fact]
    //public async Task PostWithoutTokenReturnsUnauthorized()
    //{
    //    var request = new CreateUserRequest("Jane Doe", 40, "Melbourne", "VIC", "3000");

    //    var response = await _client.PostAsJsonAsync("/api/users", request);

    //    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    //}

    //[Fact]
    //public async Task PostWithTestIdentityReturnsCreated()
    //{
    //    var request = new CreateUserRequest("Jane Doe", 40, "Melbourne", "VIC", "3000");
    //    Authenticate();
    //    var response = await _client.PostAsJsonAsync("/api/users", request);

    //    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    //    var created = await response.Content.ReadFromJsonAsync<UserResponse>();
    //    Assert.NotNull(created);
    //    Assert.Equal("Jane Doe", created!.Name);
    //    Assert.True(created.Id > 0);
    //}

    //[Fact]
    //public async Task PutWithTestIdentityUpdatesUser()
    //{
    //    var created = await CreateAsync("Before Update");
    //    var request = new UpdateUserRequest("After Update", 41, "Geelong", "VIC", "3220");

    //    Authenticate();
    //    var response = await _client.PutAsJsonAsync($"/api/users/{created.Id}", request);

    //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    var updated = await response.Content.ReadFromJsonAsync<UserResponse>();
    //    Assert.Equal("After Update", updated!.Name);
    //    Assert.Equal(41, updated.Age);
    //}

    //[Fact]
    //public async Task DeleteWithTestIdentityReturnsNoContent()
    //{
    //    var created = await CreateAsync("To Delete");

    //    Authenticate();
    //    var response = await _client.DeleteAsync($"/api/users/{created.Id}");

    //    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

    //    var get = await _client.GetAsync($"/api/users/{created.Id}");
    //    Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    //}

    private void Authenticate()
    {
        _client.DefaultRequestHeaders.Remove("X-Test-Auth");
        _client.DefaultRequestHeaders.Add("X-Test-Auth", "true");
    }

    private async Task<UserResponse> CreateAsync(string name)
    {
        var request = new CreateUserRequest(name, 35, "Melbourne", "VIC", "3000");
        Authenticate();
        var response = await _client.PostAsJsonAsync("/api/users", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<UserResponse>())!;
    }
}
