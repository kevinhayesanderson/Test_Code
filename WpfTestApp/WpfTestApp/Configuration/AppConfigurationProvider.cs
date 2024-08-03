using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace WpfTestApp.Configuration
{
    public class AppConfigurationProvider : ConfigurationProvider
    {
        private string _appsettingsfile;

        private Settings? settings;

        public AppConfigurationProvider(string appsettingsfile)
        {
            _appsettingsfile = appsettingsfile;
        }

        public override void Load()
        {
            var localAppDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppDomain.CurrentDomain.FriendlyName);
            if (!Directory.Exists(localAppDataFolder))
            {
                Directory.CreateDirectory(localAppDataFolder);
                var destinationFolder = Path.Combine(localAppDataFolder, _appsettingsfile);
                File.Copy(_appsettingsfile, destinationFolder);
                _appsettingsfile = destinationFolder;
            }
            string jsonString = File.ReadAllText(_appsettingsfile);
            settings = JsonSerializer.Deserialize<Settings>(jsonString)!;
            Data = settings == null ? settings?.ToDictionary(StringComparer.OrdinalIgnoreCase) : CreateAndSaveDefaultValues();
        }

        public bool Persist()
        {
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.WriteIndented = true;
            string jsonString = JsonSerializer.Serialize(settings, jsonSerializerOptions);
            File.WriteAllText(_appsettingsfile, jsonString);
            return true;
        }

        public ref Settings? GetSettings() => ref settings;

        private static IDictionary<string, string> CreateAndSaveDefaultValues()
        {
            return Settings.DefaultValues;
        }
    }
}