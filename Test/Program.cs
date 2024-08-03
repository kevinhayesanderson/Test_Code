using BenchmarkDotNet.Attributes;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public interface IProgram
    {
        event Action Started;

        event Action<string> StatusUpdated;
    }

    // See http://msdn.microsoft.com/msdnmag/issues/05/10/Reliability/ for more about safe handles.
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLibraryHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            return NativeLibrary.FreeLibrary(handle);
        }
    }

    public static class NativeLibrary
    {
        private const string s_kernel = "kernel32";

        [DllImport(s_kernel, CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport(s_kernel, CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
        public static extern void SetDllDirectory(string lpPathName);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public static string GetLibraryPathname(string filename)
        {
            // If 64-bit process, load 64-bit DLL
            bool is64bit = System.Environment.Is64BitProcess;

            string prefix = "Win32";

            if (is64bit)
            {
                prefix = "x64";
            }

            var lib1 = prefix + @"\" + filename;

            return lib1;
        }
    }

    public class BenchMarkTestClass
    {
        private readonly long LongNum = 9223372036854775807;

        [Benchmark]
        public string LongToString1()
        {
            return LongNum.ToString();
        }

        [Benchmark]
        public string LongToString2()
        {
            return Convert.ToString(LongNum);
        }

        [Benchmark]
        public string LongToString3()
        {
            return string.Format("{0}", LongNum);
        }

        [Benchmark]
        public string LongToString4()
        {
            return $"{LongNum}";
        }

        [Benchmark]
        public string LongToString5()
        {
            return "" + LongNum;
        }

        [Benchmark]
        public string LongToString6()
        {
            return new StringBuilder().Append(LongNum).ToString();
        }

        [Benchmark]
        public string LongToString7()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(long));
            return (string)converter.ConvertTo(LongNum, typeof(string));
        }

        public BenchMarkTestClass()
        {
        }
    }

    internal class Program : IProgram
    {
        public event Action Started;

        public event Action<string> StatusUpdated;

        public static void AddDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            DirectoryInfo dInfo = new DirectoryInfo(FileName);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
            dInfo.SetAccessControl(dSecurity);
        }

        public static string doubleInfo(string actual, string expected)
        {
            StringBuilder info = new StringBuilder();
            info.AppendLine($"Actual:{actual}");
            info.AppendLine($"expected:{expected}");
            int noOfDecimalActual = actual.ToString().Split('.')[1].Length;
            int noOfDecimalExpected = expected.ToString().Split('.')[1].Length;
            int length = Math.Max(noOfDecimalActual, noOfDecimalExpected);
            info.AppendLine($"No of decimal places in actual:{noOfDecimalActual}");
            info.AppendLine($"No of decimal places in expected:{noOfDecimalExpected}");
            var arrActual = actual.ToString().Split('.')[1].ToCharArray();
            var arrExpected = expected.ToString().Split('.')[1].ToCharArray();
            info.AppendLine($"The different digits are at decimal places");
            for (int i = 0; i < length; i++)
            {
                if (i < arrActual.Length && i < arrExpected.Length)
                {
                    if (arrActual[i] != arrExpected[i])
                        _ = info.AppendLine($"{i + 1}\ta:{arrActual.GetValue(i)}\te:{arrExpected.GetValue(i)}");
                }
                else
                {
                    var actualVal = (i < arrActual.Length) ? arrActual.GetValue(i) : string.Empty;
                    var expectedVal = (i < arrExpected.Length) ? arrExpected.GetValue(i) : string.Empty;
                    if (actualVal != expectedVal)
                        _ = info.AppendLine($"{i + 1}\ta:{actualVal}\te:{expectedVal}");
                }
            }
            double actualD = double.Parse(actual);
            double expectedD = double.Parse(expected);
            var absTol = actualD > expectedD ? actualD - expectedD : expectedD - actualD;
            var relTol = actualD > expectedD ? ((actualD - expectedD) / expectedD) : ((expectedD - actualD) / actualD);
            info.AppendLine("||Actual Tolerance||");
            info.AppendLine($"Absolute Tolerance: {absTol.ToString()}");
            info.AppendLine($"Relative Tolerance: {relTol.ToString()}");
            info.AppendLine("||Suggested Tolerances||");
            info.AppendLine($"Absolute Tolerance: 1{absTol.ToString("E0", System.Globalization.CultureInfo.InvariantCulture).Split('E')[1]}");
            info.AppendLine($"Relative Tolerance: 1{relTol.ToString("E0", System.Globalization.CultureInfo.InvariantCulture).Split('E')[1]}");
            return info.ToString();
        }

        public static string GetUniversalName(string localPath)
        {
            const int UNIVERSAL_NAME_INFO_LEVEL = 0x00000001;
            const int ERROR_MORE_DATA = 234;
            const int NOERROR = 0;
            string retVal = string.Empty;
            IntPtr buffer = IntPtr.Zero;
            try
            {
                int size = 0;
                int apiRetVal = WNetGetUniversalName(localPath, UNIVERSAL_NAME_INFO_LEVEL, (IntPtr)IntPtr.Size, ref size);
                if (apiRetVal == ERROR_MORE_DATA)
                {
                    buffer = Marshal.AllocCoTaskMem(size);
                    apiRetVal = WNetGetUniversalName(localPath, UNIVERSAL_NAME_INFO_LEVEL, buffer, ref size);
                    if (apiRetVal == NOERROR)
                    {
                        retVal = Marshal.PtrToStringAuto(new IntPtr(buffer.ToInt64() + IntPtr.Size), size);
                        retVal = retVal.Substring(0, retVal.IndexOf('\0'));
                    }
                }
            }
            catch
            {
                retVal = string.Empty;
            }
            finally
            {
                Marshal.FreeCoTaskMem(buffer);
            }
            return retVal;
        }

        public static bool isCardenAngle(double dval)
        {
            return Math.Abs((int)dval) <= 90;
        }

        public static bool isEqual(double dVal1, double dVal2, double dPrecision = 1e-6)
        {
            if (Math.Abs(dVal1 - dVal2) < dPrecision || Math.Abs(dVal1 - dVal2) < Math.Abs(dVal1) * dPrecision)
                return true;
            return false;
        }

        public static bool isZero(double dval, double dPrecision = 1e-6)
        {
            if (dval.Equals(double.NaN) || dval.Equals(double.NegativeInfinity) || dval.Equals(double.PositiveInfinity))
            {
                return false;
            }
            return Math.Abs(dval) < dPrecision;
        }

        public static bool isZero1(double dval, double dPrecision = 1e-6)
        {
            return Math.Abs(dval) < dPrecision;
        }

        public static bool checkEncoding(string value, Encoding encoding)
        {
            bool retCode;
            var charArray = value.ToCharArray();
            byte[] bytes = new byte[charArray.Length];
            for (int i = 0; i < charArray.Length; i++)
            {
                bytes[i] = (byte)charArray[i];
            }
            retCode = string.Equals(encoding.GetString(bytes, 0, bytes.Length), value, StringComparison.InvariantCulture);
            return retCode;
        }

        public static void Main(string[] args)
        {
            #region Commented out code

            //var path = @"C:\Users\hayeskev\source\Models\02_LavalRotor";
            //var validExtenstions = new string[] {".sdf", ".sqlite"};
            //var files = Directory.GetFiles(path).Where(file  => validExtenstions.Contains(new FileInfo(file).Extension)).Select(file =>file).ToList();

            //Started += Program_Started;

            //string no = " 64.";
            //double dval = double.Parse(no);

            //var modelFile = @"C:\Users\hayeskev\source\Models\Issue_5996_Caba\SearchPath\001_FC010_n43000_Ring0.ca3";
            //SetVariableValue(modelFile, "ID_MEMBER_EXPORT_UUID", Guid.NewGuid().ToString());

            //if (args.Length > 0)
            //{
            //    var expected = args[0].Split(":")[1];
            //    var actual = args[1].Split(":")[1];
            //    Console.WriteLine(doubleInfo(actual, expected));
            //}

            //RunProcess("", "", "");

            //Console.WriteLine(isEqual(3, 4));
            // Console.WriteLine(isEqual(8.999999999999999E+001, 9.000000000000001E+001));
            //Console.WriteLine(isEqual(8.999999999999999E+001, 9.000000000000001E+001, 1E-16));
            //Console.WriteLine(isZero(0));
            //Console.WriteLine(isZero(0.00));
            //Console.WriteLine(isZero(0.000000000000000));
            //Console.WriteLine(isZero(1E-16));
            //Console.WriteLine(isZero(0.0000000000000001E-001));
            //Console.WriteLine(isZero(0.0000000000000001));
            //Console.WriteLine(isZero(0.0000000000000001, 1E-16));
            //Console.WriteLine(isZero(-0.0000000000000001, 1E-16));
            //Console.WriteLine(isZero(-1E-16));
            //Console.WriteLine(isZero(-1E16));
            //Console.WriteLine(isZero(1E-16));
            //Console.WriteLine(isZero(1E16));
            //Console.WriteLine(isZero(-2.220446049250313E-16));//true
            //Console.WriteLine(isZero(-2.220446049250313E16));//false
            //Console.WriteLine(isZero(2.220446049250313E-16));//true
            //Console.WriteLine(isZero(2.220446049250313E16));//false
            //isZero1(-1E-16);
            //Console.WriteLine(isZero1(-1E-16));
            //Console.WriteLine(isZero1(-1E16));
            //Console.WriteLine(isZero1(1E-16));
            //Console.WriteLine(isZero1(1E16));

            //Console.WriteLine(isCardenAngle(-1.800000000000000E+002));
            //Console.WriteLine(isCardenAngle(8.999999999999999E+001));
            //Console.WriteLine(isCardenAngle(2.220446049250313E-017));

            //string s1 = "14411,14412,20505";
            //string s2 = "14411,14412,20505";
            //int h1 = s1.GetHashCode();
            //int h2 = s2.GetHashCode();
            //Console.WriteLine($"{h1}");
            //Console.WriteLine($"{h2}");

            //_coobjLib.GetUnmanagedFunction = new UnmanagedLibrary(Path.Combine("Coobject.dll"));
            //Console.WriteLine(roundToPrecision(0.0799999999999999));

            //Console.WriteLine(doubleInfo(0.0799999999999999, 0.0800000000000001));

            //FileIOPermission fileIOPermission = new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, tempFile);
            //try
            //{
            //    fileIOPermission.Assert();
            //    fileIOPermission.Demand();
            //}
            //catch (SecurityException ex)
            //{
            //    Document.logError($"Couldn't delete file {tempFile}: {ex.Message}");
            //}

            ////File.SetAttributes(tempFile, FileAttributes.Normal);
            ////File.Delete(tempFile);
            //if (File.Exists(tempFile) && !DeleteFile(tempFile))
            //{
            //    Document.logError($"Couldn't delete file {tempFile}");
            //}

            //StringBuilder toWrite = new StringBuilder();

            //int textLength = unicodeEncoding.GetByteCount(toWrite.ToString());

            //using (FileStream fs = new FileStream(startCalcBatPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            //{
            //    fs.Write(unicodeEncoding.GetBytes(toWrite.ToString()), 0, textLength);
            //}
            //var userDomainName = Environment.UserDomainName;
            //var userName = Environment.UserName;
            //var account = $@"{userDomainName}\{userName}";
            //AddDirectorySecurity(modelFile, account, FileSystemRights.Read | FileSystemRights.Write | FileSystemRights.Delete, AccessControlType.Allow);
            //RemoveDirectorySecurity(tempFile, account, FileSystemRights.Read | FileSystemRights.Write | FileSystemRights.Delete, AccessControlType.Deny);

            //var preFrmEmpty = new PreFrmCommon.Empty();

            //var dllToLoad = @"K:\Bearinx\Bearinx\Bin\DynMath.dll";
            //NativeLibrary.SetDllDirectory(Path.GetDirectoryName(dllToLoad));
            //IntPtr m_hLibrary = NativeLibrary.LoadLibrary(dllToLoad);
            //if (m_hLibrary == IntPtr.Zero)
            //{
            //    int hr = Marshal.GetHRForLastWin32Error();
            //    Marshal.ThrowExceptionForHR(hr);
            //}
            //Console.WriteLine(m_hLibrary.ToString());

            //var checkText1 = "Kevin Hayes Anderson";
            //var checkText2 = @"C:\Users\hayeskev\source\Models\Issue_6064_Ascii_test\Prüfung";
            //Console.WriteLine(checkEncoding(checkText1, Encoding.ASCII));
            //Console.WriteLine(checkEncoding(checkText2, Encoding.ASCII));
            //Console.ReadKey();

            //var path = @"C:\Program Files\Microsoft Visual Studio\2022\**\Common7\IDE\";
            //var retCode = Directory.Exists(path);

            //BENCHMARK.NET
            //var summary = BenchmarkRunner.Run<BenchMarkTestClass>();

            //CancellationTokenSource cts = new CancellationTokenSource();
            //ThreadPool.QueueUserWorkItem(new WaitCallback(DoSomeWork), cts.Token);
            //Thread.Sleep(2500);

            //// Request cancellation.
            //cts.Cancel();
            //Console.WriteLine("Cancellation set in token source...");
            //Thread.Sleep(2500);
            //// Cancellation should have happened, so call Dispose.
            //cts.Dispose();

            //UInt64 totalMemory = default;
            //ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            //ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(objectQuery);
            //ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
            //foreach (var managementObject in managementObjectCollection)
            //{
            //    totalMemory = (UInt64)managementObject["TotalVisibleMemorySize"];

            //}
            //double factor = 0.03;
            //Console.WriteLine($"IntMax :{int.MaxValue}");
            //Console.WriteLine($"TM1    :{totalMemory * 1000}");
            //Console.WriteLine($"TM1*f  :{totalMemory * 1000 * factor}");
            //Console.WriteLine($"Total Memory 1_int:{Convert.ToInt32(totalMemory*1000)}");

            //if NET6_0
            //var gCMemoryInfo = GC.GetGCMemoryInfo();
            //Console.WriteLine($"TM2    :{gCMemoryInfo.TotalAvailableMemoryBytes}");
            //Console.WriteLine($"TM2*f  :{gCMemoryInfo.TotalAvailableMemoryBytes * factor}");
            //#else
            //long memKb;
            //NativeLibrary.GetPhysicallyInstalledSystemMemory(out memKb);
            //Console.WriteLine($"TM2    :{GC.GetTotalMemory(true)}");
            //Console.WriteLine($"TM2*f  :{memKb * 1000 * factor}");
            //new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory
            //#endif

            //Console.WriteLine($"Total Memory 1_int:{Convert.ToInt32(gCMemoryInfo.TotalAvailableMemoryBytes)}");

            //var type = typeof(BenchMarkTestClass);
            //var constrInfo = type.GetConstructor(new Type[] {  });
            //Console.WriteLine(constrInfo);

            #endregion Commented out code

            var url1 = @"https://sconnect.schaeffler.com/groups/bearinx-simulation-suite-information-platform";
            var url2 = @"https://sconnect.schaeffler.com/groups/advanced-multibody-dynamics-simpla-and-other-simulation-technologies";
            var url3 = @"https://sconnect.schaeffler.com/groups/tutorial-videos-by-bearing-analysis-tools?invite=false";
            var url4 = @"https://sconnect.schaeffler.com/groups/tutorial-videos-by-bearing-analysis-tools";
            var res1 = URL_Test2(url1);
            Console.WriteLine(res1);
            var res2 = URL_Test1(url2);
            Console.WriteLine(res2);
            var res3 = URL_Test1(url3);
            Console.WriteLine(res3);
            var res4 = URL_Test1(url4);
            Console.WriteLine(res4);
        }

        public static async Task<bool> URL_Test(string url)
        {
            var checkingResponse = await new HttpClient().GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            return checkingResponse.IsSuccessStatusCode;
        }

        public static bool URL_Test1(string url)
        {
            var checkingResponse = new HttpClient().GetAsync(url, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
            return checkingResponse.IsSuccessStatusCode;
        }

        public static bool URL_Test2(string url)
        {
            var uri = new Uri(url);
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = uri
                };
                var httpClient = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
                var checkingResponse = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();
                return checkingResponse.IsSuccessStatusCode;
            }
            catch (SocketException)
            {
                var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.ConnectAsync(uri.Host, uri.Port);
                var isConnected = socket.Connected;
                return isConnected;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void DoSomeWork(object obj)
        {
            CancellationToken token = (CancellationToken)obj;

            for (int i = 0; i < 100000; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("In iteration {0}, cancellation has been requested...",
                                      i + 1);
                    // Perform cleanup if necessary.
                    //...
                    // Terminate the operation.
                    break;
                }
                // Simulate some work.
                Thread.SpinWait(500000);
            }
        }

        public static void Program_Started()
        {
            Console.WriteLine("Started");
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool DeleteFile(string path);

        public static void RemoveDirectorySecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            DirectoryInfo dInfo = new DirectoryInfo(FileName);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.RemoveAccessRule(new FileSystemAccessRule(Account, Rights, ControlType));
            dInfo.SetAccessControl(dSecurity);
        }

        //public static double roundToPrecision(double value, double dPrecision = 1E-15)
        //{
        //    double round1 = Math.Round(value, 15, MidpointRounding.ToEven);
        //    double round2 = Math.Round(value, 15, MidpointRounding.AwayFromZero);
        //    double res = Math.MaxMagnitude(round1, round2);
        //    return res;
        //}

        public static void SetVariableValue(string modelFile, string variableName, string value)
        {
            string[] lines = File.ReadAllLines(modelFile);
            var variableLines = lines.Where(line => line.Contains($"VARIABLE {variableName}")).Select(line => line).ToList();
            for (int i = 0; i < variableLines.Count; i++)
            {
                int index = Array.IndexOf(lines, variableLines[i]);
                var match = Regex.Match(variableLines[i], @"(?<=\"").*(?=\"")");
                variableLines[i] = (!string.IsNullOrEmpty(match.Value)) ? variableLines[i].Replace(match.Value, value) : variableLines[i].Replace(@"""""", $@"""{value}""");
                lines[index] = variableLines[i];
            }
            File.WriteAllLines(modelFile, lines, Encoding.UTF8);
        }

        public void RunProcess(string fileName, string args, string startDir)
        {
            fileName = @"C:\Users\hayeskev\source\Models\Issue 5996_Caba Calculation\test.bat";
            //StringBuilder log = new StringBuilder();
            //var processInfo = new ProcessStartInfo("cmd.exe", $@"/c ""{fileName}""");
            var processInfo = new ProcessStartInfo();
            processInfo.FileName = fileName;
            if (string.IsNullOrEmpty(args))
            {
                processInfo.Arguments = string.Empty;
            }
            if (string.IsNullOrEmpty(startDir))
            {
                processInfo.WorkingDirectory = startDir;
            }
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            processInfo.ErrorDialog = false;
            processInfo.StandardOutputEncoding = Encoding.UTF8;
            processInfo.StandardErrorEncoding = Encoding.UTF8;
            var timer = new Stopwatch();
            var process = Process.Start(processInfo);
            if (process != null)
            {
                process.EnableRaisingEvents = true;
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null) // maybe not needed
                    {
                        Console.WriteLine(e.Data);
                        //log.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null) // maybe not needed
                    {
                        Console.WriteLine(e.Data);
                        //log.AppendLine(e.Data);
                    }
                };
                process.Exited += (sender, e) => // async way to know wheather the process is exited
                {
                    timer.Stop();
                    process.CancelOutputRead();
                    process.CancelErrorRead();
                    var exitCode = process.ExitCode;
                    Console.WriteLine($"ExitCode: {exitCode}");
                    Console.WriteLine($"Process completed after {timer.ElapsedMilliseconds}");
                    //log.AppendLine($"ExitCode: {exitCode}");
                    process.Close();
                };
                try
                {
                    timer.Start();
                    process.Start();
                    Console.WriteLine($"Started {process.ProcessName} with pid {process.Id}");
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to start process {fileName}: {ex.Message}");
                    //throw;
                }
                //process.WaitForExit(); // synchronous wait for process to exit
            }
            //Console.WriteLine("Writing log:");
            //Console.WriteLine(log.ToString());
        }

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.U4)]
        private static extern int WNetGetUniversalName(string lpLocalPath,
                                               [MarshalAs(UnmanagedType.U4)] int dwInfoLevel,
                                               IntPtr lpBuffer,
                                               [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize);
    }
}