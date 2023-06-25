using System.IO.Compression;
using System.Text.RegularExpressions;

namespace CNLSP
{
    internal class Program
    {
        public static void WriteColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void DownloadFile(string url, string path)
        {
            using HttpClient client = new();
            using Task<Stream> stream = client.GetStreamAsync(url);
            using FileStream fileStream = new(path, FileMode.OpenOrCreate);
            stream.Result.CopyTo(fileStream);
            if (stream.IsCompletedSuccessfully)
                WriteColor("Downloaded file(s) successfully!", ConsoleColor.Green);
        }

        private static readonly string LatiteFolder =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\RoamingState\Latite";

        private static readonly string UserName = Environment.UserName;

        static int Main(string[] args)
        {
            WriteColor($"Hello, {UserName}!\nThe program will setup your development environment shortly...",
                ConsoleColor.White);

            Thread.Sleep(3000);

            WriteColor("\nChecking if ChakraCore.dll is in your Latite folder...", ConsoleColor.Yellow);

            Thread.Sleep(1000);

            if (File.Exists($"{LatiteFolder}\\ChakraCore.dll"))
                WriteColor("ChakraCore.dll is in the Latite folder!", ConsoleColor.Green);
            else
            {
                WriteColor("ChakraCore.dll is not in the Latite folder! Downloading ChakraCore.dll...",
                    ConsoleColor.Red);
                DownloadFile("https://latite-client.is-from.space/r/ChakraCore.dll", $"{LatiteFolder}\\ChakraCore.dll");
            }

            Thread.Sleep(1000);

            WriteColor("\nWhat do you want the name of the folder of the script to be?", ConsoleColor.White);
            Console.Write("> ");

            string? scriptFolderName = Console.ReadLine();
            string scriptFolder = string.Empty;

            if (scriptFolderName != string.Empty)
            {
                if (Regex.IsMatch(scriptFolderName,
                        "^(?!(?:CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])(?:\\.[^.]*)?$)[^<>:\"/\\\\|?*\\x00-\\x1F]*[^<>:\"/\\\\|?*\\x00-\\x1F\\ .]$"))
                {
                    scriptFolder = $"{LatiteFolder}\\Scripts\\{scriptFolderName}";
                    if (!Directory.Exists(scriptFolder))
                    {
                        Directory.CreateDirectory(scriptFolder);
                        Thread.Sleep(1000);
                        WriteColor($"Created folder {scriptFolderName} in {LatiteFolder}\\Scripts!",
                            ConsoleColor.Green);
                    }
                    else if (Directory.Exists(scriptFolder))
                        WriteColor($"Folder {scriptFolderName} already exists in {LatiteFolder}!", ConsoleColor.Red);
                }
                else
                {
                    WriteColor(
                        "The folder name you supplied is not compatible with Windows, so the folder name is now \"Example\".",
                        ConsoleColor.Red);
                    scriptFolderName = "Example";
                    scriptFolder = $"{LatiteFolder}\\Scripts\\Example";
                    Directory.CreateDirectory(scriptFolder);
                    Thread.Sleep(1000);
                    WriteColor($"Created folder {scriptFolderName} in {LatiteFolder}\\Scripts!", ConsoleColor.Green);
                }
            }
            else if (scriptFolderName == string.Empty)
            {
                WriteColor("You supplied an empty folder name, so the folder name is now \"Example\". ",
                    ConsoleColor.Red);
                scriptFolderName = "Example";
                scriptFolder = $"{LatiteFolder}\\Scripts\\Example";
                Directory.CreateDirectory(scriptFolder);
                Thread.Sleep(1000);
                WriteColor($"Created folder {scriptFolderName} in {LatiteFolder}\\Scripts!", ConsoleColor.Green);
            }

            DirectoryInfo di = new(scriptFolder);
            FileInfo[] files = di.GetFiles("*");
            if (files.Length == 0)
            {
                Thread.Sleep(1000);
                WriteColor("\nDownloading zip file for example scripting files...", ConsoleColor.Yellow);
                DownloadFile("https://latite-client.is-from.space/r/NewLatiteScriptingProject.zip",
                    $"{scriptFolder}\\files.zip");

                Thread.Sleep(1000);
                WriteColor("Extracting files.zip...", ConsoleColor.Yellow);
                ZipFile.ExtractToDirectory($"{scriptFolder}\\files.zip", $"{scriptFolder}");
                WriteColor("Finished extracting files.zip!", ConsoleColor.Green);

                Thread.Sleep(1000);
                WriteColor(
                    $"Finished setting up development environment!\nThe script folder location is: {scriptFolder}",
                    ConsoleColor.Green);
                WriteColor("Press any key to exit...", ConsoleColor.Red);
                Console.ReadLine();
                Thread.Sleep(5000);
            }
            else
            {
                WriteColor(
                    "Files exist in the script folder directory!\nThe script folder directory MUST be empty to setup the development environment! Press any key to exit...",
                    ConsoleColor.Red);
                Console.ReadLine();
                Thread.Sleep(5000);
                return 1;
            }

            return 0;
        }
    }
}