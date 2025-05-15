using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using SharedModels;
using HCM_app.ViewModels;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using AuthAPIHCM;
using Microsoft.AspNetCore.Hosting;

public class AuthCrudFlowTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _authClient;
    private readonly HttpClient _crudClient;

    public AuthCrudFlowTests()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });

        _authClient = _factory.CreateClient(); // ✅ No BaseAddress – uses in-memory test server

        _crudClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7261/") // External CRUD service still fine
        };
    }

    [Fact]
    public async Task ExampleTest()
    {
        var response = await _authClient.GetAsync("/api/auth/login");
        Assert.NotNull(response);
    }
    [Fact]
    public async Task RegisterLoginCrudFlow_Succeeds()
    {
        // --- 1. Register User ---
        var registerModel = new RegisterViewModel()
        {
            Email = "testuser@example.com",
            Password = "Test1234!",
            FirstName = "Test",
            LastName = "User",
            Department = "IT",
            JobTitle = "Engineer",
            Salary = 60000.0
        };

        var registerResponse = await _authClient.PostAsJsonAsync<RegisterViewModel>("api/auth/register", registerModel);
        Assert.Equal(HttpStatusCode.NoContent, registerResponse.StatusCode);

        // --- 2. Login User ---
        var loginModel = new
        {
            Email = "testuser@example.com",
            Password = "Test1234!"
        };

        var loginResponse = await _authClient.PostAsJsonAsync("api/auth/login", loginModel);
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var token = await loginResponse.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(token));

        // --- 3. Get user by email ---
        var emailResponse = await _crudClient.GetAsync($"api/CRUD/users/email-testuser@example.com");
        Assert.Equal(HttpStatusCode.OK, emailResponse.StatusCode);

        var user = await emailResponse.Content.ReadFromJsonAsync<UserDataModel>();
        Assert.Equal("testuser@example.com", user.Email);

        // --- 4. Update password ---
        _crudClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Trim('"'));

        var passUpdate = new ChangePassViewModel(user.Id, "NewPass123!");

        var passResponse = await _crudClient.PatchAsJsonAsync("api/CRUD/user/password", passUpdate);
        var body = await passResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {passResponse.StatusCode}");
        Console.WriteLine($"Body: {body}");
        Assert.Equal(HttpStatusCode.NoContent, passResponse.StatusCode);


        // --- 5. Delete user ---
        var deleteResponse = await _crudClient.DeleteAsync($"api/CRUD/user/{user.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}
