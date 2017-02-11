#load "GitHubPayload.csx"
using System.Net;

public static async Task<GitHubPayload> Run(HttpRequestMessage req, TraceWriter log)
{
    dynamic data = await req.Content.ReadAsAsync<object>();

    return new GitHubPayload {
            action = data?.action,
            statusUrl = data?.pull_request?._links?.statuses?.href,
            commitsUrl = data?.pull_request?._links?.commits?.href
        };
}