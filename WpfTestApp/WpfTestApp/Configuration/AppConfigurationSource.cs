using Microsoft.Extensions.Configuration;

namespace WpfTestApp.Configuration
{
    public class AppConfigurationSource : IConfigurationSource
    {
        private readonly string _appsettingsfile;

        public AppConfigurationSource(string appsettingsfile) =>
            _appsettingsfile = appsettingsfile;

        public IConfigurationProvider Build(IConfigurationBuilder builder) => new AppConfigurationProvider(_appsettingsfile);
    }
}