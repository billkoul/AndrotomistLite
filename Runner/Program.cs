using APKProfiler;
using System;
using System.IO;
using Androtomist;

namespace TestingConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int choice = -1; int innerChoice = -1;
            while (choice != 0)
            {
                Console.WriteLine("\n1. Initialize code analysis" +
                                  "\n2. Initialize bulk code analysis" +
                                  "\n3. Initialize taint analysis " +
                                  "\n4. Initialize bulk taint analysis " +
                                  "\n5. Initialize dynamic instrumentation (beta) " +
                                  "\n0. Exit\n");
                
                int.TryParse(Console.ReadLine(), out choice);
                
                if (choice == 1)
                {
                    Console.WriteLine("Give path to Apk (with apk name included): ");
                    string pathToApk = Console.ReadLine();
                    Decompiler decompiler = new Decompiler();   //Create decompiler object to handle all functionality
                    decompiler.DecompileWithApktool(pathToApk); //Decompile the apk
                    
                    //Give path to the produced manifest.xml to the corresponding decompiler field
                    decompiler.PathToManifest = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(pathToApk), "AndroidManifest.xml");
                    decompiler.AnalyzeManifest(decompiler.PathToManifest);  //Parse the manifest to extract information
                    
                    //Give path to Smali folders (on the level that contains all smali subfolders)
                    decompiler.PathToSmali = Path.Combine(Environment.CurrentDirectory, decompiler.ApkFileName);
                    decompiler.AnalyzeSmali(decompiler.PathToSmali);    //Parse .smali files inside all subdirectories and extract information
                    
                    //Give path to apk's certificate
                    decompiler.PathToCertificate = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(pathToApk), "original", "META-INF", "CERT.RSA");

                    while (innerChoice != 0)
                    {
                        Console.WriteLine("\n1. Write manifest information to file" +
                                          "\n2. Write smali information to file" +
                                          "\n0. Go back to previous menu\n");
                        innerChoice = -1;
                        int.TryParse(Console.ReadLine(), out innerChoice);
                        
                        if (innerChoice == 1)
                        {
                            decompiler.WriteManifestInfoToFile();
                        }
                        else if (innerChoice == 2)
                        {
                            decompiler.WriteSmaliInfoToFile();
                        }
                        else if (innerChoice == 0)
                        {
                            innerChoice = -1;
                            break;
                        }
                        else
                            Console.WriteLine("Invalid input. Please retry!");
                    }
                    Console.WriteLine("Time elapsed for the parsing of smali files: " + decompiler.Smali.Stopwatch.Elapsed.TotalSeconds);

                }
                else if (choice == 2)
                {
                    Console.WriteLine("Enter folder path for bulk static analysis: ");
                    string targetDirectory = Console.ReadLine();

                    string[] fileEntries = Directory.GetFiles(targetDirectory);

                    foreach (string pathToApk in fileEntries)
                    {
                        Decompiler decompiler = new Decompiler(); //Create decompiler object to handle all functionality
                        decompiler.DecompileWithApktool(pathToApk); //Decompile the apk

                        //Give path to the produced manifest.xml to the corresponding decompiler field
                        decompiler.PathToManifest = Path.Combine(Environment.CurrentDirectory, Path.GetFileNameWithoutExtension(pathToApk), "AndroidManifest.xml");
                        decompiler.AnalyzeManifest(decompiler.PathToManifest); //Parse the manifest to extract information


                        //Give path to Smali folders (on the level that contains all smali subfolders)
                        decompiler.PathToSmali = Path.Combine(Environment.CurrentDirectory, decompiler.ApkFileName);
                        decompiler.AnalyzeSmali(decompiler.PathToSmali); //Parse .smali files inside all subdirectories and extract information

                        decompiler.WriteManifestInfoToFile();
                        decompiler.WriteSmaliInfoToFile();
                    }
                }
                else if (choice == 3)
                {
                    Console.WriteLine("Enter path to Apk (with apk name included): ");
                    string pathToApk = Console.ReadLine();

                    TaintAnalysis taintAnalyis = new TaintAnalysis(pathToApk);

                    taintAnalyis.Run();
                    taintAnalyis.WriteTaintInfoToFile();

                }
                else if (choice == 4)
                {
                    Console.WriteLine("Enter folder path for bulk taint analysis: ");
                    string targetDirectory = Console.ReadLine();

                    string[] fileEntries = Directory.GetFiles(targetDirectory);

                    foreach (string pathToApk in fileEntries)
                    {

                        TaintAnalysis taintAnalyis = new TaintAnalysis(pathToApk);

                        taintAnalyis.Run();
                        taintAnalyis.WriteTaintInfoToFile();
                    }

                }
                else if (choice == 5)
                {
                    Console.WriteLine("Enter path to Apk (with apk name included): ");
                    string pathToApk = Console.ReadLine();

                    Decompiler decompiler = new Decompiler();   //Create decompiler object to handle all functionality
                    decompiler.DecompileWithApktool(pathToApk); //Decompile the apk

                    Instrumentation instrumentation = new Instrumentation
                    {
                        FilePath = pathToApk,
                        PackageName = decompiler.Manifest.PackageName
                    };

                    var result = instrumentation.Analyze();
                    Console.Write(result);

                }
                else if (choice == 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Your choice isn't a valid option. Please retry:");
                    choice = Convert.ToInt32(Console.ReadLine());
                }
            }
        }
    }
}
