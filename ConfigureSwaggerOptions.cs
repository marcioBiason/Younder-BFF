using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Younder_BFF
{
   /// <summary>
    /// Classe de Configuração de versões no Swagger
    /// </summary>
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private static readonly string ApiName = "Template - API";
        readonly IApiVersionDescriptionProvider provider;

        /// <summary>
        /// Provider
        /// </summary>
        /// <param name="provider"></param>
        public ConfigureSwaggerOptions( IApiVersionDescriptionProvider provider ) => this.provider = provider;

        /// <summary>
        /// Configura as versões no swagger
        /// </summary>
        /// <param name="options"></param>
        public void Configure( SwaggerGenOptions options )
        {
            foreach ( var description in provider.ApiVersionDescriptions )
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo { Title = ApiName, Version = description.ApiVersion.ToString() });        
            }
        }
    }
}