using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Hosting;

public static class KestrelConfiguration
{
    public static IWebHostBuilder ConfigureGrpcWebHosting(this IWebHostBuilder builder)
    {
        return builder.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Any, 5000, o =>
            {
                o.Protocols = HttpProtocols.Http1;
                o.UseHttps();
            });

            options.Listen(IPAddress.Any, 5001, o =>
            {
                o.Protocols = HttpProtocols.Http2;
                o.UseHttps();
            });
        });
    }
}