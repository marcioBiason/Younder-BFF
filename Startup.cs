using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Younder_BFF;
using Younder_BFF.EF;
using Younder_BFF.v1.Services;

namespace YounderBFF {
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration"></param>
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration Field
        /// </summary>
        /// <value></value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices (IServiceCollection services) {
            var conf_origins = Configuration["CORSWithOrigins:URL"];
            Console.WriteLine ($"Origins permitidas: {conf_origins}");
            var origins = conf_origins.Split ("#");

            services.AddControllers ();
            services.AddCors (options => options.AddPolicy ("InternalPolicy", builder => {
                builder.AllowAnyHeader ()
                    .AllowAnyMethod ()
                    .WithOrigins (origins)
                    .AllowCredentials ();
            }));

            services.AddDbContext<BancoDadosContext> (
                x => x.UseSqlite (Configuration["ConnectionString:DefaultConnection"]));

            services.AddScoped<IUsuarioService, UsuarioService> ();

            #region Configure JWT
            services.AddAuthentication (options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer (Options => {
                    Options.SaveToken = true;
                    Options.RequireHttpsMetadata = true;

                    Options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters () {
                        ValidateIssuerSigningKey = false,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        IssuerSigningKey = new SymmetricSecurityKey (Encoding.ASCII.GetBytes (Configuration["Jwt:Secret"]))
                    };
                });
            #endregion

            #region ApiVersioning
            services.AddApiVersioning (options => {
                options.UseApiBehavior = false;
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion (1, 0);
                options.ApiVersionReader = ApiVersionReader.Combine (
                    new HeaderApiVersionReader ("x-api-version"),
                    new QueryStringApiVersionReader (),
                    new UrlSegmentApiVersionReader ()
                );
            });
            services.AddVersionedApiExplorer (
                options => {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions> ();
            services.AddSwaggerGen (options => { // integrate xml comments
                options.IncludeXmlComments (XmlCommentsFilePath);

                options.AddSecurityDefinition ("Bearer", new OpenApiSecurityScheme {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                });

                options.AddSecurityRequirement (new OpenApiSecurityRequirement () {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                            }
                        }, new List<string> ()
                    }
                });
            });
            #endregion
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="provider"></param>
        /// <param name="env"></param>
        public void Configure (IApplicationBuilder app, IApiVersionDescriptionProvider provider, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            }

            app.UseSwagger ();
            app.UseSwaggerUI (
                options => {
                    foreach (var description in provider.ApiVersionDescriptions) {
                        options.SwaggerEndpoint (
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant ());
                    }
                });

            app.UseRouting ();

            app.UseAuthentication ();
            app.UseAuthorization ();
            app.UseCors (x => x.AllowAnyOrigin ().AllowAnyMethod ().AllowAnyHeader ());
            //app.UseMvc ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ();
            });
        }

        private static string XmlCommentsFilePath {
            get {
                var basePath = Environment.CurrentDirectory; // PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof (Startup).GetTypeInfo ().Assembly.GetName ().Name + ".xml";
                return Path.Combine (basePath, fileName);
            }
        }
    }
}