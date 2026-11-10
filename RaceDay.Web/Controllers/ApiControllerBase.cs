using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Web.Controllers
{
    // This base controller sends the API session cookie with each API request.
    public abstract class ApiControllerBase : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        protected ApiControllerBase(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        protected HttpClient CreateApiClient()
        {
            var client = _clientFactory.CreateClient("RaceDayApi");

            // The API uses its own session cookie to identify the logged-in user.
            var apiCookie = HttpContext.Session.GetString("ApiSessionCookie");

            if (!string.IsNullOrWhiteSpace(apiCookie))
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", apiCookie);
            }

            return client;
        }

        protected bool IsLoggedIn()
        {
            return HttpContext.Session.GetInt32("UserID") != null;
        }

        protected bool HasRole(string role)
        {
            return HttpContext.Session.GetString("Role") == role;
        }
    }
}
