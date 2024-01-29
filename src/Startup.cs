using Microsoft.AspNetCore.Builder;
using MIOS.net.Services;


namespace MIOS.net{
    public class Startup
    {
        public Startup (
            IConfiguration configuration,
            IWebHostEnvironment env
            )
        {
            _configuration = configuration;
            _environment = env;

        }

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            bool useUi = _configuration.GetValue<bool>("UseUi", true);
            if(useUi)
            {
                services.AddHostedService<ConsumeScopedServiceHostedService>();
                services.AddScoped<IUiService, UiService>();
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Ui}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}