using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace CrystalFolders
{
    public partial class MainWindow : System.Windows.Window
    {
        /// <summary>
        /// Change Folder Icons
        /// </summary>

        public static string icoPath, userPath;
        public static bool isPortable = false, isRestore = false;
        public static ObservableCollection<string> folderList;
        public static List<string> subfolderList, ignore;

        public MainWindow()
        {
            InitializeComponent();

            folderList = new ObservableCollection<string>();
            subfolderList = new List<string>();
            ignore = new List<string>();

            // Obtener la ruta de usuario
            userPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        public void ApplyFolderSettings(string folderPath)
        {
            Icons.SHFOLDERCUSTOMSETTINGS customSettings = new Icons.SHFOLDERCUSTOMSETTINGS
            {
                dwMask = 0x10
            };

            // Poner un icono o quitarlo
            if (!isRestore)
            {
                customSettings.pszIconFile = icoPath;
                customSettings.iIconIndex = 0;
            }
            else
            {
                // pszIconFile and iIconIndex empty
                // pszIconFile y iIconIndex vacíos
            }

            uint FCS_FORCEWRITE = 0x00000002;

            // Aplicar la configuración al Desktop.ini
            Icons.SHGetSetFolderCustomSettings(ref customSettings, folderPath, FCS_FORCEWRITE);
        }

        public bool DirectoryPermissions(string directory)
        {
            // Una opción simple para saber si el directorio tiene permisos de escritura y
            // modificación, de lo contrario, no será posible personalizar el icono
            try
            {
                using (FileStream fs = File.Create(Path.Combine(directory, "cf_tmp.txt"), 1))
                {
                    fs.Close();
                }
                File.Delete(Path.Combine(directory, "cf_tmp.txt"));

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private void CheckBoxes(string primary, bool addrem)
        {
            // Lista de directorios para el folder especificado
            string[] directories = Directory.GetDirectories(userPath + @"\" + primary + @"\", "*");

            // Para cada directorio en la lista de directorios...
            foreach (string directory in directories)
            {
                // Crear una ruta corta del directorio (..\Documentos)
                string path = directory.Replace(userPath + @"\", @"..\");

                // Si el folder no está oculto...
                if (!((File.GetAttributes(directory) & FileAttributes.Hidden) == FileAttributes.Hidden))
                {
                    // Agregar o remover las rutas de la folderList
                    if (addrem)
                    {
                        if (!folderList.Contains(path))
                        {
                            folderList.Add(path);
                        }
                    }
                    else
                    {
                        folderList.Remove(path);

                        if (SlideSub.IsChecked == true)
                        {
                            RemoveSubFolders(directory);
                        }
                    }
                }
            }

            // Agregar subfolders si se activa la opción
            if (SlideSub.IsChecked == true)
            {
                AddSubFolders();
            }

            NCount();

            // Si la opción de configurar a portable está activada, desactivarla
            if (addrem && SlidePortable.IsChecked == true)
            {
                SlidePortable.IsChecked = false;
            }
        }

        private void RemoveSubFolders(string dirRemove)
        {
            try
            {
                string[] subdirectories = Directory.GetDirectories(dirRemove);

                // Eliminar cada subdirectorio de la subfolderList si existe
                foreach (string subdirectory in subdirectories)
                {
                    if (subfolderList.Contains(subdirectory))
                    {
                        subfolderList.Remove(subdirectory);
                    }
                }
                NCount();
            }
            catch
            {
                // Si no permite acceso a ciertos directorios o accesos directos, ignorar
            }
        }

        private void AddSubFolders()
        {
            // Si se han arrastrado folders a la DropList...
            if (DropList.Items.Count > 0)
            {
                // Para cada ruta, obtener los subdirectorios
                foreach (string path in folderList)
                {
                    string directory = path.Replace(@"..\", userPath + @"\");
                    try
                    {
                        string[] subdirectories = Directory.GetDirectories(directory);
                        // Agregar cada subdirectorio a la lista de subfolders si no existe
                        foreach (string subdirectory in subdirectories)
                        {
                            if (!subfolderList.Contains(subdirectory))
                            {
                                subfolderList.Add(subdirectory);
                            }
                        }
                    }
                    catch
                    {
                        // Si no se puede acceder a una folder, se ignora (es posible que
                        // se necesiten permisos de administrador o sea un acceso directo)
                        Console.WriteLine(path + " Inaccessible");
                        ignore.Add(path);
                    }
                }

                // Eliminar los elementos ignorados de la folderList
                foreach (string ignoreitem in ignore)
                {
                    folderList.Remove(ignoreitem);
                }

                DropList.Items.Refresh();
                NCount();
            }
        }

        private void NCount()
        {
            // Obtener el conteo total de los folders a personalizar
            int total = folderList.Count + subfolderList.Count;
            Dot.Value = total;
            Dotsub.Value = subfolderList.Count;
        }

        private void Clear()
        {
            folderList.Clear();
            subfolderList.Clear();
            icoPath = null;
            isRestore = false;
            Iconpic.Source = null;
            CBtn_Letter.Text = Properties.Resources.C;
            CBtn_Text.Text = Properties.Resources.ustomize;
            LabelSub.Content = Properties.Resources.IncludeSubfolders;
            LabelPortable.Content = Properties.Resources.ConfigureAsPortable;
            Documents.IsChecked = false;
            Pictures.IsChecked = false;
            Music.IsChecked = false;
            Videos.IsChecked = false;
            Desktop.IsChecked = false;
            SlidePortable.IsChecked = false;
            SlideSub.IsChecked = false;
            Icon_border.Visibility = Visibility.Visible;
            Icon_cross.Visibility = Visibility.Visible;
            NCount();
        }

        private BitmapImage BitmapToSource(Bitmap bitmap)
        {
            using MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            BitmapImage bmpImg = new BitmapImage();
            bmpImg.BeginInit();
            bmpImg.CacheOption = BitmapCacheOption.OnLoad;
            bmpImg.StreamSource = ms;
            bmpImg.EndInit();
            bmpImg.Freeze();
            bitmap.Dispose();
            return bmpImg;
        }

        // Events: ↓ ↓ ↓

        #region Checked and Unchecked
        private void Documents_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes("Documents", true);
        }

        private void Pictures_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes("Pictures", true);
        }

        private void Music_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes("Music", true);
        }

        private void Videos_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes("Videos", true);
        }

        private void Desktop_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes("Desktop", true);
        }

        private void Documents_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes("Documents", false);
        }

        private void Pictures_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes("Pictures", false);
        }

        private void Music_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes("Music", false);
        }

        private void Videos_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes("Videos", false);
        }

        private void Desktop_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes("Desktop", false);
        }
        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Aplicar bordes redondeados dependiendo del SO
            Config.RoundCorners(Bg1, Bg2, Border1, Border2, Deco1, Deco2);

            DropList.ItemsSource = folderList;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Mover ventana sin bordes
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void DropList_Drop(object sender, DragEventArgs e)
        {
            string[] dropFolders = (string[])e.Data.GetData(DataFormats.FileDrop);
            int warnMssg = 0;

            // Para cada directorio en los folders arrastrados
            foreach (string directory in dropFolders)
            {
                // Si es un directorio...
                if (Directory.Exists(directory))
                {
                    // Si el directorio tiene permisos de escritura y modificación
                    // o si no es solo la carperta de usuario
                    if (DirectoryPermissions(directory) && (directory != userPath))
                    {
                        // Acortar eliminando ruta con nombre de usuario
                        string folder = directory.Replace(userPath + @"\", @"..\");

                        // Evitar que se agreguen carpetas principales que tienen un icono predeterminado
                        switch (folder)
                        {
                            case @"..\Searches":
                            case @"..\Contacts":
                            case @"..\Downloads":
                            case @"..\Documents":
                            case @"..\Desktop":
                            case @"..\Favorites":
                            case @"..\Pictures":
                            case @"..\Saved Games":
                            case @"..\Music":
                            case @"..\3D Objects":
                            case @"..\Videos":
                            case @"..\Links":
                                warnMssg++;
                                break;
                            default:
                                // Agregar las carpetas a la folderList, si no existen
                                if (!folderList.Contains(folder))
                                {
                                    folderList.Add(folder);
                                }
                                break;
                        }

                        // Si está activada la opción de subcarpetas, agregarlas
                        if (SlideSub.IsChecked == true)
                        {
                            AddSubFolders();
                        }
                    }
                    else
                    {
                        warnMssg++;
                    }
                }
                else
                {
                    // Do nothing
                }
            }

            if (warnMssg > 0)
            {
                Console.WriteLine("Skipped folders: " + warnMssg + " Carpetas omitidas: " + warnMssg);
                Growl.WarningGlobal(warnMssg + Properties.Resources.FoldersHaveBeenSkipped);
            }
            NCount();
        }

        private void ChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            string initialDir = Config.isIntalled ? Config.appData + "\\Folders" : AppDomain.CurrentDomain.BaseDirectory + @"Folders";

            OpenFileDialog filedialog = new OpenFileDialog
            {
                Title = Properties.Resources.ChooseIcon,
                Filter = Properties.Resources.Icons + " (*.ico)|*.ico",
                InitialDirectory = initialDir
            };

            if (filedialog.ShowDialog() == true)
            {
                icoPath = Path.GetFullPath(filedialog.FileName);

                // Deshacer cambios para restaurar si se oprimió el botón.
                if (isRestore)
                {
                    CBtn_Letter.Text = Properties.Resources.C;
                    CBtn_Text.Text = Properties.Resources.ustomize;
                    LabelSub.Content = Properties.Resources.IncludeSubfolders;
                    LabelPortable.Content = Properties.Resources.ConfigureAsPortable;
                    isRestore = false;
                }

                // Convertir el icono seleccionado a PNG.
                using Bitmap icoBmp = new Bitmap(icoPath);

                Iconpic.Source = BitmapToSource(icoBmp);
                Icon_border.Visibility = Visibility.Hidden;
                Icon_cross.Visibility = Visibility.Hidden;
                Console.WriteLine("Ico: " + icoPath);
            }
        }

        private void RestoreBtn_Click(object sender, RoutedEventArgs e)
        {
            isRestore = true;
            CBtn_Letter.Text = "R";
            CBtn_Text.Text = Properties.Resources.estore;
            LabelSub.Content = Properties.Resources.RestoreSubfolders;
            LabelPortable.Content = Properties.Resources.RestoreFromPortable;
            if (Icon_border.Visibility == Visibility.Hidden)
            {
                Icon_border.Visibility = Visibility.Visible;
                Icon_cross.Visibility = Visibility.Visible;
            }
            Icon_cross.Visibility = Visibility.Hidden;
            Iconpic.Source = (ImageSource)Application.Current.Resources["Restore-icon"];
            NCount();
        }

        private async void Customize_Click(object sender, RoutedEventArgs e)
        {
            // Si no se ha elegido icono o no se restaurará, regresar
            if (icoPath == null && isRestore == false)
            {
                return;
            }
            else if (Dot.Value > 600 && Config.message)
            {
                // Mensaje de confirmación para más de 600 carpetas, el contenido
                // de los botones está invertido debido al color e importancia.
                var mssg = HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                {
                    Caption = Properties.Resources.TooManyFolders,
                    Message = Properties.Resources.ThereAreMoreThan600Folders,
                    IconBrushKey = ResourceToken.PrimaryBrush,
                    IconKey = ResourceToken.WarningGeometry,
                    Button = MessageBoxButton.YesNo,
                    YesContent = "No",
                    NoContent = Properties.Resources.Yes
                });

                if (mssg == MessageBoxResult.Yes)
                {
                    return;
                }
            }

            WaitDialog wait = new WaitDialog() { Owner = this };

            // Evita que salga Wait antes del mensaje límite en portables
            if (!isPortable)
            {
                wait.Show();
                await Task.Delay(1);
            }

            // Si la opción de subcarpetas está activada, agregarlas a la folderList principal
            if (SlideSub.IsChecked == true)
            {
                foreach (string directory in subfolderList)
                {
                    folderList.Add(directory);
                }
            }

            // En caso de que sea portable, se hace una copia de la ruta del icono y se modifica icoPath
            string icoPathBackup = "";

            if (isPortable)
            {
                icoPathBackup = icoPath;
                icoPath = "CF_Icon " + Path.GetFileName(icoPath);
                if (Dot.Value > 30)
                {
                    _ = HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                    {
                        Caption = Properties.Resources.TooManyFolders,
                        Message = Properties.Resources.OnlyAllowsLessThan30Folders,
                        IconBrushKey = ResourceToken.PrimaryBrush,
                        IconKey = ResourceToken.WarningGeometry,
                        Button = MessageBoxButton.OK,
                        ConfirmContent = Properties.Resources.OK,
                    });
                    return;
                }
                else
                {
                    wait.Show();
                    await Task.Delay(1);
                }
            }

            // Personalizar los folders de la lista principal.
            foreach (string folder in folderList)
            {
                string fullPath = folder.Replace(@"..\", userPath + @"\") + @"\";

                // Si el switch de Portable está activado...
                if (isPortable)
                {
                    if (!isRestore)
                    {
                        // Copiar el icono y agregarle el atributo de oculto
                        try
                        {
                            File.Copy(icoPathBackup, fullPath + icoPath);
                        }
                        catch
                        {
                            File.Delete(fullPath + icoPath);
                            File.Copy(icoPathBackup, fullPath + icoPath);
                        }
                        File.SetAttributes(Path.Combine(fullPath + icoPath), File.GetAttributes(icoPathBackup) | FileAttributes.Hidden);
                    }
                    else
                    {
                        // Si se restaura, borrar cualquier icono que se haya copiado anteriormente
                        string[] files = Directory.GetFiles(fullPath);
                        foreach (string file in files)
                        {
                            if (file.Contains("CF_Icon"))
                            {
                                File.Delete(file);
                            }
                        }
                    }
                }

                // Personalizar carpetas
                ApplyFolderSettings(fullPath);
            }

            // Growl message dependiendo de si se han personalizado o restaurado
            if (!isRestore)
            {
                Growl.SuccessGlobal(Properties.Resources.FoldersHaveBeenCustomized);
            }
            else
            {
                Growl.SuccessGlobal(Properties.Resources.FoldersHaveBeenRestored);
            }

            Clear();
            wait.Close();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            // Eliminar la carpeta seleccionada
            if (DropList.SelectedItems.Count == 1)
            {
                string selected = DropList.SelectedItem.ToString();
                folderList.Remove(selected);

                // Eliminar las subcarpetas también, si el switch está activado
                if (SlideSub.IsChecked == true)
                {
                    string directory = selected.Replace(@"..\", userPath + @"\");
                    RemoveSubFolders(directory);
                }

                NCount();
            }
        }

        private async void SlideSub_Checked(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Cursor != Cursors.AppStarting)
                {
                    Cursor = Cursors.AppStarting;
                }
            });

            await Task.Delay(1);
            AddSubFolders();

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Cursor != Cursors.Arrow)
                {
                    Cursor = Cursors.Arrow;
                }
            });
        }

        private void SlideSub_Unchecked(object sender, RoutedEventArgs e)
        {
            // Eliminar la lista de subfolders cuando se desactiva la opción
            subfolderList.Clear();
            NCount();
        }

        private void SlidePortable_Checked(object sender, RoutedEventArgs e)
        {
            isPortable = true;
        }

        private void SlidePortable_Unchecked(object sender, RoutedEventArgs e)
        {
            isPortable = false;
        }

        private void SlidePortable_Click(object sender, RoutedEventArgs e)
        {
            // Evitar que se active la opción portable si se han activado carpetas especiales
            if (Documents.IsChecked == true || Pictures.IsChecked == true || Music.IsChecked == true
                || Videos.IsChecked == true || Desktop.IsChecked == true)
            {
                SlidePortable.IsChecked = false;
                return;
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            HelpDialog dlgextract = new HelpDialog() { Owner = this };
            dlgextract.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Growl.ClearGlobal();
            Close();
        }

        private void ClearList_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            About dlgextract = new About() { Owner = this };
            dlgextract.Show();
        }

        private void LabelSub_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Cambiar de posición el texto dependiendo del idioma
            if (!isRestore)
            {
                Dotsub.Margin = Config.currentLan == "en" ? new Thickness(476, 266, 88, 0)
                    : new Thickness(478, 266, 88, 0);
            }
            else
            {
                Dotsub.Margin = Config.currentLan == "en" ? new Thickness(476, 266, 88, 0)
                    : new Thickness(494, 266, 88, 0);
            }

        }
    }
}