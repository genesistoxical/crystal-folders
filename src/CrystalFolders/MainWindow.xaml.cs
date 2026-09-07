using HandyControl.Controls;
using HandyControl.Data;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Globalization;
using System.Windows.Data;
using System.Runtime.InteropServices;
using CrystalFolders.Properties;
using Brushes = System.Windows.Media.Brushes;
using Path = System.IO.Path;

namespace CrystalFolders
{
    public partial class MainWindow : System.Windows.Window
    {
        /// <summary>
        /// Change Folder Icons
        /// </summary>

        public static string icoPath, userPath;
        public static bool isPortable, isRestore, isApplyingPortable;
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

        internal static string ResolveFolderPath(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
                return folder;

            return folder.StartsWith(@"..\", StringComparison.Ordinal)
                ? folder.Replace(@"..\", userPath + @"\")
                : folder;
        }

        internal static BitmapImage LoadResourceIcon(string name) =>
            new BitmapImage(new Uri($"pack://application:,,,/Resources/{name}", UriKind.Absolute));

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct ShellFileInfo
        {
            public IntPtr IconHandle;
            public int IconIndex;
            public uint Attributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string DisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string TypeName;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SHGetFileInfo(
            string path,
            uint fileAttributes,
            ref ShellFileInfo fileInfo,
            uint fileInfoSize,
            uint flags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr iconHandle);

        internal static ImageSource LoadFolderIcon(string folderPath)
        {
            ShellFileInfo fileInfo = new ShellFileInfo();
            const uint SHGFI_ICON = 0x000000100;
            const uint SHGFI_SMALLICON = 0x000000001;

            if (SHGetFileInfo(
                    folderPath,
                    0,
                    ref fileInfo,
                    (uint)Marshal.SizeOf<ShellFileInfo>(),
                    SHGFI_ICON | SHGFI_SMALLICON) == IntPtr.Zero
                || fileInfo.IconHandle == IntPtr.Zero)
            {
                return null;
            }

            try
            {
                ImageSource image = Imaging.CreateBitmapSourceFromHIcon(
                    fileInfo.IconHandle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromWidthAndHeight(16, 16));
                image.Freeze();
                return image;
            }
            finally
            {
                DestroyIcon(fileInfo.IconHandle);
            }
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

        private void CheckBoxes(Environment.SpecialFolder specialFolder, bool addrem)
        {
            try // try catch como solución temporal al error "la app crashea"
            {
                // Obtener la ruta real de la biblioteca
                string primaryPath = Environment.GetFolderPath(specialFolder);

                // Si la ruta está vacía (por ej. una biblioteca no existe), salir
                if (string.IsNullOrEmpty(primaryPath)) return;

                // Lista de directorios para el folder especificado
                string[] directories = Directory.GetDirectories(primaryPath + @"\", "*");
                // Para cada directorio en la lista de directorios...
                foreach (string directory in directories)
                {
                    // Crear una ruta corta si está dentro del userPath, sino usar la absoluta
                    string path;
                    if (directory.StartsWith(userPath + @"\", StringComparison.OrdinalIgnoreCase))
                    {
                        path = directory.Replace(userPath + @"\", @"..\");
                    }
                    else
                    {
                        path = directory;
                    }

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
            catch
            {
                if (addrem)
                {
                    // Mensaje de que no se han podido agregar carpetas
                    Growl.InfoGlobal(Properties.Resources.Oops);
                }
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
                    // Restaurar ruta absoluta si es relativa
                    string directory;
                    if (path.StartsWith(@"..\"))
                    {
                        directory = path.Replace(@"..\", userPath + @"\");
                    }
                    else
                    {
                        directory = path;
                    }

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
            isApplyingPortable = false;
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
            CheckBoxes(Environment.SpecialFolder.MyDocuments, true);
        }

        private void Pictures_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes(Environment.SpecialFolder.MyPictures, true);
        }

        private void Music_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes(Environment.SpecialFolder.MyMusic, true);
        }

        private void Videos_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes(Environment.SpecialFolder.MyVideos, true);
        }

        private void Desktop_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxes(Environment.SpecialFolder.Desktop, true);
        }

        private void Documents_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes(Environment.SpecialFolder.MyDocuments, false);
        }

        private void Pictures_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes(Environment.SpecialFolder.MyPictures, false);
        }

        private void Music_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes(Environment.SpecialFolder.MyMusic, false);
        }

        private void Videos_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes(Environment.SpecialFolder.MyVideos, false);
        }

        private void Desktop_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxes(Environment.SpecialFolder.Desktop, false);
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

        // Doble Click en la lista
        private void DropList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Por si clicka fuera de un elemento
            if (!(DropList.SelectedItem is string selectedFolder))
                return;

            string folderPath = Path.GetFullPath(ResolveFolderPath(selectedFolder));
            if (!Directory.Exists(folderPath))
                return;

            Process.Start(new ProcessStartInfo
            {
                FileName = folderPath,
                UseShellExecute = true
            });
        }

        // Acción de Dropear las folders en la Lista
        private void DropList_Drop(object sender, DragEventArgs e)
        {
            string[] dropFolders = (string[])e.Data.GetData(DataFormats.FileDrop);
            int warnMssg = 0;

            // Para cada directorio en los folders arrastrados
            foreach (string directory in dropFolders)
            {
                if (!Directory.Exists(directory))
                    continue;
                
                // Obtener la fecha de modificación de la carpeta
                DirectoryInfo folderInfo = new DirectoryInfo(directory);
                DateTime modifDate = folderInfo.LastWriteTime;

                // Si el directorio tiene permisos de escritura y modificación
                // o si no es solo la carperta de usuario
                if (DirectoryPermissions(directory) && directory != userPath)
                {
                    // Acortar eliminando ruta con nombre de usuario si es que está dentro
                    string folder = directory.StartsWith($@"{userPath}\", StringComparison.OrdinalIgnoreCase) 
                        ? directory.Replace($@"{userPath}\", @"..\") 
                        : directory;

                    // Evitar que se agreguen carpetas principales del sistema que tienen un icono predeterminado
                    bool isSpecialFolder = IsProtectedFolderPath(directory);

                    if (isSpecialFolder)
                        warnMssg++;
                    // Agregar las carpetas a la folderList, si no estaban ya
                    else if (!folderList.Contains(folder))
                        folderList.Add(folder);

                    // Si está activada la opción de subcarpetas, agregarlas
                    if (SlideSub.IsChecked == true) 
                        AddSubFolders();
                }
                else
                    warnMssg++;

                try
                {
                    // Regresar la fecha de modificación de la carpeta
                    Console.WriteLine($"Dir: {directory}");
                    Directory.SetLastWriteTime(directory, modifDate);
                }
                catch
                {
                    Console.WriteLine($"Folder LastWriteTime Error: {directory}");
                }
                
                // Sort Order pa que quede bonico
                string[] sortedFolders = folderList
                    .OrderBy(folder =>
                    {
                        Icons.FolderIconInfo info = Icons.AnalyzeFolder(ResolveFolderPath(folder));
                        return info.HasConfiguredIcon
                            ? info.HasIconFileInside ? 0 : 1
                            : info.HasIconFileInside ? 2 : 3;
                    })
                    .ToArray();

                folderList.Clear();
                foreach (string sortedFolder in sortedFolders)
                    folderList.Add(sortedFolder);
            }

            if (warnMssg > 0)
            {
                Console.WriteLine("Skipped folders: " + warnMssg + " Carpetas omitidas: " + warnMssg);
                Growl.WarningGlobal(warnMssg + Properties.Resources.FoldersHaveBeenSkipped);
            }
            NCount();
        }

        
        #region ACTION BUTTONS

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
            
            // Mostrar el icono de Restaurar
            Icon_border.Visibility = Visibility.Visible;
            Icon_cross.Visibility = Visibility.Hidden;
            Iconpic.Source = (ImageSource)Application.Current.Resources["Restore-icon"];
            NCount();
        }

        private void AutoApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            // Buscar iconos en las carpetas y en cualquiera de sus subcarpetas.
            if (!folderList.Any(folder => Directory.EnumerateFiles(
                    ResolveFolderPath(folder), "*.ico", SearchOption.AllDirectories).Any()))
                return;
            
            isApplyingPortable = true;
            // isApplyingPortable es una variable estática, por lo que WPF no
            // vuelve a evaluar automáticamente el converter del FontWeight.
            DropList.Items.Refresh();
            CBtn_Letter.Text = "A";
            CBtn_Text.Text = Properties.Resources.pply;
            LabelSub.Content = Properties.Resources.ApplySubfolders;
            LabelPortable.Content = Properties.Resources.ConfigureAsPortable;
            
            // Mostrar el icono de Portable (caracol)
            Icon_border.Visibility = Visibility.Visible;
            Icon_cross.Visibility = Visibility.Hidden;
            Iconpic.Source = (ImageSource)Application.Current.Resources["folder-automation"];
            
            NCount();
        }

        #endregion
        

        private async void Customize_Click(object sender, RoutedEventArgs e)
        {
            // Si no se ha elegido icono o no se restaurará, regresar
            if (icoPath == null && !isRestore && !isApplyingPortable)
                return;

            if (Dot.Value > 600 && Config.message)
            {
                // Mensaje de confirmación para más de 600 carpetas, el contenido
                // de los botones está invertido debido al color e importancia.
                MessageBoxResult msg = HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
                {
                    Caption = Properties.Resources.TooManyFolders,
                    Message = Properties.Resources.ThereAreMoreThan600Folders,
                    IconBrushKey = ResourceToken.PrimaryBrush,
                    IconKey = ResourceToken.WarningGeometry,
                    Button = MessageBoxButton.YesNo,
                    YesContent = "No",
                    NoContent = Properties.Resources.Yes
                });

                if (msg == MessageBoxResult.Yes)
                    return;
            }

            WaitDialog wait = new WaitDialog { Owner = this };

            // Evita que salga Wait antes del mensaje límite en portables
            if (!isPortable)
            {
                wait.Show();
                await Task.Delay(1);
            }

            // Si la opción de subcarpetas está activada, agregarlas a la folderList principal
            if (SlideSub.IsChecked == true)
                foreach (string directory in subfolderList) 
                    folderList.Add(directory);

            // En caso de que sea portable, se hace una copia de la ruta del icono y se modifica icoPath
            icoPath = isPortable ? $"CF_Icon {Path.GetFileName(icoPath)}" : icoPath;
            
            // Cuidao con + de 30 folders, mensaje de error y atpc
            if (isPortable && Dot.Value > 30)
            {
                HandyControl.Controls.MessageBox.Show(new MessageBoxInfo
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
            
            if (isPortable)
            {
                wait.Show();
                await Task.Delay(1);
            }
            
            // Personalizar los folders de la lista principal.
            foreach (string folder in folderList)
            {
                // Reconstruir la ruta absoluta (relativa o ya absoluta)
                string fullPath = folder.StartsWith(@"..\")
                    ? $@"{folder.Replace(@"..\", $@"{userPath}\")}\" 
                    : $@"{folder}\";
                
                Icons.FolderIconInfo info = Icons.AnalyzeFolder(fullPath);
                
                // Si está aplicando iconos pero justo esta carpeta no tiene ignórala
                if (isApplyingPortable && !info.HasIconFileInside)
                    continue;

                // Si el switch de Portable está activado...
                if (isPortable)
                {
                    if (isRestore)
                    {
                        // Si se restaura, borrar cualquier icono que se haya copiado anteriormente
                        if (info.HasPortableIconInside) 
                            File.Delete(info.PortableIconPath);
                    }
                    else
                    {
                        // Copiar el icono y agregarle el atributo de oculto
                        if (!info.HasPortableIconInside)
                        {
                            info.PortableIconPath = Icons.IconToPortableIcon(info.FirstIconInsidePath);
                            try
                            {
                                File.Copy(info.FirstIconInsidePath, info.PortableIconPath);
                            }
                            catch
                            {
                                File.Delete(info.PortableIconPath);
                                File.Copy(info.FirstIconInsidePath, info.PortableIconPath);
                            }
                        }
                        
                        File.SetAttributes(Path.Combine(info.PortableIconPath),
                            File.GetAttributes(info.PortableIconPath) | FileAttributes.Hidden);
                    }
                }
                
                if (isApplyingPortable)
                {
                    if (!info.HasIconFileInside)
                        continue;
                    
                    if (!info.HasPortableIconInside)
                    {
                        // Copiar el icono y agregarle el atributo de oculto
                        string baseIconPath = info.FirstIconInsidePath;
                        string portableIconPath = Icons.IconToPortableIcon(info.FirstIconInsidePath);
                        Console.WriteLine(portableIconPath);
                        try
                        {
                            File.Copy(baseIconPath, portableIconPath);
                            File.Delete(baseIconPath);
                            info.PortableIconPath = portableIconPath;
                            info.FirstIconInsidePath = portableIconPath;
                        }
                        catch
                        {
                            File.Delete(portableIconPath);
                            File.Copy(baseIconPath, portableIconPath);
                            File.Delete(baseIconPath);
                            info.PortableIconPath = portableIconPath;
                            info.FirstIconInsidePath = portableIconPath;
                        }
                    }

                    icoPath = info.PortableIconPath;
                }

                // Obtener la fecha de modificación de la carpeta.
                // NOTA: Desafortunadamente esto evitará que el caché se
                // actualice y no mostrará cambios al personalizar las carpetas.
                DirectoryInfo folderInfo = new DirectoryInfo(fullPath);
                DateTime modifDate = folderInfo.LastWriteTime;

                // Al agregar un milisegundo extra, el caché de las carpetas SÍ
                // se actualizará, pero mantendrá la fecha de modificación
                // prácticamente intacta.
                modifDate = modifDate.AddMilliseconds(1);

                // Personalizar carpetas
                Icons.ConfigureIconToFolder(fullPath, isRestore ? "" : icoPath);

                try
                {
                    // Regresar la fecha de modificación de la carpeta
                    Directory.SetLastWriteTime(fullPath, modifDate);
                }
                catch
                {
                    Console.WriteLine($"Folder SetLastWriteTime Error: {modifDate} : {fullPath}");
                }
            }

            // Growl message dependiendo de si se han personalizado o restaurado
            Growl.SuccessGlobal(isRestore
                ? Properties.Resources.FoldersHaveBeenRestored
                : Properties.Resources.FoldersHaveBeenCustomized);

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
                    string directory = selected.StartsWith(@"..\")
                        ? selected.Replace(@"..\", $@"{userPath}\")
                        : selected;
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
                    Cursor = Cursors.AppStarting;
            });

            await Task.Delay(1);
            AddSubFolders();

            Application.Current.Dispatcher.Invoke(() =>
            {
                if (Cursor != Cursors.Arrow) 
                    Cursor = Cursors.Arrow;
            });
        }

        private void SlideSub_Unchecked(object sender, RoutedEventArgs e)
        {
            // Eliminar la lista de subfolders cuando se desactiva la opción
            subfolderList.Clear();
            NCount();
        }

        private void SlidePortable_Checked(object sender, RoutedEventArgs e) => isPortable = true;
        private void SlidePortable_Unchecked(object sender, RoutedEventArgs e) => isPortable = false;

        private bool AnySpecialFolderChecked => 
            Documents.IsChecked == true || Pictures.IsChecked == true || Music.IsChecked == true
            || Videos.IsChecked == true || Desktop.IsChecked == true;
        
        private void SlidePortable_Click(object sender, RoutedEventArgs e)
        {
            // Evitar que se active la opción portable si se han activado carpetas especiales
            if (AnySpecialFolderChecked)
                SlidePortable.IsChecked = false;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            HelpDialog dlgextract = new HelpDialog { Owner = this };
            dlgextract.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Growl.ClearGlobal();
            Close();
        }

        private void ClearList_Click(object sender, RoutedEventArgs e) => Clear();

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            About dlgextract = new About { Owner = this };
            dlgextract.Show();
        }

        private void LabelSub_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Cambiar de posición el texto dependiendo del idioma
            if (!isRestore)
                Dotsub.Margin = Config.currentLan == "en"
                    ? new Thickness(476, 266, 88, 0)
                    : new Thickness(478, 266, 88, 0);
            else
                Dotsub.Margin = Config.currentLan == "en"
                    ? new Thickness(476, 266, 88, 0)
                    : new Thickness(494, 266, 88, 0);

        }
        
        #region PROTECTED FOLDERS

        // Carpetas Protegidas por Windows a las que no se le puede cambiar el Icono
        private static readonly Environment.SpecialFolder[] ProtectedFolders = {
            Environment.SpecialFolder.MyDocuments, Environment.SpecialFolder.MyPictures,
            Environment.SpecialFolder.MyMusic, Environment.SpecialFolder.MyVideos,
            Environment.SpecialFolder.Desktop, Environment.SpecialFolder.Favorites,
            Environment.SpecialFolder.Recent // Representa Searches y/o Links en entornos viejos/específicos dependiendo SO. Lo ideal es match rudo.
        };

        // Otras Carpetas más concretas que vamos a ignorar a la hora de cambiar iconos porque son mas propias de Windows
        // (Que te jodan Microsoft!!)
        private static string[] OtherProtectedFolders => new[]
        {
            Path.Combine(userPath, "Downloads"),
            Path.Combine(userPath, "Contacts"),
            Path.Combine(userPath, "Saved Games"),
            Path.Combine(userPath, "3D Objects"),
            Path.Combine(userPath, "Links"),
            Path.Combine(userPath, "Searches")
        };
        
        internal static bool IsProtectedFolderPath(string dirPath) =>
            ProtectedFolders
                .Select(Environment.GetFolderPath)
                .Any(sFolderPath => 
                    !string.IsNullOrEmpty(sFolderPath) 
                    && dirPath.Equals(sFolderPath, StringComparison.OrdinalIgnoreCase))
            || OtherProtectedFolders.Any(ep => dirPath.Equals(ep, StringComparison.OrdinalIgnoreCase)); 
        
        #endregion
        
        
        
    }

    #region FOLDER PATH STYLE

    public sealed class PathFontWeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!MainWindow.isApplyingPortable) return Brushes.Black;
            
            string folderPath = MainWindow.ResolveFolderPath(value as string);
            Icons.FolderIconInfo info = Icons.AnalyzeFolder(folderPath);
            
            return info.HasPortableIconInside
                ? Brushes.Black
                : info.HasIconFileInside
                    ? Brushes.DarkSlateBlue
                    : Brushes.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }

    #endregion
    
    
    #region Folder State Icons

    // Convierte la ruta de cada folder en una abreviacion con puntos suspensivos delante para que quepa bien y ver el final de la ruta
    public sealed class LeadingEllipsisConverter : IValueConverter
    {
        private const int MaxVisibleCharacters = 24;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            if (string.IsNullOrEmpty(path) || path.Length <= MaxVisibleCharacters)
                return path;

            return "..." + path.Substring(path.Length - MaxVisibleCharacters);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }
    
    // Icono de la Carpeta
    public sealed class FolderStateIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string folderPath = MainWindow.ResolveFolderPath(value as string);
                if (MainWindow.IsProtectedFolderPath(folderPath))
                    return MainWindow.LoadResourceIcon("folder-protected.png");

                return MainWindow.LoadFolderIcon(folderPath);
            }
            catch
            {
                return MainWindow.LoadResourceIcon("folder-custom.png");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }

    // Icono que denota la ubicación del Icono configurado en la carpeta
    // Portable: Caracol || Externo: Link || No tiene: no se muestra 
    public sealed class IconLocationIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string folderPath = MainWindow.ResolveFolderPath(value as string);
                Icons.FolderIconInfo info = Icons.AnalyzeFolder(folderPath);

                if (info.HasConfiguredIcon)
                    return MainWindow.LoadResourceIcon(
                        info.ConfiguredIconIsInside
                            ? "snail.png"
                            : "link.png");
                
                return info.HasIconFileInside ? MainWindow.LoadResourceIcon("snail.png") : null;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }
    
