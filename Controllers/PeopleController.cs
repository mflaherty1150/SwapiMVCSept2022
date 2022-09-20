using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwapiMVC.Models;

namespace SwapiMVC.Controllers
{
    [Route("[controller]")]
    public class PeopleController : Controller
    {
        private readonly ILogger<PeopleController> _logger;
        private readonly HttpClient _httpClient;

        public PeopleController(ILogger<PeopleController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("swapi");
        }

        [Route("Index")]
        public async Task<IActionResult> Index(string page)
        {
            string route = $"people?page={page ?? "1"}";
            HttpResponseMessage response = await _httpClient.GetAsync(route);

            var responseString = await response.Content.ReadAsByteArrayAsync();
            var people = JsonSerializer.Deserialize<ResultsViewModel<PeopleViewModel>>(responseString);

            return View(people);
        }

        [Route("Person")]
        public async Task<IActionResult> Person(string id)
        {
            var response = await _httpClient.GetAsync($"people/{id}");
            if (id is null || response.IsSuccessStatusCode == false)
                return RedirectToAction(nameof(Index));

            var responseString = await response.Content.ReadAsByteArrayAsync();
            var person = JsonSerializer.Deserialize<PeopleViewModel>(responseString);
            return View(person);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("[action]")]
        public IActionResult Error(ErrorEventArgs e)
        {
            return View("Error!");
        }
    }
}