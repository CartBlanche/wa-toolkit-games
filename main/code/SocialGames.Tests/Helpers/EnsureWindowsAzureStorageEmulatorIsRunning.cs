namespace Microsoft.Samples.SocialGames.Tests
{
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Win32;

    public class EnsureWindowsAzureStorageEmulatorIsRunning
    {
        public void DoIt()
        {
            // TODO: Only call dsinit if necessary
            //// this.RunCommand(this.DSInitPath(), "/silent");

            // TODO: Only call csrun if necessary
            this.RunCommand(this.CSRunPath(), "/devstore:start");
        }

        private string CSRunPath()
        {
            return Path.Combine(this.WindowsAzureSDKInstallPath(), @"bin\csrun");
        }

        private string DSInitPath()
        {
            return Path.Combine(this.WindowsAzureSDKInstallPath(), @"bin\devstore\dsinit");
        }

        private void RunCommand(string fileName, string arguments)
        {
            var process = Process.Start(fileName, arguments);
            process.WaitForExit();
        }

        private string WindowsAzureSDKInstallPath()
        {
            RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
            RegistryKey windowsAzureSDKKey = rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\ServiceHosting\v1.0");
            
            return (string)windowsAzureSDKKey.GetValue("InstallPath");
        }
    }
}
