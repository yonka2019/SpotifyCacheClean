using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SpotifyCacheClean
{
    internal class Program
    {
        // to edit
        private const string SPOTIFY_CACHE_PATH = @"C:\Users\yonka\AppData\Local\Packages\SpotifyAB.SpotifyMusic_zpdnekdrzrea0\LocalState\Spotify\Users\d792kmzau1z83vie5r149injq-user\";  // fill according yours
        private static readonly string LOCAL_MUSIC_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + @"\";  // DEFAULT: C:\Users\Music

        // not edit
        private static bool songsMooved = false;
        private const int WAIT_TIME_MS = 5000; // 5000 / 1000 -> 5 seconds
        private const string SPOTIFY_PROCESS_NAME = "Spotify";
        private static readonly string LOCAL_MUSIC_TEMP_PATH = Path.GetTempPath() + @"SpotifyLocalSongs\";
        private static bool error = false;
        private const string SPOTIFY_CACHE_FILE = "local-files.bnk";

        private static async Task Main()
        {
            try
            {
                if (CloseSpotify())
                    WriteLog(LogType.INFO, "Spotify closed");
                else
                    WriteLog(LogType.INFO, "Spotify already closed");


                if (Directory.Exists(LOCAL_MUSIC_TEMP_PATH))
                {
                    Directory.Delete(LOCAL_MUSIC_TEMP_PATH, true);
                    WriteLog(LogType.INFO, "Temp local songs folder removed");
                }

                Directory.CreateDirectory(LOCAL_MUSIC_TEMP_PATH);
                WriteLog(LogType.INFO, "Temp local songs folder created");

                MoveDirectory(LOCAL_MUSIC_PATH, LOCAL_MUSIC_TEMP_PATH); // music -> temp
                WriteLog(LogType.INFO, "Songs moved to temp folder");

                if (!Directory.Exists(SPOTIFY_CACHE_PATH))
                    throw new Exception("Can't find spotify cache path, is the cache path correct? ('SPOTIFY_CACHE_PATH' Field)");

                File.Delete(SPOTIFY_CACHE_PATH + SPOTIFY_CACHE_FILE);
                WriteLog(LogType.INFO, "Spotify cache removed");

                Process.Start(SPOTIFY_PROCESS_NAME + ".exe");
                WriteLog(LogType.INFO, "Spotify started");

                WriteLog(LogType.WARNING, "DO NOT CLOSE THIS WINDOW UNTIL THIS PROCESS FINISHED");
                await Wait(WAIT_TIME_MS); // wait till spotify fully start

                MoveDirectory(LOCAL_MUSIC_TEMP_PATH, LOCAL_MUSIC_PATH); // temp -> music
                WriteLog(LogType.INFO, "Songs restored to original local songs folder");
            }

            catch (Exception e)
            {
                WriteLog(LogType.ERROR, e.ToString());
                error = true;
                if (songsMooved) // if songs already mooved and program crashed. -> restore data
                {
                    MoveDirectory(LOCAL_MUSIC_TEMP_PATH, LOCAL_MUSIC_PATH); // temp -> music
                    WriteLog(LogType.INFO, "Songs restored to original local songs folder, if the songs doesn't appeared, try to fix the error and rerun this utility");
                }
            }

            if (!error)
                WriteLog(LogType.INFO, "-- # SUCCESS # -- ");

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

        private static void MoveDirectory(string sourceDir, string destDir)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);

                if (!File.Exists(destFile))  // if file (folder) isn't already exist - move him
                {
                    File.Move(file, destFile);
                }
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string name = Path.GetFileName(subDir);
                string destSubDir = Path.Combine(destDir, name);
                MoveDirectory(subDir, destSubDir);
            }
            songsMooved = true;
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

        private static void WriteLog(LogType logType, string info)
        {
            Console.WriteLine($"\n[{DateTime.Now:HH:mm:ss}] [{logType}]  {info}");
        }
    }

    internal enum LogType
    {
        INFO,
        WARNING,
        ERROR
    }
}
