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

        private static bool DownloadFile(string url, string path)
        {
            using HttpClient client = new();
            using Task<Stream> stream = client.GetStreamAsync(url);
            using FileStream fileStream = new(path, FileMode.OpenOrCreate);
            stream.Result.CopyTo(fileStream);
            return stream.IsCompletedSuccessfully;
        }

        // who knew just moving files and subdirectories could be so annoying (probably skill issue on my part)
        private static void MoveFiles(string sourceDirectory, string destinationDirectory)
        {
            // If destination directory doesn't exist, create it
            if (!Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            string[] files;
            try
            {
                files = Directory.GetFiles(sourceDirectory);
            }
            catch (UnauthorizedAccessException e)
            {
                WriteColor($"Access to {sourceDirectory} is denied: {e.Message}", ConsoleColor.Red);
                return;
            }

            // Move each file to the destination directory
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destinationDirectory, fileName);
                File.Move(file, destFile);
            }

            // Get all subdirectories
            string[] subdirectories;
            try
            {
                subdirectories = Directory.GetDirectories(sourceDirectory);
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Access to {sourceDirectory} is denied: {e.Message}");
                return;
            }

            // Recursively call Move for each subdirectory
            foreach (string subdir in subdirectories)
            {
                string subdirName = Path.GetFileName(subdir);
                string destSubdir = Path.Combine(destinationDirectory, subdirName);
                MoveFiles(subdir, destSubdir);
            }
        }

        private static readonly string LatiteFolder =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\RoamingState\LatiteRecode\Plugins";

        private static readonly string UserName = Environment.UserName;

        static int Main(string[] args)
        {
            Console.Title = "CNLSP";

            WriteColor($"Hello, {UserName}!\nThe program will setup your development environment shortly...",
                ConsoleColor.White);

            Thread.Sleep(3000);

            WriteColor("\nWhat do you want the name of the folder of the plugin to be?", ConsoleColor.White);
            Console.Write("> ");

            string? scriptFolderName = Console.ReadLine();
            string scriptFolder = string.Empty;

            if (scriptFolderName != string.Empty)
            {
                if (Regex.IsMatch(scriptFolderName,
                        "^(?!(?:CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])(?:\\.[^.]*)?$)[^<>:\"/\\\\|?*\\x00-\\x1F]*[^<>:\"/\\\\|?*\\x00-\\x1F\\ .]$"))
                {
                    scriptFolder = $"{LatiteFolder}\\{scriptFolderName}";
                    if (!Directory.Exists(scriptFolder))
                    {
                        Directory.CreateDirectory(scriptFolder);
                        Thread.Sleep(1000);
                        WriteColor($"Created folder {scriptFolderName} in {LatiteFolder}!",
                            ConsoleColor.Green);
                    }
                    else if (Directory.Exists(scriptFolder))
                        WriteColor($"Folder {scriptFolderName} already exists!", ConsoleColor.Red);
                }
                else
                {
                    WriteColor(
                        "The folder name you supplied is not compatible with Windows, so the folder name is now \"Example\".",
                        ConsoleColor.Red);
                    scriptFolderName = "Example";
                    scriptFolder = $"{LatiteFolder}\\Example";
                    Directory.CreateDirectory(scriptFolder);
                    Thread.Sleep(1000);
                    WriteColor($"Created folder {scriptFolderName} in {LatiteFolder}!", ConsoleColor.Green);
                }
            }
            else if (scriptFolderName == string.Empty)
            {
                WriteColor("You supplied an empty folder name, so the folder name is now \"Example\". ",
                    ConsoleColor.Red);
                scriptFolderName = "Example";
                scriptFolder = $"{LatiteFolder}\\{scriptFolderName}";
                Directory.CreateDirectory(scriptFolder);
                Thread.Sleep(1000);
                WriteColor($"Created folder {scriptFolderName} in {LatiteFolder}!\n", ConsoleColor.Green);
            }

            DirectoryInfo di = new(scriptFolder);
            FileInfo[] files = di.GetFiles("*");
            if (files.Length == 0)
            {
                Thread.Sleep(1000);
                WriteColor("\nDownloading the Latite Scripting template", ConsoleColor.Yellow);
                if (DownloadFile("https://github.com/LatiteScripting/Template/archive/refs/heads/master.zip",
                        $"{scriptFolder}\\LatiteScriptingTemplate.zip"))
                {
                    Thread.Sleep(1000);
                    WriteColor("Extracting LatiteScriptingTemplate.zip...", ConsoleColor.Yellow);
                    ZipFile.ExtractToDirectory($"{scriptFolder}\\LatiteScriptingTemplate.zip", $"{scriptFolder}");
                    File.Delete($"{scriptFolder}\\LatiteScriptingTemplate.zip");
                    WriteColor("Finished extracting LatiteScriptingTemplate.zip!\n", ConsoleColor.Green);

                    // Move all files from Template-master folder to the actual plugin folder
                    MoveFiles($"{scriptFolder}\\Template-master", scriptFolder);
                    Directory.Delete($"{scriptFolder}\\Template-master", true);

                    Thread.Sleep(1000);
                    WriteColor(
                        $"Finished setting up development environment!\n\nThe script folder location is: {scriptFolder}\n",
                        ConsoleColor.Green);
                    WriteColor("Press any key to exit...", ConsoleColor.Red);
                    Console.ReadLine();
                }
                else
                {
                    WriteColor("Failed to download LatiteScriptingTemplate.zip\n", ConsoleColor.Red);
                    WriteColor("Press any key to exit...", ConsoleColor.Red);
                    Console.ReadLine();
                }
            }
            else
            {
                WriteColor(
                    "Files exist in the script folder directory!\nThe script folder directory MUST be empty to setup the development environment! Press any key to exit...",
                    ConsoleColor.Red);
                Console.ReadLine();
                return 1;
            }

            return 0;
        }
    }
}