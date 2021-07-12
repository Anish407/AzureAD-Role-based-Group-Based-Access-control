using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAdAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // switch off the default ASP.NET Core claim mappings, 
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

          



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //in case of a web api the method is AddMicrosoftIdentity + WebApi  
                .AddMicrosoftIdentityWebApi(Configuration)
                //This line exposes the ITokenAcquisition service that you can use in your controller and page actions.
                .EnableTokenAcquisitionToCallDownstreamApi()
                // we can either use AddDownstreamWebApi or 
                .AddDownstreamWebApi("MyApi", Configuration.GetSection("GraphBeta"))
                //You'll also need to choose a token cache implementation, for example .AddInMemoryTokenCaches()
                .AddInMemoryTokenCaches();

        

            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //  .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, op =>
            //  {
            //      op.Audience = "api://myadwebapi";
            //      op.Authority = "https://login.microsoftonline.com/06e2775e-9d3d-49de-ad36-da82e295fa67/v2.0";
            //      op.TokenValidationParameters.ValidateIssuer = false;
            //  });


            services.AddControllers(options =>
            {
                //add an authorization policy to only allowed authorized requests
                //and the access token must contain an email claim.
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim("roles", "read")
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAdAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyAdAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
