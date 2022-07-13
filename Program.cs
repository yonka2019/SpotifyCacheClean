using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SpotifyCacheClean
{
    internal class Program
    {
        private const string SPOTIFY_CACHE_PATH = @"C:\Users\yonka\AppData\Local\Packages\SpotifyAB.SpotifyMusic_zpdnekdrzrea0\LocalState\Spotify\Users\d792kmzau1z83vie5r149injq-user\";
        private const string SPOTIFY_PROCESS_NAME = "Spotify";
        private static readonly string userMyMusicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\";
        private static readonly string userMyMusicTempPath = Path.GetTempPath() + @"SpotifyLocalSongs\";
        private static bool error = false;

        private static async Task Main(string[] args)
        {
            try
            {
                if (CloseSpotify())
                    WriteLog("Spotify closed");
                else
                    WriteLog("Spotify already closed");


                if (Directory.Exists(userMyMusicTempPath))
                {
                    Directory.Delete(userMyMusicTempPath, true);
                    WriteLog("User music temp folder removed");
                }

                Directory.CreateDirectory(userMyMusicTempPath);
                WriteLog("User music temp folder created");

                MoveSongs(userMyMusicPath, userMyMusicTempPath); // music -> temp
                WriteLog("Songs moved to temp folder");

                File.Delete(SPOTIFY_CACHE_PATH + "local-files.bnk");
                WriteLog("Spotify cache removed");

                Process.Start(SPOTIFY_PROCESS_NAME + ".exe");
                WriteLog("Spotify started");

                await Wait(3000); // wait till spotify fully start

                MoveSongs(userMyMusicTempPath, userMyMusicPath); // temp -> music
                WriteLog("Songs restored to music folder");
            }
            catch (Exception e)
            {
                WriteLog("-- ERROR --\n" + e.ToString());
                error = true;
            }

            if (!error)
                WriteLog("-- SUCCESS -- ");

            Console.ReadKey();
        }

        private static async Task Wait(int timeToWait)
        {
            int dotsNum = timeToWait / 100;

            for (int i = 0; i < dotsNum; i++)
            {
                Console.Write(".");
                await Task.Delay(100);
            }
            Console.WriteLine();
        }

        private static void MoveSongs(string fromPath, string toPath)
        {
            DirectoryInfo songsDirectory = new DirectoryInfo(fromPath);
            FileInfo[] songsFiles = songsDirectory.GetFiles("*.mp3");

            foreach (FileInfo song in songsFiles)
            {
                song.MoveTo(toPath + song.Name);
            }
        }

        private static bool CloseSpotify()
        {
            Process[] processes = Process.GetProcessesByName(SPOTIFY_PROCESS_NAME);
            if (processes.Length == 0)
                return false;

            foreach (Process process in processes)
            {
                try
                {
                    process.Kill();
                    process.WaitForExit();
                    process.Dispose();
                }
                catch { return true; }
            }
            return true;
        }

        private static void WriteLog(string info)
        {
            Console.WriteLine($"\n[LOG] [{DateTime.Now:HH:mm:ss}] : {info}");
        }
    }
}
