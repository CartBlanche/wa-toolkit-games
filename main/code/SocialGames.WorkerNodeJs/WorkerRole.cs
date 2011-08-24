using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using System.IO;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        Process proc;

        public override void Run()
        {
            proc.WaitForExit();
        }

        public override bool OnStart()
        {
            proc = new Process()
            {
                StartInfo = new ProcessStartInfo(
                    Environment.ExpandEnvironmentVariables(@"%RoleRoot%\approot\node.exe"),
                    "gameex.js --debug")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Environment.ExpandEnvironmentVariables(@"%RoleRoot%\approot\"),
                }
            };
            proc.Start();

            return base.OnStart();
        }
    }
}