    // Tooltip del icono de la carpeta con su ruta
    public sealed class FolderStateTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string folderPath = MainWindow.ResolveFolderPath(value as string);
                Icons.FolderIconInfo info = Icons.AnalyzeFolder(folderPath);

                return info.HasConfiguredIcon 
                    ? info.ConfiguredIconIsPortable 
                        ? $"./{Path.GetFileName(info.PortableIconPath)}"
                        : info.ConfiguredIconPath 
                    : null;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }

    // Tooltip del Icono de la Ubicación del Icono
    public sealed class IconLocationTooltipConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string folderPath = MainWindow.ResolveFolderPath(value as string);
                Icons.FolderIconInfo info = Icons.AnalyzeFolder(folderPath);
                return info.HasConfiguredIcon
                    ? info.HasPortableIconInside
                        ? $"{Resources.PortableOnFolderToolTip}: {Path.GetFileName(info.ConfiguredIconPath)}" 
                        : $"{Resources.ExternalIconTooltip}: {Path.GetFileName(info.ConfiguredIconPath)}"
                    : $"{Resources.IconOnFolderToolTip}: {Path.GetFileName(info.HasPortableIconInside ? info.PortableIconPath : info.FirstIconInsidePath)}";
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            Binding.DoNothing;
    }

    #endregion
}
