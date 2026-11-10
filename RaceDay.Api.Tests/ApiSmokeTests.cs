
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using Xunit;

namespace RaceDay.Api.Tests;

public class ApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<RaceDayContext>));

                services.AddDbContext<RaceDayContext>(options =>
                    options.UseInMemoryDatabase("RaceDayTests"));
            }));
    }

    [Fact]
    public async Task EventsEndpoint_IsPublic()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/events");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_RejectsUnauthenticatedRequest()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/events",
            new { eventName = "Test Run" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
