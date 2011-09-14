namespace Microsoft.Samples.SocialGames.WorkerNodeJs
{
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
    using System.Net.Sockets;

    public class WorkerRole : RoleEntryPoint
    {
        Process proc;

        public override void Run()
        {
            while (!proc.WaitForExit(60000))
            {
                if (!NodeIsOk())
                    break; 
            }

            Trace.TraceError("NodeJs is down");
        }

        public override bool OnStart()
        {
            Trace.TraceInformation("Microsoft.Samples.SocialGames.WorkerNodeJs.OnStart");
            this.LaunchNode();
            return base.OnStart();
        }

        private bool NodeIsOk()
        {
            Trace.TraceInformation("Testing Node.Js server");

            try
            {
                TcpClient node;

                node = new TcpClient("127.0.0.1", 8124);

                Thread.Sleep(500);

                node.Close();

                return true;
            }
            catch
            {
                Trace.TraceError("Testing Failed");
                return false;
            }
        }

        private Process LaunchNode()
        {
            Trace.TraceInformation("Launching Node.Js server");
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

            return proc;
        }
    }
}
