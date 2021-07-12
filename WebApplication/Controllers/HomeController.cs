using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        public IHttpClientFactory ClientFactory { get; }
        public ITokenAcquisition TokenAcquisition { get; }
        public IConfiguration Configuration { get; }
        public GraphServiceClient GraphServiceClient { get; }

        public HomeController(IHttpClientFactory clientFactory,
            ITokenAcquisition tokenAcquisition,
            IConfiguration configuration,
            GraphServiceClient graphServiceClient)
        {
            ClientFactory = clientFactory;
            TokenAcquisition = tokenAcquisition;
            Configuration = configuration;
            GraphServiceClient = graphServiceClient;
        }

        [AuthorizeForScopes(Scopes = new[] { "api://myadwebapi/access_as_user" })]
        public async Task<IActionResult> Index()
        {
            var client = ClientFactory.CreateClient();

            // User will contain the claims that are present in the ID token.
            var user = User;


            var scope = Configuration["CallApi:ScopeForAccessToken"];
            //get access token for logged in user
            var accessToken = await TokenAcquisition.GetAccessTokenForUserAsync(new[] { scope });
            // get access  token for the application
            //var accessTokenforapp = await TokenAcquisition.GetAccessTokenForAppAsync("api://myadwebapi/access_as_user/.default");

            client.BaseAddress = new Uri(Configuration["CallApi:ApiBaseAddress"]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //var response = await client.GetAsync("weatherforecast");
            var response = await client.GetAsync("GroupBasedAuth");

            ViewBag.data = await response.Content.ReadAsStringAsync();


            return View();
        }

        [Authorize(Policy = "CheckGroups")]
        public async Task<IActionResult> Privacy()
        {
            //https://docs.microsoft.com/en-us/azure/active-directory/develop/scenario-web-app-call-api-call-api?tabs=aspnetcore

            // need to configure o365 mail
            //var user = await GraphServiceClient.Me.Request().GetAsync();
            //try
            //{
            //    using (var photoStream = await GraphServiceClient.Me.Photo.Content.Request().GetAsync())
            //    {
            //        byte[] photoByte = ((MemoryStream)photoStream).ToArray();
            //        ViewBag.data = Convert.ToBase64String(photoByte);
            //    }
            //    ViewBag.Name = user.DisplayName;
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.data = null;
            //}
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
