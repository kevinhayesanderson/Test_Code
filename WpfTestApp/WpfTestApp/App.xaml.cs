using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Windows;
using WpfTestApp.Configuration;

namespace WpfTestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? Host { get; private set; }

        public IServiceProvider? ServiceProvider { get; private set; }

        public IConfigurationRoot? ConfigurationRoot { get; private set; }

        public App()
        {
            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();
                    ConfigurationRoot = configuration
                    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddApplicationConfiguration("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();
                })
                .ConfigureServices((context, services) =>
                {
                    if (ConfigurationRoot != null)
                    {
                        var appConfigurationProvider = ConfigurationRoot.Providers.OfType<AppConfigurationProvider>().FirstOrDefault();
                        var settings = appConfigurationProvider?.GetSettings();
                        if (settings != null)
                        {
                            settings.PropertyChanged += Settings_PropertyChanged;
                            services.AddSingleton(settings);
                        }
                    }
                    services.AddSingleton<MainWindow>();
                    ServiceProvider = services.BuildServiceProvider();
                })
                .Build();
        }

        private void Settings_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ConfigurationRoot != null)
            {
                foreach (var provider in ConfigurationRoot.Providers)
                {
                    if (provider is AppConfigurationProvider appConfigurationProvider)
                    {
                        appConfigurationProvider.Persist();
                    }
                }
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await Host!.StartAsync();
            var mainWindow = Host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await Host!.StopAsync();
            base.OnExit(e);
        }
    }
}