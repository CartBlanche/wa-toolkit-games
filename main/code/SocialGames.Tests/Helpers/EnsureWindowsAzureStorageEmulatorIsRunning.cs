namespace Microsoft.Samples.SocialGames.Tests
{
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Win32;
    using System;

    public class EnsureWindowsAzureStorageEmulatorIsRunning
    {
        public void DoIt()
        {
            // TODO: Only call dsinit if necessary
            this.RunCommand(this.DSInitPath(), "/silent");

            // TODO: Only call csrun if necessary
            //this.RunCommand(this.CSRunPath(), "/devstore:start");
        }

        private string CSRunPath()
        {
            return Path.Combine(this.WindowsAzureSDKInstallPath(), @"bin\csrun");
        }

        private string DSInitPath()
        {
            switch (WindowsAzureVersion) {
                case "1.5" :
                    return Path.Combine(this.WindowsAzureSDKInstallPath(), @"bin\devstore\dsinit");
                case "1.6" :
                    var rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                    var windowsAzureEmulatorKey = rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows Azure Emulator");
                    return Path.Combine((string)windowsAzureEmulatorKey.GetValue("InstallPath"), @"emulator\devstore\dsinit");
                default:
                    throw new InvalidOperationException("Searched for 1.5 and 1.6 but we didn't find the Windows Azure Emulator");
            }
        }

        private void RunCommand(string fileName, string arguments)
        {
            var process = Process.Start(fileName, arguments);
            process.WaitForExit();
        }

        private static string WindowsAzureVersion 
        {
            get
            {
                var rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);

                return
                    rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\ServiceHosting\v1.0") != null ? "1.5" :
                    rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\ServiceHosting\v1.6") != null ? "1.6" : null;
            }
            
        }

        private string WindowsAzureSDKInstallPath()
        {
            RegistryKey rootKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
            // try 1.5
            RegistryKey windowsAzureSDKKey = rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\ServiceHosting\v1.0");
            if (windowsAzureSDKKey != null) 
            {
                return (string)windowsAzureSDKKey.GetValue("InstallPath");
            }

            // try 1.6
            windowsAzureSDKKey = rootKey.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SDKs\ServiceHosting\v1.6");
            if (windowsAzureSDKKey != null)
            {
                return (string)windowsAzureSDKKey.GetValue("InstallPath");
            }

            return null;
        }
    }
}
