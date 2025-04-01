namespace TodoList.App.Middlewares;

public sealed class ContainerIdHeaderMiddleware(RequestDelegate next)
{
    private readonly string _containerId = GetDockerContainerId();

    public async Task Invoke(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Container-ID"] = _containerId;
            return Task.CompletedTask;
        });

        await next(context);
    }

    // could be cached
    private static string GetDockerContainerId()
    {
        var lines = File.ReadAllLines("/proc/self/cgroup");
        foreach (var line in lines)
        {
            var parts = line.Split(':');
            if (parts.Length == 3 && parts[2].Contains("docker"))
            {
                var idPart = parts[2].Split('/').Last();
                return idPart.Length >= 64 ? idPart[^64..] : idPart;
            }
        }

        throw new InvalidOperationException("Could not determine Docker container ID.");
    }
}