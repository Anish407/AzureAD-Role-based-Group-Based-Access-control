using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Authorization;

namespace WebApplication
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
            string[] initialScopes = Configuration.GetValue<string>("CallApi:ScopeForAccessToken")?.Split(' ');
            services.AddScoped<IGraphServiceClient, GraphServiceClient>();

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
              .AddMicrosoftIdentityWebApp(Configuration, "AzureAd")
              .EnableTokenAcquisitionToCallDownstreamApi(new string[] { "api://myadwebapi/access_as_user" })
              .AddMicrosoftGraph(Configuration.GetSection("GraphBeta"))
              .AddDownstreamWebApi("MyApi", Configuration.GetSection("GraphBeta"))
              .AddInMemoryTokenCaches();

            //services.AddMicrosoftIdentityWebAppAuthentication(Configuration, "AzureAd");
            services.AddHttpContextAccessor();


            services.AddAuthorization(options =>
            {
                options.AddPolicy("CheckGroups", policy =>
                    policy.Requirements.Add(new GroupsCheckRequirement("7b78463a-890e-4132-aca1-4fdc13694dab")));
            });
            services.AddScoped<IAuthorizationHandler, GroupsCheckHandler>();


            services.AddRazorPages().AddMvcOptions(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

           
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
