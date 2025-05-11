using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ReverseProxy.Swagger;

namespace ApiGateway
{
    public sealed class ConfigureSwaggerOptions(IOptions<ReverseProxyDocumentFilterConfig> cfg) : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly ReverseProxyDocumentFilterConfig _config = cfg.Value;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var clusterId in _config.Clusters.Keys)
                options.SwaggerDoc(clusterId, new OpenApiInfo { Title = clusterId, Version = "v1" });
            options.DocumentFilter<ReverseProxyDocumentFilter>();
        }
    }
}