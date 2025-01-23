using HandyControl.Themes;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Application = System.Windows.Application;

namespace CrystalFolders
{
    /// <summary>
    /// Config.ini
    /// </summary>
    internal class Config
    {
        internal static bool isIntalled;
        internal static string iniPath, appData;
        internal static string[] iniLines;
        internal static bool restart = false;

        internal static void CheckPath()
        {
            // Busca el archivo ini en la misma carpeta para saber si
            // Drop Icons está instalado o no, incluso si se tiene una
            // versión instalada y otra portable
            isIntalled = !File.Exists("Config.ini");

            // Establece las rutas de ini y dat, dependiendo de lo anterior
            appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Crystal Folders";
            iniPath = isIntalled ? appData + "\\Config.ini" : "Config.ini";
            iniLines = File.ReadAllLines(iniPath);

            Console.WriteLine("Crystal Folders is installed? " + isIntalled + " - ¿Crystal Folders está instalado? " + isIntalled);
        }

        #region Language
        internal static string currentLan, selecLan;

        internal static void Language()
        {
            // Modificar el idioma de la aplicación en base a Config.ini y establecer
            // el idioma actual en un string para no volveer a leer el archivo
            switch (iniLines[1])
            {
                case "Language = en":
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("");
                    currentLan = "en";
                    selecLan = "en";
                    break;

                case "Language = es":
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("es-419");
                    currentLan = "es";
                    selecLan = "es";
                    break;
            }

            Console.WriteLine("Current language: " + currentLan + " - Idioma actual: " + currentLan);
        }

        internal static void CheckLanguage()
        {
            // Si el idioma seleccionado no es el mismo que el actual,
            // cambiarlo y modificar el archivo .ini
            if (selecLan == currentLan == false)
            {
                switch (selecLan)
                {
                    case "en":
                        iniLines[1] = iniLines[1].Replace("es", "en");
                        File.WriteAllLines(iniPath, iniLines);
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("");
                        currentLan = "en";
                        break;

                    case "es":
                        iniLines[1] = iniLines[1].Replace("en", "es");
                        File.WriteAllLines(iniPath, iniLines);
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("es-419");
                        currentLan = "es";
                        break;
                }

                Console.WriteLine("Selected language: " + selecLan + " - Idioma seleccionado: " + selecLan);
                restart = true;
            }
        }
        #endregion

        #region Theme
        internal static string HEX;
        internal static SolidColorBrush colorBrush;
        internal static string winvers;

        internal static void GetTheme()
        {
            // Obtiene el color HEX de Config.ini
            HEX = iniLines[5];

            // Crear brush de nuevo color y aplicarlo al tema
            colorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(HEX));
            ThemeManager.Current.AccentColor = colorBrush;
            Application.Current.Resources["Primary"] = colorBrush;
        }

        internal static void SetTheme()
        {
            // Establece un nuevo color del tema, remplazando dígitos
            iniLines[5] = iniLines[5].Replace(iniLines[5], HEX);
            File.WriteAllLines(iniPath, iniLines);

            // Crear brush de nuevo color y aplicarlo al tema
            ThemeManager.Current.AccentColor = colorBrush;
            Application.Current.Resources["Primary"] = colorBrush;
        }

        internal static void RoundCorners(Border bg1, Border bg2, Border border1, Border border2, Border deco1, Border deco2)
        {
            // Desactivar bordes redondeados en las versiones METRO de Win
            if (winvers.Contains("10") || winvers.Contains("8"))
            {
                bg1.CornerRadius = new CornerRadius(0, 0, 0, 0);
                border1.CornerRadius = new CornerRadius(0, 0, 0, 0);
                deco1.CornerRadius = new CornerRadius(0, 0, 0, 0);

                if (bg2 is null && border2 is null && deco2 is null)
                {
                    // Si no hay controles de borde para redondear, no hacer nada.
                }
                else
                {
                    bg2.CornerRadius = new CornerRadius(0, 0, 0, 0);
                    border2.CornerRadius = new CornerRadius(0, 0, 0, 0);
                    deco2.CornerRadius = new CornerRadius(0, 0, 0, 0);
                }
            }
        }
        #endregion

        #region Message
        internal static bool message;

        internal static void GetMessage()
        {
            // Mensaje de confirmación para más de 600 carpetas. Si se modifica
            // el archivo Config.ini, este no se mostrará nuevamente.
            if (iniLines[2].Contains("Message = true"))
            {
                message = true;
            }
            else
            {
                message = false;
            }
        }
        #endregion
    }
}
