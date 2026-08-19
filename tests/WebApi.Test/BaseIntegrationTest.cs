using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Infrastructure.DataAcess;
using Xunit;

namespace WebApi.Test;

public class BaseIntegrationTest : IClassFixture<MyRecipeBookApplicationFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    private readonly HttpClient _httpClient;
    internal MyRecipeBookDbContext dbContext;

    public BaseIntegrationTest(MyRecipeBookApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();

        _scope = factory.Services.CreateScope();

        dbContext = _scope.ServiceProvider.GetRequiredService<MyRecipeBookDbContext>();
    }

    protected Task<HttpResponseMessage> Post(string requestUri, object request, CancellationToken cancellationToken,
        string culture = "en")
    {
        ChangeRequestCulture(culture);

        return _httpClient.PostAsJsonAsync(requestUri, request, cancellationToken);
    }

    private void ChangeRequestCulture(string culture)
    {
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        dbContext.Dispose();
        _scope.Dispose();
    }
}