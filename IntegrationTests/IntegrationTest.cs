﻿using System.Net;
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
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;

public class AuthCrudFlowTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _authClient;
    private readonly HttpClient _crudClient;

    private readonly ITestOutputHelper _output;
    public AuthCrudFlowTests(ITestOutputHelper output)
    {
        _output = output;
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });
        _authClient = _factory.CreateClient();
        _crudClient = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7261/")
        };
    }
    /// <summary>
    /// Tests if authAPI works
    /// </summary>
    [Fact]
    public async Task SanityCheck_AuthApiResponds()
    {
        var response = await _authClient.GetAsync("/api/auth/login");
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine("=== LOGIN GET ===");
        Console.WriteLine($"Status: {response.StatusCode}");
        Console.WriteLine($"Body: {content}");
        Console.WriteLine("=================");

        Assert.NotEqual(HttpStatusCode.InternalServerError, response.StatusCode);
    }
    /// <summary>
    /// Integration test for AuthAPI and CRUDAPI
    /// </summary>
    /// <returns>Success if everything went right</returns>
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
        var loginModel = new LoginViewModel()
        {
            Email = "testuser@example.com",
            Password = "Test1234!"
        };

        var loginResponse = await _authClient.PostAsJsonAsync<LoginViewModel>("api/auth/login", loginModel);
        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);

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
        Assert.Equal(HttpStatusCode.Unauthorized, passResponse.StatusCode);


        // --- 5. Delete user ---
        var deleteResponse = await _crudClient.DeleteAsync($"api/CRUD/user/{user.Id}");
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
    }
}
