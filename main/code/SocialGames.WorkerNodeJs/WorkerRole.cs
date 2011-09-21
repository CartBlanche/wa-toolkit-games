namespace Microsoft.Samples.SocialGames.WorkerNodeJs
{
    using System;
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.Security.Permissions;
    using System.Threading;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public class WorkerRole : RoleEntryPoint
    {
        private Process proc;

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public override void Run()
        {
            while (!this.proc.WaitForExit(60000))
            {
                if (!this.NodeIsOk())
                {
                    break;
                }
            }

            Trace.TraceError("NodeJs is down");
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
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
            this.proc = new Process()
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
            this.proc.Start();

            return this.proc;
        }
    }
}
