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
                Console.WriteLine("\n1. Initialize static analysis" +
                                  "\n2. Initialize mass static analysis" +
                                  "\n3. Initialize dynamic analysis " +
                                  "\n0. Exit\n");
                
                int.TryParse(Console.ReadLine(), out choice);
                
                if (choice == 1)
                {
                    Console.WriteLine("Give path to Apk (with apk name included):");
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
                    Console.WriteLine("Give folder path for mass analysis");
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
                    Console.WriteLine("Give path to Apk (with apk name included):");
                    string pathToApk = Console.ReadLine();

                    Decompiler decompiler = new Decompiler();   //Create decompiler object to handle all functionality
                    decompiler.DecompileWithApktool(pathToApk); //Decompile the apk

                    Instrumentation instr = new Instrumentation
                    {
                        FilePath = pathToApk, 
                        PackageName = decompiler.Manifest.PackageName
                    };

                    var result = instr.Analyze(); 
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
