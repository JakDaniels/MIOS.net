using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using PhotinoNET;

namespace MIOS.net.Services
{
    internal interface IUiService
    {
        Task UiThread(CancellationToken stoppingToken);
    }

    internal class UiService : IUiService
    {
        private readonly ILogger _logger;

        private readonly IHostApplicationLifetime _applicationLifetime;

        private readonly IConfiguration _configuration;

        private Thread? _thread;
        
        public UiService(
            ILogger<UiService> logger,
            IHostApplicationLifetime applicationLifetime,
            IConfiguration configuration)
        {
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _configuration = configuration;
        }

        public async Task UiThread(CancellationToken stoppingToken)
        {
            _thread = new Thread(UIThreadStart)
            {
                IsBackground = true
            };
            _thread.TrySetApartmentState(ApartmentState.STA);
            _thread.Start();
        }

        private void UIThreadStart()
        {
            bool exitServerOnUiClose    = _configuration.GetValue<bool>("exit", true);
            int listenPort              = _configuration.GetValue<int>("port", 5000);
            bool allowDebugConsole      = _configuration.GetValue<bool>("debug", false);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Starting UI on port " + listenPort.ToString());
            Console.ResetColor();
            var window = new PhotinoWindow()
                .SetTitle("MIOS.net")
                .SetUseOsDefaultSize(false)
                .SetSize(new Size(1000,800))
                .Center()
                .SetDevToolsEnabled(allowDebugConsole)
                .Load(new Uri("http://localhost:" + listenPort.ToString() + "/"));

            window.WaitForClose();
            if(exitServerOnUiClose) _applicationLifetime.StopApplication();
        }
    }
}