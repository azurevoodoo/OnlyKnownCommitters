#load "../PullRequestWebhook/GitHubPayload.csx"
#load "HttpClientHelper.csx"

public static async Task Run(GitHubPayload gitHubPayload, TraceWriter log)
{
    switch(gitHubPayload?.action)
    {
        case "synchronize":
        case "opened":
        case "reopened":
            break;
        default:
            log.Info($"Skipping action: {gitHubPayload?.action}");
            return;
    }

    if (!(gitHubPayload.statusUrl?.StartsWith("https://api.github.com/repos/") ?? false))
    {
        log.Error("Aborting, no status url specified.");
        return;
    }

    if (!(gitHubPayload.commitsUrl?.StartsWith("https://api.github.com/repos/") ?? false))
    {
        log.Error("Aborting, no commits url specified.");
        return;
    }
    
    log.Info($"Action: {gitHubPayload.action}, Status: {gitHubPayload.statusUrl}, Commits: {gitHubPayload.commitsUrl}");
    
    foreach(var commit in await GetObjectAsync<dynamic[]>(gitHubPayload.commitsUrl))
    {
        string  sha = commit?.sha,
                authorId = (commit?.author!=null) ? commit.author.id : null,
                state = (!string.IsNullOrEmpty(authorId)) ? "success" : "failure";

        var status = new {
            state = state,
            target_url = (string)commit?.html_url,
            description = $"Commit {sha} author validation {state}",   
            context = $"author/validation/{sha}"
        };
        
        log.Info($"Status: {status}");

        await PostObjectAsJsonAsync<dynamic, dynamic>(gitHubPayload.statusUrl, status);
    }
}