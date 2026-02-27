using System;
using System.Diagnostics;
using System.IO;
using ClosedXML.Excel;

namespace Androtomist
{
    public class TaintAnalysis
    {
        private string _apkPath;
        private string _result;

        public TaintAnalysis(string path = "")
        {
            _apkPath = path;
        }

        public void Run()
        {
            string path = Directory.GetCurrentDirectory();

            ProcessStartInfo procStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = path + "\\runTaint.py " + _apkPath
            };

            Console.WriteLine(procStartInfo.Arguments);

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;
            procStartInfo.FileName = "py";

            using (Process process = Process.Start(procStartInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    _result = reader.ReadToEnd();
                }
            }
        }

        public void WriteTaintInfoToFile()
        {
            string apkFileName = Path.GetFileNameWithoutExtension(_apkPath);
            //If xlsx file doesn't already exist, create it and write smali info 
            if (File.Exists(apkFileName + ".xlsx"))
            {
                var workbook = new XLWorkbook(apkFileName + ".xlsx");

                IXLWorksheet IpWorksheet = workbook.Worksheets.Add("Taint Analysis");
                IpWorksheet.Cell(1, 1).Value = "Results";

                string[] results = _result.Split("\n");

                for (int index = 0; index <= results.Length-1; index++)
                    IpWorksheet.Cell(index + 1, 1).Value = results[index];

                workbook.Save();
            }
            else
            {
                var workbook = new XLWorkbook();

                IXLWorksheet IpWorksheet = workbook.Worksheets.Add("Taint Analysis");
                IpWorksheet.Cell(1, 1).Value = "Results";

                string[] results = _result.Split("\n");

                for (int index = 0; index <= results.Length-1; index++)
                    IpWorksheet.Cell(index + 1, 1).Value = results[index];

                workbook.SaveAs(apkFileName + ".xlsx");
            }

            Console.Out.WriteLine("Export Successful!");
        }
    }
}
