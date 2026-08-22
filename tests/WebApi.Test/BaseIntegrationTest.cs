using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using MyRecipeBook.Domain.Extensions;
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

    protected Task<HttpResponseMessage> Get(string requestUri, CancellationToken cancellationToken,
        string culture = "en", string acessToken = "")
    {
        ChangeRequestCulture(culture);
        AuthorizeRequest(acessToken);

        return _httpClient.GetAsync(requestUri, cancellationToken);
    }

    private void ChangeRequestCulture(string culture)
    {
        _httpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
        _httpClient.DefaultRequestHeaders.AcceptLanguage.ParseAdd(culture);
    }

    private void AuthorizeRequest(string acessToken)
    {
        if (acessToken.IsNotEmpty())
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", acessToken);
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        dbContext.Dispose();
        _scope.Dispose();
    }
}