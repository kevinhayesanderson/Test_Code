using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfTestApp.Configuration
{
    public class Settings : INotifyPropertyChanged
    {
        public static readonly ImmutableDictionary<string, string> DefaultValues = new Dictionary<string, string>()
        {
            ["SamZone"] = "260000000",
            ["SamZoneBacon"] = "150000000",
            ["SamcefFolder"] = @"C:\Samtech\Samcef\2021.2-1992_i4_x64",
            ["LicenseHost"] = "WS004527",
            ["BearinxFolder"] = @"K:\Bearinx\Bearinx",
            ["BearinxFolder_x64"] = @"K:\Bearinx\Bearinx",
            ["SimpackFolder"] = @"C:\ProgramFiles\SIMPACK-9.9.2",
            ["EnableScriptLogging"] = "False",
            ["KeepBearinxDlls"] = "True",
            ["FontSize"] = "12",
            ["PythonFolder"] = @"C:\Anaconda",
            ["UseConda"] = "False",
            ["CondaEnvironmentName"] = "",
            ["ForcePythonShutdown"] = "False",
            ["LogToFilePath"] = "",
            ["IsLogToFileEnabled"] = "False",
            ["SimcenterFolder"] = @"C:\ProgramFiles\Siemens\Simcenter3D_2021.2",
            ["SamcefLicenseManager"] = "LicenseMonitor",
            ["FlexLMLicenseServer"] = "28000@de012512.schaeffler.com",
            ["UGSLicenseServer"] = "28000@ws004527.schaeffler.com"
        }.ToImmutableDictionary();

        private string bearinxFolder = string.Empty;
        private string bearinxFolder_x64 = string.Empty;
        private string condaEnvironmentName = string.Empty;
        private string enableScriptLogging = string.Empty;
        private string flexLMLicenseServer = string.Empty;
        private string fontSize = string.Empty;
        private string forcePythonShutdown = string.Empty;
        private string isLogToFileEnabled = string.Empty;
        private string keepBearinxDlls = string.Empty;
        private string licenseHost = string.Empty;
        private string logToFilePath = string.Empty;
        private string pythonFolder = string.Empty;
        private string samcefFolder = string.Empty;
        private string samcefLicenseManager = string.Empty;
        private string samZone = string.Empty;
        private string samZoneBacon = string.Empty;
        private string simcenterFolder = string.Empty;
        private string simpackFolder = string.Empty;
        private string uGSLicenseServer = string.Empty;
        private string useConda = string.Empty;
        private string panelLayout = string.Empty;


        public event PropertyChangedEventHandler? PropertyChanged;

        public string BearinxFolder
        {
            get => bearinxFolder;
            set
            {
                if (value != bearinxFolder)
                {
                    bearinxFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string BearinxFolder_x64
        {
            get => bearinxFolder_x64;
            set
            {
                if (value != bearinxFolder_x64)
                {
                    bearinxFolder_x64 = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string CondaEnvironmentName
        {
            get => condaEnvironmentName;
            set
            {
                if (value != condaEnvironmentName)
                {
                    condaEnvironmentName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string EnableScriptLogging
        {
            get => enableScriptLogging;
            set
            {
                if (value != enableScriptLogging)
                {
                    enableScriptLogging = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string FlexLMLicenseServer
        {
            get => flexLMLicenseServer;
            set
            {
                if (value != flexLMLicenseServer)
                {
                    flexLMLicenseServer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string FontSize
        {
            get => fontSize;
            set
            {
                if (value != fontSize)
                {
                    fontSize = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string ForcePythonShutdown
        {
            get => forcePythonShutdown;
            set
            {
                if (value != forcePythonShutdown)
                {
                    forcePythonShutdown = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string IsLogToFileEnabled
        {
            get => isLogToFileEnabled;
            set
            {
                if (value != isLogToFileEnabled)
                {
                    isLogToFileEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string KeepBearinxDlls
        {
            get => keepBearinxDlls;
            set
            {
                if (value != keepBearinxDlls)
                {
                    keepBearinxDlls = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string LicenseHost
        {
            get => licenseHost;
            set
            {
                if (value != licenseHost)
                {
                    licenseHost = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string LogToFilePath
        {
            get => logToFilePath;
            set
            {
                if (value != logToFilePath)
                {
                    logToFilePath = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string PythonFolder
        {
            get => pythonFolder;
            set
            {
                if (value != pythonFolder)
                {
                    pythonFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SamcefFolder
        {
            get => samcefFolder;
            set
            {
                if (value != samcefFolder)
                {
                    samcefFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SamcefLicenseManager
        {
            get => samcefLicenseManager;
            set
            {
                if (value != samcefLicenseManager)
                {
                    samcefLicenseManager = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SamZone
        {
            get => samZone;
            set
            {
                if (value != samZone)
                {
                    samZone = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SamZoneBacon
        {
            get => samZoneBacon;
            set
            {
                if (value != samZoneBacon)
                {
                    samZoneBacon = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SimcenterFolder
        {
            get => simcenterFolder;
            set
            {
                if (value != simcenterFolder)
                {
                    simcenterFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string SimpackFolder
        {
            get => simpackFolder;
            set
            {
                if (value != simpackFolder)
                {
                    simpackFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string UGSLicenseServer
        {
            get => uGSLicenseServer;
            set
            {
                if (value != uGSLicenseServer)
                {
                    uGSLicenseServer = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string UseConda
        {
            get => useConda;
            set
            {
                if (value != useConda)
                {
                    useConda = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string PanelLayout
        {
            get => panelLayout;
            set
            {
                if (value != panelLayout)
                {
                    panelLayout = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public static bool TryGetDefault(string key, out string outValue)
        {
            outValue = string.Empty;
            if (DefaultValues.TryGetValue(key, out var value))
            {
                outValue = value;
                return true;
            }
            return false;
        }

        public bool TrySetDefault(string key)
        {
            if (TryGetDefault(key, out string defaultValue))
            {
                GetType().GetProperty(key)?.SetValue(this, defaultValue, null);
                return true;
            }
            return false;
        }

        internal IDictionary<string, string> ToDictionary(StringComparer ordinalIgnoreCase)
        {
            return new Dictionary<string, string>(ordinalIgnoreCase)
            {
                [nameof(BearinxFolder)] = BearinxFolder,
                [nameof(BearinxFolder_x64)] = BearinxFolder_x64,
                [nameof(CondaEnvironmentName)] = CondaEnvironmentName,
                [nameof(EnableScriptLogging)] = EnableScriptLogging,
                [nameof(FlexLMLicenseServer)] = FlexLMLicenseServer,
                [nameof(FontSize)] = FontSize,
                [nameof(ForcePythonShutdown)] = ForcePythonShutdown,
                [nameof(IsLogToFileEnabled)] = IsLogToFileEnabled,
                [nameof(KeepBearinxDlls)] = KeepBearinxDlls,
                [nameof(LicenseHost)] = LicenseHost,
                [nameof(LogToFilePath)] = LogToFilePath,
                [nameof(PythonFolder)] = PythonFolder,
                [nameof(SamcefFolder)] = SamcefFolder,
                [nameof(SamcefLicenseManager)] = SamcefLicenseManager,
                [nameof(SamZone)] = SamZone,
                [nameof(SamZoneBacon)] = SamZoneBacon,
                [nameof(SimcenterFolder)] = SimcenterFolder,
                [nameof(SimpackFolder)] = SimpackFolder,
                [nameof(UGSLicenseServer)] = UGSLicenseServer,
                [nameof(UseConda)] = UseConda
            };
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Windowplacement Windowplacement { get; set; } = null!;


    }
    public sealed class Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Rect(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
    }


    public sealed class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }


    public sealed class Windowplacement
    {
        public int Length { get; set; }
        public int Flags { get; set; }
        public int ShowCmd { get; set; }
        public Point? MinPosition { get; set; }
        public Point? MaxPosition { get; set; }
        public Rect? NormalPosition { get; set; }
    }
}