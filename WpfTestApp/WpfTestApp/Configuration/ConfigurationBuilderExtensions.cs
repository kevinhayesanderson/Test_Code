using WpfTestApp.Configuration;

namespace Microsoft.Extensions.Configuration;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddApplicationConfiguration(this IConfigurationBuilder builder, string appsettingsfile)
    {
        return builder.Add(new AppConfigurationSource(appsettingsfile));
    }
}