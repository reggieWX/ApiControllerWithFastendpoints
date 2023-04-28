using FastEndpoints;
using System.Threading;
using System.Threading.Tasks;

namespace ApiControllerWithFastendpoints.Endpoints;

public class MyEndpoint : Endpoint<MyRequest, MyResponse>
{
    public override void Configure()
    {
        Post("/hello/world");
        Version(3);
    }

    public override async Task HandleAsync(MyRequest r, CancellationToken c)
    {
        await SendAsync(new()
        {
            FullName = $"{r.FirstName} {r.LastName}",
            Message = "Welcome to FastEndpoints..."
        });
    }
}
