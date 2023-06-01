using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace APKProfiler
{
    public class Smali
    {
        private List<string> smaliFiles;
        private List<string> apiCalls;
        private List<string> urls;
        private List<string> ips;
        private Stopwatch stopwatch;
        public List<string> SmaliFiles => smaliFiles;
        public List<string> ApiCalls => apiCalls;
        public List<string> Urls => urls;
        public List<string> Ips => ips;
        public Stopwatch Stopwatch => stopwatch;

        public Smali()
        {
            smaliFiles = new List<string>();
            apiCalls = new List<string>();
            urls = new List<string>();
            ips = new List<string>();
        }

        //Function that gets the name of each .smali file inside the produced folder from the decompiling of the apk.
        private void GetFileList(string pathToSmali)
        {
            smaliFiles = Directory.GetFiles(pathToSmali, "*.smali", SearchOption.AllDirectories).ToList();
        }

        public void ParseSmali(string pathToSmali)
        {
            GetFileList(pathToSmali);
            string tmpString;
            int startIndex;
            //Set up the regular expression for URLs
            Regex urlRegex = new Regex(@"(http|https|ftp)(:\/\/)([0-9a-zA-Z]+)([\.\w]*[0-9a-zA-Z])*([a-zA-Z0-9\/\.?=&#$%~_:()\-+""'<>]*)");
            //Set up the regular expression for IPs
            Regex ipRegex = new Regex(@"([0-9]{1,3}\.){3}([0-9]{1,3}){1}:?([0-9]{1,5})?");

            try
            {
                stopwatch = Stopwatch.StartNew();
                foreach (string smaliFile in smaliFiles)
                {
                    foreach (string line in File.ReadLines(smaliFile))
                    {
                        //If line contains API Call
                        if (line.Contains("invoke-virtual") || line.Contains("invoke-static") || line.Contains("invoke-direct"))
                        {
                            startIndex = line.IndexOf('L');
                            //Substring from startIndex (index of L) +1 up to startIndex+1 + length of the line minus 2 to remove up to the V symbol at the end - startIndex
                            //so that the total length is not larger that the actual line length.
                            tmpString = line.Substring(startIndex + 1, line.Length - startIndex - 2);
                            if (!apiCalls.Contains(tmpString) && tmpString.Length > 10 && !tmpString.Contains("$") && !tmpString.Contains("<") && tmpString.Contains(";->"))
                            {
                                apiCalls.Add(tmpString);
                            }
                            
                        }
                        //If line contains URL
                        if (urlRegex.Match(line).Success)
                        {
                            tmpString = urlRegex.Match(line).Value; //Get the url value
                            tmpString = tmpString.Substring(0, tmpString.Length - 1);   //Remove the last quotation mark from the string
                            if (!urls.Contains(tmpString))
                            {
                                urls.Add(tmpString);
                            }
                        }
                        //If line contains IP
                        if (ipRegex.Match(line).Success)
                        {
                            tmpString = ipRegex.Match(line).Value;
                            if (!ips.Contains(tmpString))
                            {
                                ips.Add(tmpString);
                            }
                        }
                    }
                }
                stopwatch.Stop();
                
            }
            catch (FileLoadException) { }
        }
    }
}
