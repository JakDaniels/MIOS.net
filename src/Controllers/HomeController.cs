using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MIOS.net.Models;
using IniFileParser;
using IniFileParser.Model;
using System.Text.Json;
using System.Reflection;
using MIOS.net.Interfaces;

namespace MIOS.net.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    private readonly ILogger<HomeController> _logger;

    private readonly IConfiguration _configuration;

    private readonly IIniService _iniService;

    public HomeController(
        ILogger<HomeController> logger,
        IHttpClientFactory clientFactory,
        IConfiguration configuration,
        IIniService iniService)
    {
        _logger = logger;
        _clientFactory = clientFactory;
        _configuration = configuration;
        _iniService = iniService;
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

    public async Task<IActionResult> Config(string InstanceType = "Standalone")
    {
        var iniData = _iniService.GetDefaultIniData(InstanceType);
        if (iniData == null) return View("Error");
        var json = JsonSerializer.Serialize(iniData, new JsonSerializerOptions { WriteIndented = true });
        return View("Config", new ConfigViewModel { JsonConfig = json });
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
