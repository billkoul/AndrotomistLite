using APKProfiler;
using System;
using System.IO;
using Androtomist;
using System.Linq;

namespace TestingConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                PrintMenu();

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            RunSingleStaticAnalysis();
                            break;

                        case 2:
                            RunBulkStaticAnalysis();
                            break;

                        case 3:
                            RunSingleTaintAnalysis();
                            break;

                        case 4:
                            RunBulkTaintAnalysis();
                            break;

                        case 5:
                            RunInstrumentation();
                            break;

                        case 0:
                            return;

                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }
        }

        static void PrintMenu()
        {
            Console.WriteLine("\n1. Initialize code analysis");
            Console.WriteLine("2. Initialize bulk code analysis");
            Console.WriteLine("3. Initialize taint analysis");
            Console.WriteLine("4. Initialize bulk taint analysis");
            Console.WriteLine("5. Initialize dynamic instrumentation (beta)");
            Console.WriteLine("0. Exit\n");
        }

        static string GetValidApkPath()
        {
            Console.WriteLine("Enter full path to APK (including .apk):");
            string path = Console.ReadLine()?.Trim('"');

            if (string.IsNullOrWhiteSpace(path))
                throw new Exception("Path cannot be empty.");

            if (!File.Exists(path))
                throw new FileNotFoundException("APK file not found.", path);

            if (Path.GetExtension(path).ToLower() != ".apk")
                throw new Exception("File must have .apk extension.");

            return Path.GetFullPath(path);
        }

        static void RunSingleStaticAnalysis()
        {
            string pathToApk = GetValidApkPath();

            Decompiler decompiler = new Decompiler();

            Console.WriteLine("Decompiling...");
            decompiler.DecompileWithApktool(pathToApk);

            string outputFolder = Path.Combine(
                Environment.CurrentDirectory,
                Path.GetFileNameWithoutExtension(pathToApk));

            if (!Directory.Exists(outputFolder))
                throw new Exception("Decompilation failed. Output folder not found.");

            decompiler.PathToManifest = Path.Combine(outputFolder, "AndroidManifest.xml");

            if (!File.Exists(decompiler.PathToManifest))
                throw new Exception("Manifest file not found after decompilation.");

            decompiler.AnalyzeManifest(decompiler.PathToManifest);

            decompiler.PathToSmali = outputFolder;

            if (!Directory.Exists(decompiler.PathToSmali))
                throw new Exception("Smali folder not found.");

            decompiler.AnalyzeSmali(decompiler.PathToSmali);

            Console.WriteLine("Static analysis complete.");
            Console.WriteLine($"Smali parsing time: {decompiler.Smali.Stopwatch.Elapsed.TotalSeconds} seconds");
        }

        static void RunBulkStaticAnalysis()
        {
            Console.WriteLine("Enter folder path for bulk static analysis:");
            string folder = Console.ReadLine()?.Trim('"');

            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException("Directory not found.");

            var apkFiles = Directory.GetFiles(folder, "*.apk");

            foreach (var apk in apkFiles)
            {
                try
                {
                    Console.WriteLine($"\nProcessing: {apk}");
                    RunSingleStaticAnalysisInternal(apk);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed: {ex.Message}");
                }
            }
        }

        static void RunSingleStaticAnalysisInternal(string pathToApk)
        {
            Decompiler decompiler = new Decompiler();
            decompiler.DecompileWithApktool(pathToApk);
        }

        static void RunSingleTaintAnalysis()
        {
            string pathToApk = GetValidApkPath();

            TaintAnalysis taint = new TaintAnalysis(pathToApk);
            taint.Run();
            taint.WriteTaintInfoToFile();

            Console.WriteLine("Taint analysis completed.");
        }

        static void RunBulkTaintAnalysis()
        {
            Console.WriteLine("Enter folder path for bulk taint analysis:");
            string folder = Console.ReadLine()?.Trim('"');

            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException("Directory not found.");

            var apkFiles = Directory.GetFiles(folder, "*.apk");

            foreach (var apk in apkFiles)
            {
                try
                {
                    Console.WriteLine($"\nProcessing: {apk}");
                    TaintAnalysis taint = new TaintAnalysis(apk);
                    taint.Run();
                    taint.WriteTaintInfoToFile();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed: {ex.Message}");
                }
            }
        }

        static void RunInstrumentation()
        {
            string pathToApk = GetValidApkPath();

            Decompiler decompiler = new Decompiler();
            decompiler.DecompileWithApktool(pathToApk);

            if (decompiler.Manifest == null)
                throw new Exception("Manifest not loaded.");

            Instrumentation instrumentation = new Instrumentation
            {
                FilePath = pathToApk,
                PackageName = decompiler.Manifest.PackageName
            };

            var result = instrumentation.Analyze();
            Console.WriteLine(result);
        }
    }
}