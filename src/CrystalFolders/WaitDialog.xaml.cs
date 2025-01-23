using System.Windows;

namespace CrystalFolders
{
    /// <summary>
    /// Small Wait Dialog
    /// </summary>
    public partial class WaitDialog : Window
    {
        public WaitDialog()
        {
            InitializeComponent();
            Label.Text = MainWindow.isRestore ? Properties.Resources.YourFoldersAreBeingRestored : 
                Properties.Resources.YourFoldersAreBeingCustomized;
        }
    }
}