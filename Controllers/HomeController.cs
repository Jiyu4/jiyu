using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CurrencySystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("user") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Convert(string fromCurrency, string toCurrency, double amount)
        {
            string apiKey = _config["ApiSettings:ApiKey"] ?? "";

            string url =
                $"http://api.exchangeratesapi.io/v1/latest?access_key={apiKey}&symbols={fromCurrency},{toCurrency}";

            using HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(url);

            var data = JObject.Parse(response);
            var rates = data["rates"];

            double fromRate = rates?[fromCurrency]?.Value<double>() ?? 0;
            double toRate = rates?[toCurrency]?.Value<double>() ?? 0;

            double result = 0;

            if (fromRate != 0)
                result = (toRate / fromRate) * amount;

            ViewBag.Result = result;
            ViewBag.From = fromCurrency;
            ViewBag.To = toCurrency;
            ViewBag.Amount = amount;

            return View("Dashboard");
        }
    }
}