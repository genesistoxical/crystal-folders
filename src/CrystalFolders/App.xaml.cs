using System;
using System.Windows;
using win_version_csharp;

namespace CrystalFolders
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Config.CheckPath();
            Config.Language();
            Config.GetMessage();

            if (WinVersion.GetVersion(out VersionInfo info))
            {
                Config.winvers = "Windows Version: " + info.Major;
                Console.WriteLine(Config.winvers);
            }

            InitializeComponent();
            Config.GetTheme();
        }
    }
}