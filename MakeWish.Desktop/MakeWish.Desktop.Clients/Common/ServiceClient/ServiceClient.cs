using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentResults;
using MakeWish.Desktop.Clients.Common.Models;
using MakeWish.Desktop.Clients.Common.UserContext;

namespace MakeWish.Desktop.Clients.Common.ServiceClient;

public abstract class ServiceClient
{
    private readonly IUserContext _userContext;
    
    protected readonly HttpClient HttpClient;
    
    protected ServiceClient(
        IHttpClientFactory httpClientFactory,
        IUserContext userContext,
        string baseUrl)
    {
        HttpClient = httpClientFactory.CreateClient();
        HttpClient.BaseAddress = new Uri(baseUrl);
        _userContext = userContext;
    }
    
    protected void AddAuthorizationHeader()
    {
        var token = _userContext.Token;
        if (!string.IsNullOrEmpty(token))
        {
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
    
    protected static async Task<Result<T>> ParseResponse<T>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);
            return Result.Fail(problem?.Detail ?? "Request failed");
        }
        
        var value = await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        return value is not null ? Result.Ok(value) : Result.Fail("Request failed");
    }
    
    protected static async Task<Result<string>> ParseStringResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            return Result.Fail(await response.Content.ReadAsStringAsync(cancellationToken));
        }
        
        var value = await response.Content.ReadAsStringAsync(cancellationToken);
        return Result.Ok(value);
    }
    
    protected static async Task<Result> ParseResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return Result.Ok();
        
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        try
        {
            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(body);
            if (problemDetails != null)
            {

                return Result.Fail(problemDetails.Detail);
            }
        }
        catch (Exception)
        {
            // ignored
        }

        return Result.Fail(body);

    }
}