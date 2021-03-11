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
        private static Settings settings = new Settings();
        private string toolsPath = settings.ToolsPath;
        private string remoteAddr = settings.RemoteAddr;
        private string fridaPath = settings.FridaPath;
        private string script = settings.Script;
        private string events = settings.Events;

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
            _ = t1.cmd("cd /d " + toolsPath + " && adb.exe push ../frida-server-12.6.23-android-x86_64 /data/local/tmp");
            _ = t1.cmd("cd /d " + toolsPath + " && adb.exe shell \"su -c 'chmod 755 /data/local/tmp/frida-server-12.6.23-android-x86_64'\"");
            _ = t1.cmd("cd /d " + toolsPath + " && adb.exe shell \"su -c '/data/local/tmp/frida-server-12.6.23-android-x86_64 >/dev/null 2>&1 &'\"");

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
                    data += t1.cmd("cd " + fridaPath + " && frida -U -l " + script + " " + PackageName, 60000);
                },
                () =>
                {
                    t1.cmd("cd " + toolsPath + " && adb.exe shell \"monkey -p " + PackageName + " --pct-trackball 0 --pct-syskeys 0 --pct-nav 0 --pct-majornav 0 --ignore-crashes -v " + events + "\"", 5000);
                },
                () =>
                {
                    data += t1.cmd("cd " + fridaPath + " && frida -U -l " + script + " " + PackageName, 60000);
                }
            );
            #endregion

            return data;
        }
    }
}
