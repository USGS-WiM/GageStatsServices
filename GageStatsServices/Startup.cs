using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GageStatsAgent;
using Microsoft.AspNetCore.Mvc;
using WIM.Services.Middleware;
using WIM.Services.Analytics;
using WIM.Utilities.ServiceAgent;
using WIM.Services.Resources;
using GageStatsServices.Filters;
using GageStatsDB;
using WIM.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using WIM.Security.Authentication.Basic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using WIM.Services.Messaging;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NetTopologySuite.Geometries;
using GageStatsDB.Resources;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WIM.Services.Security.Authentication.JWTBearer;
using WIM.Security.Authorization;
using GageStatsServices.Resources;

namespace GageStatsServices
{
    public class Startup
    {
        private string _hostKey = "USGSWiM_HostName";
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
            if (env.IsDevelopment()) {
                builder.AddUserSecrets<Startup>();
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
        }//end startup       

        public IConfigurationRoot Configuration { get; }

        //Method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Transient objects are always different; a new instance is provided to every controller and every service.
            //Singleton objects are the same for every object and every request.
            //Scoped objects are the same within a request, but different across different requests.
            
            //Configure injectable obj
            services.AddScoped<IAnalyticsAgent, GoogleAnalyticsAgent>((gaa) => new GoogleAnalyticsAgent(Configuration["AnalyticsKey"]));
            services.Configure<APIConfigSettings>(Configuration.GetSection("APIConfigSettings"));
            services.Configure<JwtBearerSettings>(Configuration.GetSection("JwtBearerSettings"));

            //provides access to httpcontext
            services.AddHttpContextAccessor();
            services.AddScoped<GageStatsAgent.GageStatsAgent>();
            services.AddScoped<IGageStatsAgent>(x => x.GetRequiredService<GageStatsAgent.GageStatsAgent>());
            services.AddScoped<IAuthenticationAgent>(x => x.GetRequiredService<GageStatsAgent.GageStatsAgent>());

            // Add framework services
            services.AddDbContext<GageStatsDBContext>(options =>
                                                        options.UseNpgsql(String.Format(Configuration
                                                            .GetConnectionString("Connection"), Configuration["dbuser"], Configuration["dbpassword"], Configuration["dbHost"]),
                                                            //default is 1000, if > maxbatch, then EF will group requests in maxbatch size
                                                            opt => { opt.MaxBatchSize(1000); opt.UseNetTopologySuite(); })
                                                            .EnableSensitiveDataLogging());

            //Authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = BasicDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = BasicDefaults.AuthenticationScheme;
            }).AddBasicAuthentication()
           .AddJwtBearer(options =>
           {
               //options.Events = new JWTBearerAuthenticationEvents();
               options.Events = new testClass();
               options.RequireHttpsMetadata = false;
               options.SaveToken = true;
               
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JwtBearerSettings:SecretKey"])),
                   ValidateIssuer = false,
                   ValidateAudience = false                  

               };
           });

            //Authorization
            services.AddAuthorization(options => loadAutorizationPolicies(options));
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin()
                                                                 .AllowAnyMethod()
                                                                 .AllowAnyHeader()
                                                                 .WithExposedHeaders(new string[] { this._hostKey, X_MessagesDefault.msgheader })
                                                                 .AllowCredentials());
            });

            services.AddMvc(options => {
                options.RespectBrowserAcceptHeader = true;
                options.Filters.Add(new GageStatsHypermedia());
                options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(Point)));
                options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(Geometry)));
            })                               
                .AddJsonOptions(options => loadJsonOptions(options));                                                
        }     

        // Method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseX_Messages(option => { option.HostKey = this._hostKey; });
            app.UseAuthentication();
            app.UseCors("CorsPolicy");
            app.Use_Analytics();            

            app.UseMvc();            
        }

        #region Helper Methods
        private void loadJsonOptions(MvcJsonOptions options)
        {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            options.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
            options.SerializerSettings.TypeNameAssemblyFormatHandling = Newtonsoft.Json.TypeNameAssemblyFormatHandling.Simple;
            options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
        }
        private void loadAutorizationPolicies(AuthorizationOptions options)
        {
            //https://www.thereformedprogrammer.net/a-better-way-to-handle-authorization-in-asp-net-core/
            //https://jasonwatmore.com/post/2019/01/08/aspnet-core-22-role-based-authorization-tutorial-with-example-api
     
            options.AddPolicy(
                Policy.Managed,
                policy => policy.RequireRole(Role.Admin, Role.Manager));
            options.AddPolicy(
                Policy.AdminOnly,
                policy => policy.RequireRole(Role.Admin));
        }
        #endregion
    }
}
