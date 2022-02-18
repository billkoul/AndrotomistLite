using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Androtomist
{
    public class Instrumentation
    {
        protected Terminal t1;
        private static readonly Settings settings = new Settings();
        private readonly string toolsPath = settings.ToolsPath;
        private readonly string remoteAddr = settings.RemoteAddr;
        private readonly string fridaPath = settings.FridaPath;
        private readonly string fridaServer = settings.FridaServer;
        private readonly string script = settings.Script;
        private readonly int events = settings.Events;

        public string FilePath;
        public string PackageName;

        public string Analyze()
        {
            if(string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(PackageName))
                throw new Exception("No FilePath or PackageName provided.");

            ConnectDevice();
            InstallSample();
            var data = ExtractDynamicData();
            DisconnectDevice();

            return data;
        }

        protected void ConnectDevice()
        {

            _ = t1.cmd("cd /d " + toolsPath + " && adb.exe connect " + remoteAddr);
            _ = t1.cmd("cd /d " + toolsPath + " && adb.exe push " + fridaPath + "/" + fridaServer + " /data/local/tmp");
            _ = t1.cmd("cd /d " + toolsPath + " && adb.exe shell \"su -c 'chmod 755 /data/local/tmp/" + fridaServer + "'\"");
            _ = t1.cmd("cd /d " + toolsPath + " && adb.exe shell \"su -c '/data/local/tmp/" + fridaServer + " >/dev/null 2>&1 &'\"");

        }

        protected void DisconnectDevice()
        {
                t1.cmd("cd /d " + toolsPath + " && adb.exe disconnect " + remoteAddr);
        }

        protected void InstallSample()
        {
            _ = t1.cmd("cd " + toolsPath + " && adb.exe install " + FilePath);

        }

        protected string ExtractDynamicData()
        {
            string data = "";

            #region ParallelTasks
            Parallel.Invoke(
                () =>
                {
                    data += t1.cmd("cd " + fridaPath + " && frida -U -l " + script + " " + PackageName, events);
                },
                () =>
                {
                    t1.cmd("cd " + toolsPath + " && adb.exe shell \"monkey -p " + PackageName + " --pct-trackball 0 --pct-syskeys 0 --pct-nav 0 --pct-majornav 0 --ignore-crashes -v " + events + "\"", 5000);
                },
                () =>
                {
                    data += t1.cmd("cd " + fridaPath + " && frida -U -l " + script + " " + PackageName, events);
                }
            );
            #endregion

            return data;
        }
    }
}
