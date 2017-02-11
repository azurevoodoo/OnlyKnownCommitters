using System.Net;
using System.Net.Http.Headers;

private static string GitHub_Authorization { get; }
    = $"token {System.Environment.GetEnvironmentVariable("CUSTOMCONNSTR_GITHUB_TOKEN", EnvironmentVariableTarget.Process)}";

public static async Task<T> GetObjectAsync<T>(string uri)
{
    return await UseHttpClient<T>(
        client => client.GetAsync(uri)
    );
}

public static async Task<TOut> PostObjectAsJsonAsync<TIn, TOut>(string uri, TIn value)
{  
    return await UseHttpClient<TOut>(
        client => client.PostAsJsonAsync(uri,value)
    );
}

private static async Task<T> UseHttpClient<T>(Func<HttpClient, Task<HttpResponseMessage>> action)
{
    using (var client = new HttpClient())
    {
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("PullRequestValidator","1.0.0"));
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", GitHub_Authorization);
        var response = await action.Invoke(client);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsAsync<T>();
    }
}
