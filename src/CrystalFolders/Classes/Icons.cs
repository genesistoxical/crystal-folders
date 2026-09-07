using System;
using System.IO;
using System.Linq;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace CrystalFolders
{
    public static class Icons
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        public static extern uint SHGetSetFolderCustomSettings(ref ShFolderCustomSettings pfcs, string pszPath, uint dwReadWrite);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct ShFolderCustomSettings
        {
            public uint dwSize;
            public uint dwMask;
            public IntPtr pvid;
            public string pszWebViewTemplate;
            public uint cchWebViewTemplate;
            public string pszWebViewTemplateVersion;
            public string pszInfoTip;
            public uint cchInfoTip;
            public IntPtr pclsid;
            public uint dwFlags;
            public string pszIconFile;
            public uint cchIconFile;
            public int iIconIndex;
            public string pszLogo;
            public uint cchLogo;
        }

        public const string PortablePrefix = "CF_Icon ";

        public sealed class FolderIconInfo
        {
            public bool HasIconFileInside { get; internal set; }
            public bool HasPortableIconInside { get; internal set; }
            public bool HasConfiguredIcon { get; internal set; }
            public string ConfiguredIconPath { get; internal set; }
            public string FirstIconInsidePath { get; internal set; }
            public string PortableIconPath { get; set; }
            public bool ConfiguredIconIsInside { get; internal set; }
            public bool ConfiguredIconIsPortable { get; internal set; }
        }

        public static bool IconIsPortable(string iconPath) => Path.GetFileName(iconPath).StartsWith(PortablePrefix, StringComparison.OrdinalIgnoreCase);

        public static string IconToPortableIcon(string iconPath)
        {
            if (string.IsNullOrWhiteSpace(iconPath))
                throw new ArgumentException("La ruta del icono no puede estar vacía.", nameof(iconPath));

            if (IconIsPortable(iconPath)) 
                return iconPath;

            string fullIconPath = Path.GetFullPath(iconPath);
            return Path.Combine(
                Path.GetDirectoryName(fullIconPath)!,
                PortablePrefix + Path.GetFileName(fullIconPath));
        }
        
        // Ruta del Icono configurado en la Carpeta
        public static string GetFolderIconPath(string folderPath)
        {
            string fullFolderPath = Path.GetFullPath(folderPath);

            ShFolderCustomSettings settings = new ShFolderCustomSettings
            {
                dwSize = (uint)Marshal.SizeOf<ShFolderCustomSettings>(),
                dwMask = 0x10,
                pszIconFile = new string('\0', 260),
                cchIconFile = 260
            };

            uint result = SHGetSetFolderCustomSettings(
                ref settings,
                fullFolderPath,
                0x00000001); // FCS_READ

            if (result != 0)
                return null;

            string iconPath = settings.pszIconFile?.TrimEnd('\0');
            return string.IsNullOrWhiteSpace(iconPath) ? null : iconPath;
        }

        // Analiza la Carpeta para saber si tiene un Icono Configurado
        // Si está DENTRO o FUERA de la Carpeta
        // Y si no lo tiene configurado, si hay un icono DENTRO listo para configurar
        public static FolderIconInfo AnalyzeFolder(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                throw new ArgumentException("La ruta de la carpeta no puede estar vacía.", nameof(folderPath));
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException(folderPath);

            string fullFolderPath = Path.GetFullPath(folderPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string configuredPath = GetFolderIconPath(folderPath);

            string[] iconsInside = Directory.EnumerateFiles(folderPath, "*.ico", SearchOption.TopDirectoryOnly).ToArray();

            // Info Basica
            FolderIconInfo info = new FolderIconInfo
            {
                HasIconFileInside = iconsInside.Any(),
                HasPortableIconInside = iconsInside.Any(IconIsPortable),
                ConfiguredIconPath = configuredPath,
                HasConfiguredIcon = !string.IsNullOrWhiteSpace(configuredPath)
            };
            
            // Rutas de los Iconos dentro
            if (info.HasIconFileInside)
            {
                info.FirstIconInsidePath = iconsInside.First();
                if (info.HasPortableIconInside)
                    info.PortableIconPath = iconsInside.First(IconIsPortable);
            }

            if (!info.HasConfiguredIcon)
                return info;

            // Ruta absoluta
            string resolvedPath = Path.IsPathRooted(configuredPath)
                ? Path.GetFullPath(configuredPath)
                : Path.GetFullPath(Path.Combine(fullFolderPath, configuredPath));
            string folderPrefix = fullFolderPath + Path.DirectorySeparatorChar;

            // Comprueba si el Icono Configurado esta DENTRO o es EXTERNO
            info.ConfiguredIconIsInside = resolvedPath.StartsWith(folderPrefix, StringComparison.OrdinalIgnoreCase);
            info.ConfiguredIconIsPortable = info.ConfiguredIconIsInside
                && Path.GetFileName(resolvedPath).StartsWith(PortablePrefix, StringComparison.OrdinalIgnoreCase);

            return info;
        }

        public static string[] GetIconsPathInside(string dirPath) => 
            Directory.EnumerateFiles(dirPath, "*.ico", SearchOption.TopDirectoryOnly).ToArray();

        public static void ConfigureIconToFolder(string folderPath, string iconPath)
        {
            string fullFolderPath = Path.GetFullPath(folderPath);
            string fullIconPath = string.IsNullOrEmpty(iconPath)
                ? null
                : Path.GetFullPath(iconPath);

            ShFolderCustomSettings customSettings = new ShFolderCustomSettings
            {
                dwSize = (uint)Marshal.SizeOf<ShFolderCustomSettings>(),
                dwMask = 0x10
            };

            // Poner un icono o quitarlo
            if (!string.IsNullOrEmpty(fullIconPath))
            {
                customSettings.pszIconFile = fullIconPath;
                customSettings.cchIconFile = (uint)(fullIconPath.Length + 1);
                customSettings.iIconIndex = 0;
            }
            else
            {
                // pszIconFile and iIconIndex empty
                // pszIconFile y iIconIndex vacíos
            }

            uint FCS_FORCEWRITE = 0x00000002;

            // Aplicar la configuración al Desktop.ini
            uint result = SHGetSetFolderCustomSettings(ref customSettings, fullFolderPath, FCS_FORCEWRITE);
            if (result != 0)
                throw new Win32Exception((int)result);

            if (!string.IsNullOrEmpty(fullIconPath))
                HideIcon(fullIconPath);
        }
        
        public static void HideIcon(string iconPath) => 
            File.SetAttributes(iconPath,
                File.GetAttributes(iconPath) | FileAttributes.Hidden);
    }
}
