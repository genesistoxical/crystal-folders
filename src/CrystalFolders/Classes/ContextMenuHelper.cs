using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CrystalFolders.Classes
{
    public static class ContextMenuHelper
    {
        public static bool IsWindows11()
        {
            try
            {
                return win_version_csharp.WinVersion.IsBuildNumGreaterOrEqual(22000);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsContextMenuActive()
        {
            if (!IsWindows11()) return false;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -NonInteractive -Command \"if (Get-AppxPackage *CrystalFolders.ModernMenu*) { exit 0 } else { exit 1 }\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (Process p = Process.Start(psi))
                {
                    if (p.WaitForExit(3500))
                    {
                        return p.ExitCode == 0;
                    }
                }
            }
            catch { }

            return false;
        }

        public static bool RegisterContextMenu()
        {
            if (!IsWindows11()) return false;

            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string scriptPath = Path.Combine(baseDir, "Menu", "register.ps1");

                if (!File.Exists(scriptPath)) return false;

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-ExecutionPolicy Bypass -NoProfile -NonInteractive -File \"" + scriptPath + "\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit(10000);
                    return p.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool UnregisterContextMenu()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string scriptPath = Path.Combine(baseDir, "Menu", "unregister.ps1");

                if (!File.Exists(scriptPath)) return false;

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-ExecutionPolicy Bypass -NoProfile -NonInteractive -File \"" + scriptPath + "\"",
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                using (Process p = Process.Start(psi))
                {
                    p.WaitForExit(6000);
                    return p.ExitCode == 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void AutoRegisterIfFirstRunAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    if (IsWindows11() && !IsContextMenuActive())
                    {
                        RegisterContextMenu();
                    }
                }
                catch { }
            });
        }
    }
}
