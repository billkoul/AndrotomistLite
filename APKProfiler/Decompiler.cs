using ClosedXML.Excel;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace APKProfiler
{
    public class Decompiler
    {
        //Decompiler Fields
        private string pathToApktool;
        private string pathToManifest;
        private Manifest manifest;
        private string pathToSmali;
        private Smali smali;
        private string apkFileName;
        private string pathToCertificate;

        //Getters
        public string PathToApktool 
        {
            get { return pathToApktool; }
            set { pathToApktool = value; }
        }
        public string PathToManifest
        {
            get { return pathToManifest; }
            set { pathToManifest = value; }
        }
        public string PathToSmali
        {
            get { return pathToSmali; }
            set { pathToSmali = value; }
        }
        public Manifest Manifest => manifest;
        public Smali Smali => smali;
        public string ApkFileName => apkFileName;
        public string PathToCertificate
        {
            get { return pathToCertificate; }
            set { pathToCertificate = value; }
        }
        public Decompiler()
        {
            pathToApktool = "apktool";
            pathToManifest = null;
            manifest = null;
            pathToSmali = null;
            smali = null;
            apkFileName = null;
        }

        //Decompile apk using apktool
        public void DecompileWithApktool(string apkFilePath)
        {
            try
            {
                apkFileName = Path.GetFileNameWithoutExtension(apkFilePath);
                //Create process to start hidden command prompt and use the apktool to decompile the apk the user has pointed out
                Process process = new Process();
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",   //in Command Prompt
                    WindowStyle = ProcessWindowStyle.Hidden,    //make cmd window hidden
                    Arguments = "/C " + pathToApktool + " decode " + apkFilePath + " -o " + apkFileName  //decode using apktool and put result in folder
                };
                process.StartInfo = processStartInfo;
                process.Start();
                process.WaitForExit();
                process.Close();

            }
            catch (Exception) { }

        }

        public void AnalyzeManifest(string pathToManifest)
        {
            manifest = new Manifest();
            manifest.ParseManifest(pathToManifest);

        }

        public void AnalyzeSmali(string pathToSmali)
        {
            smali = new Smali();
            smali.ParseSmali(pathToSmali);
        }
        //Function to write all information extracted from AndroidManifest.xml to a file
        public void WriteManifestInfoToFile()
        {
            //Create xlsx file if it doesn't exist and write manifest information in it
            if(File.Exists(apkFileName + ".xlsx")){
                var workbook = new XLWorkbook(apkFileName + ".xlsx");

                IXLWorksheet BasicInfoWorksheet = workbook.Worksheets.Add("Basic Info");
                BasicInfoWorksheet.Cell(1, 1).Value = "Package Name:";
                BasicInfoWorksheet.Cell(2, 1).Value = manifest.PackageName;
                BasicInfoWorksheet.Cell(1, 2).Value = "Compile SDK Version:";
                BasicInfoWorksheet.Cell(2, 2).Value = manifest.CompileSdkVersion;

                IXLWorksheet PermissionWorksheet = workbook.Worksheets.Add("Permissions");
                PermissionWorksheet.Cell(1, 1).Value = "Permission";
                for (int index = 1; index <= manifest.Permissions.Count; index++)
                    PermissionWorksheet.Cell(index + 1, 1).Value = manifest.Permissions[index - 1];

                IXLWorksheet IntentsWorksheet = workbook.Worksheets.Add("Intents");
                IntentsWorksheet.Cell(1, 1).Value = "Intent";
                for (int index = 1; index <= manifest.Intents.Count; index++)
                    IntentsWorksheet.Cell(index + 1, 1).Value = manifest.Intents[index - 1];

                IXLWorksheet ServicesWorksheet = workbook.Worksheets.Add("Services");
                ServicesWorksheet.Cell(1, 1).Value = "Service";
                for (int index = 1; index <= manifest.Services.Count; index++)
                    ServicesWorksheet.Cell(index + 1, 1).Value = manifest.Services[index - 1];

                IXLWorksheet ActivitiesWorksheet = workbook.Worksheets.Add("Activities");
                ActivitiesWorksheet.Cell(1, 1).Value = "Activity";
                for (int index = 1; index <= manifest.Activities.Count; index++)
                    ActivitiesWorksheet.Cell(index + 1, 1).Value = manifest.Activities[index - 1];

                IXLWorksheet ReceiversWorksheet = workbook.Worksheets.Add("Receivers");
                ReceiversWorksheet.Cell(1, 1).Value = "Receiver";
                for (int index = 1; index <= manifest.Receivers.Count; index++)
                    ReceiversWorksheet.Cell(index + 1, 1).Value = manifest.Receivers[index - 1];

                IXLWorksheet ProvidersWorksheet = workbook.Worksheets.Add("Providers");
                ProvidersWorksheet.Cell(1, 1).Value = "Provider";
                for (int index = 1; index <= manifest.Providers.Count; index++)
                    ProvidersWorksheet.Cell(index + 1, 1).Value = manifest.Providers[index - 1];


                workbook.Save();
            }
            //If xlsx file already exists append Manifest information
            else{
                var workbook = new XLWorkbook();

                IXLWorksheet BasicInfoWorksheet = workbook.Worksheets.Add("Basic Info");
                BasicInfoWorksheet.Cell(1, 1).Value = "Package Name:";
                BasicInfoWorksheet.Cell(2, 1).Value = manifest.PackageName;
                BasicInfoWorksheet.Cell(1, 2).Value = "Compile SDK Version:";
                BasicInfoWorksheet.Cell(2, 2).Value = manifest.CompileSdkVersion;

                IXLWorksheet PermissionWorksheet = workbook.Worksheets.Add("Permissions");
                PermissionWorksheet.Cell(1, 1).Value = "Permission";
                for (int index = 1; index <= manifest.Permissions.Count; index++)
                    PermissionWorksheet.Cell(index + 1, 1).Value = manifest.Permissions[index - 1];

                IXLWorksheet IntentsWorksheet = workbook.Worksheets.Add("Intents");
                IntentsWorksheet.Cell(1, 1).Value = "Intent";
                for (int index = 1; index <= manifest.Intents.Count; index++)
                    IntentsWorksheet.Cell(index + 1, 1).Value = manifest.Intents[index - 1];

                IXLWorksheet ServicesWorksheet = workbook.Worksheets.Add("Services");
                ServicesWorksheet.Cell(1, 1).Value = "Service";
                for (int index = 1; index <= manifest.Services.Count; index++)
                    ServicesWorksheet.Cell(index + 1, 1).Value = manifest.Services[index - 1];

                IXLWorksheet ActivitiesWorksheet = workbook.Worksheets.Add("Activities");
                ActivitiesWorksheet.Cell(1, 1).Value = "Activity";
                for (int index = 1; index <= manifest.Activities.Count; index++)
                    ActivitiesWorksheet.Cell(index + 1, 1).Value = manifest.Activities[index - 1];

                IXLWorksheet ReceiversWorksheet = workbook.Worksheets.Add("Receivers");
                ReceiversWorksheet.Cell(1, 1).Value = "Receiver";
                for (int index = 1; index <= manifest.Receivers.Count; index++)
                    ReceiversWorksheet.Cell(index + 1, 1).Value = manifest.Receivers[index - 1];

                IXLWorksheet ProvidersWorksheet = workbook.Worksheets.Add("Providers");
                ProvidersWorksheet.Cell(1, 1).Value = "Provider";
                for (int index = 1; index <= manifest.Providers.Count; index++)
                    ProvidersWorksheet.Cell(index + 1, 1).Value = manifest.Providers[index - 1];


                workbook.SaveAs(apkFileName + ".xlsx");

            }

            Console.Out.WriteLine("Export Successful!");

        }
       
        //Function to write all information extracted from .smali files to an excel file (one sheet tab per category)
        public void WriteSmaliInfoToFile()
        {
            //If xlsx file doesn't already exist, create it and write smali info 
            if(File.Exists(apkFileName + ".xlsx")){
                var workbook = new XLWorkbook(apkFileName + ".xlsx");

                IXLWorksheet IpWorksheet = workbook.Worksheets.Add("IPs");
                IpWorksheet.Cell(1, 1).Value = "IP";
                for (int index = 1; index <= smali.Ips.Count; index++)
                    IpWorksheet.Cell(index + 1, 1).Value = smali.Ips[index - 1];

                IXLWorksheet UrlWorksheet = workbook.Worksheets.Add("Urls");
                UrlWorksheet.Cell(1, 1).Value = "Urls";
                for (int index = 1; index <= smali.Urls.Count; index++)
                    UrlWorksheet.Cell(index + 1, 1).Value = smali.Urls[index - 1];

                IXLWorksheet APICallWorksheet = workbook.Worksheets.Add("Api Calls");
                APICallWorksheet.Cell(1, 1).Value = "Api Calls";
                for (int index = 1; index <= smali.ApiCalls.Count; index++)
                    APICallWorksheet.Cell(index + 1, 1).Value = smali.ApiCalls[index - 1];

                workbook.Save();
            }  
            //Else if file already exists (eg. created by writing manifest info to file) then append to that file
            else{
                var workbook = new XLWorkbook();

                IXLWorksheet IpWorksheet = workbook.Worksheets.Add("IPs");
                IpWorksheet.Cell(1, 1).Value = "IP";
                for (int index = 1; index <= smali.Ips.Count; index++)
                    IpWorksheet.Cell(index + 1, 1).Value = smali.Ips[index - 1];

                IXLWorksheet UrlWorksheet = workbook.Worksheets.Add("Urls");
                UrlWorksheet.Cell(1, 1).Value = "Urls";
                for (int index = 1; index <= smali.Urls.Count; index++)
                    UrlWorksheet.Cell(index + 1, 1).Value = smali.Urls[index - 1];

                IXLWorksheet APICallWorksheet = workbook.Worksheets.Add("Api Calls");
                APICallWorksheet.Cell(1, 1).Value = "Api Calls";
                for (int index = 1; index <= smali.ApiCalls.Count; index++)
                    APICallWorksheet.Cell(index + 1, 1).Value = smali.ApiCalls[index - 1];

                workbook.SaveAs(apkFileName + ".xlsx");
            }

            Console.Out.WriteLine("Export Successful!");
        }
        //Function to display X509Certificate information and write to file, returns certificate content as a string
        public string WriteCertificateInfoToFile()
        {
            string path = Path.Combine(Environment.CurrentDirectory, apkFileName + "-Certificate.txt");
            string certificateContent = "Certificate not found";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (File.Exists(pathToCertificate))
            {
                X509Certificate2 certificate = new X509Certificate2(File.ReadAllBytes(pathToCertificate));
                certificateContent = certificate.ToString(true);
                File.WriteAllText(path, certificateContent);
            }
            
            return certificateContent;
        }
    }
}
