using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SpotifyCacheClean
{
    internal class Program
    {
        // to edit
        private const string SPOTIFY_CACHE_PATH = @"Fill me";
        private static readonly string LOCAL_MUSIC_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\"; // DEFAULT: C:\Users\Music

        // not edit
        private const int WAIT_TIME_MS = 5000; // 5000 / 1000 -> 5 seconds
        private const string SPOTIFY_PROCESS_NAME = "Spotify";
        private static readonly string LOCAL_MUSIC_TEMP_PATH = Path.GetTempPath() + @"SpotifyLocalSongs\";
        private static bool error = false;

        private static async Task Main(string[] args)
        {
            try
            {
                if (CloseSpotify())
                    WriteLog("Spotify closed");
                else
                    WriteLog("Spotify already closed");


                if (Directory.Exists(LOCAL_MUSIC_TEMP_PATH))
                {
                    Directory.Delete(LOCAL_MUSIC_TEMP_PATH, true);
                    WriteLog("Temp local songs folder removed");
                }

                Directory.CreateDirectory(LOCAL_MUSIC_TEMP_PATH);
                WriteLog("Temp local songs folder created");

                MoveSongs(LOCAL_MUSIC_PATH, LOCAL_MUSIC_TEMP_PATH); // music -> temp
                WriteLog("Songs moved to temp folder");

                File.Delete(SPOTIFY_CACHE_PATH + "local-files.bnk");
                WriteLog("Spotify cache removed");

                Process.Start(SPOTIFY_PROCESS_NAME + ".exe");
                WriteLog("Spotify started");

                WriteLog("! DO NOT CLOSE THIS WINDOW TILL PROCESS FINISHED !");
                await Wait(WAIT_TIME_MS); // wait till spotify fully start

                MoveSongs(LOCAL_MUSIC_TEMP_PATH, LOCAL_MUSIC_PATH); // temp -> music
                WriteLog("Songs restored to original local songs folder");
            }
            catch (Exception e)
            {
                WriteLog("-- ERROR --\n" + e.ToString());
                error = true;
            }

            if (!error)
                WriteLog("-- SUCCESS -- ");

            Console.WriteLine("\nPress any button to exit...");
            Console.ReadKey();
        }

        private static async Task Wait(int timeToWait)
        {
            int dotsNum = timeToWait / 100;

            for (int i = 0; i < dotsNum; i++)
            {
                Console.Write($".");
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
            Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] {info}");
        }
    }
}
