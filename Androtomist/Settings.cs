using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Androtomist
{
    public class Settings
    {
        public string ToolsPath { get; set; }
        public string RemoteAddr { get; set; }
        public string FridaPath { get; set; }
        public string FridaServer { get; set; }
        public string Script { get; set; }
        public int Events { get; set; }
        public string AndroidImage { get; set; }
        public Settings()
        {
            try
            {
                JToken jAppSettings = JToken.Parse(
                    File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
                );

                ToolsPath = jAppSettings["Settings"]["ToolsPath"].ToString();
                RemoteAddr = jAppSettings["Settings"]["RemoteAddr"].ToString();
                FridaPath = jAppSettings["Settings"]["FridaPath"].ToString();
                FridaServer = jAppSettings["Settings"]["FridaServer"].ToString();
                Script = jAppSettings["Settings"]["Script"].ToString();
                Events = int.Parse(jAppSettings["Settings"]["Events"].ToString());
                AndroidImage = jAppSettings["Settings"]["AndroidImage"].ToString();
            }
            catch { }
        }
    }
}
