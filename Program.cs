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
                WriteColor("Downloaded file successfully!", ConsoleColor.Green);
        }

        private static readonly string LatiteFolder =
            $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\RoamingState\Latite";

        private static readonly string UserName = Environment.UserName;

        static void Main(string[] args)
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
                DownloadFile("https://latite-client.is-from.space/r/ChakraCore.dll", $"{LatiteFoler}\\ChakraCore.dll");
            }

            Thread.Sleep(1000);
        }
    }
}