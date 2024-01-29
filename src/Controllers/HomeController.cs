using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MIOS.net.Models;

namespace MIOS.net.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ILogger<HomeController> logger,
        IHttpClientFactory clientFactory)
    {
        _logger = logger;
        _clientFactory = clientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Download()
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            "http://opensimulator.org/skins/osmonobook/bodyBG.png");

        var client = _clientFactory.CreateClient();

        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            //save the response as a file to disk
            var fileStream = new FileStream("bodyBG.png", FileMode.Create);
            await response.Content.CopyToAsync(fileStream);
            fileStream.Close();
            return View("Index");
        }
        else
        {
            return View("Error");
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
