using System.Windows;

namespace CrystalFolders
{
    /// <summary>
    /// Configure as portable - HelpDialog
    /// </summary>
    public partial class HelpDialog
    {
        public HelpDialog()
        {
            InitializeComponent();

            if (!MainWindow.isRestore)
            {
                switch (Config.currentLan)
                {
                    case "en":
                        Text.Margin = new Thickness(34, 26, 0, 0);
                        Deco.Margin = new Thickness(28, 29, 0, 198);
                        break;
                    case "es":
                        Text.Margin = new Thickness(33, 26, 0, 0);
                        Deco.Margin = new Thickness(27, 29, 0, 198);
                        break;
                }

                Text.Text = Properties.Resources.WillMakeTheCustomIconVisible + "\r\n\r\n" +
                Properties.Resources.ItWorksByDraggingFolders + "\r\n\r\n" +
                Properties.Resources.YouCanCustomizeUpTo30Folders;
            }
            else
            {
                switch (Config.currentLan)
                {
                    case "en":
                        Text.Margin = new Thickness(38, 26, 0, 0);
                        Deco.Margin = new Thickness(32, 29, 0, 198);
                        break;
                    case "es":
                        Text.Margin = new Thickness(32, 26, 0, 0);
                        Deco.Margin = new Thickness(26, 29, 0, 198);
                        break;
                }

                Text.Text = Properties.Resources.WillRemoveAnyIconsCopied + "\r\n\r\n" +
                Properties.Resources.ItWorksByDraggingFolders + "\r\n\r\n" +
                Properties.Resources.YouCanRestoreUpTo30Folders;
            }
        }
    }
}
