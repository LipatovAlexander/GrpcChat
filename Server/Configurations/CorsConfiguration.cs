// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CorsConfiguration
{
    public static IServiceCollection AddGrpcWebCors(this IServiceCollection services)
    {
        return services.AddCors(o => o.AddPolicy("AllowAll", b =>
        {
            b.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
        }));
    }
}