using System.IO;
using NUnit.Framework;

namespace CrystalFolders.Tests
{
    [TestFixture]
    public class IconsTests
    {
        // Carpeta con los Iconos externos
        private const string TestIconsPath = "Test Icons";
        
        // Carpeta con Carpetas para testeo
        private const string TestFolderPath = "Test Folders";
        
        private static string TestFolderUnconfigured => Path.Combine(TestFolderPath, "test_unconfigured");
        private static string TestFolderUnconfiguredWithIcon => Path.Combine(TestFolderPath, "test_unconfigured_with_icon");
        private static string TestFolderUnconfiguredWithPortableIcon => Path.Combine(TestFolderPath, "test_unconfigured_with_portable_icon");
        private static string TestFolderConfiguredWithExternalIcon => Path.Combine(TestFolderPath, "test_configured_external_icon");
        private static string TestFolderConfiguredWithPortableIcon => Path.Combine(TestFolderPath, "test_configured_portable_icon");
        
        private static string[] TestFolderPaths => new[]
        {
            TestFolderUnconfigured,
            TestFolderUnconfiguredWithIcon,
            TestFolderUnconfiguredWithPortableIcon,
            TestFolderConfiguredWithExternalIcon,
            TestFolderConfiguredWithPortableIcon,
        };

        [SetUp]
        public void SetUp()
        {
            // Si no estan las carpetas de testing las crea
            if (!Directory.Exists(TestFolderPath))
                Directory.CreateDirectory(TestFolderPath);

            foreach (string folderPath in TestFolderPaths)
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

            // Mete un Icon cualquiera sin prefijo
            string[] icons = Icons.GetIconsPathInside(TestFolderUnconfiguredWithIcon);
            if (icons.Length == 0)
            {
                string icon = Icons.GetIconsPathInside(TestIconsPath)[0];
                File.Copy(icon, Path.Combine(TestFolderUnconfiguredWithIcon, "icon.ico"));
            }
            
            // Mete un Icon con el prefijo que lo marca como portable
            icons = Icons.GetIconsPathInside(TestFolderUnconfiguredWithPortableIcon);
            if (icons.Length == 0)
            {
                string icon = Icons.GetIconsPathInside(TestIconsPath)[0];
                File.Copy(icon, Path.Combine(TestFolderUnconfiguredWithPortableIcon, $"{Icons.PortablePrefix}icon.ico"));
            }
            
            // Mete un icon, lo hace portable y lo configura a la Carpeta
            icons = Icons.GetIconsPathInside(TestFolderConfiguredWithPortableIcon);
            if (icons.Length == 0)
            {
                string icon = Icons.GetIconsPathInside(TestIconsPath)[0];
                string portableIcon = Path.Combine(TestFolderConfiguredWithPortableIcon, $"{Icons.PortablePrefix}icon.ico");
                File.Copy(icon, portableIcon);
            }

            string configuredPortableIcon = Icons.GetIconsPathInside(TestFolderConfiguredWithPortableIcon)[0];
            Icons.ConfigureIconToFolder(TestFolderConfiguredWithPortableIcon, configuredPortableIcon);
            
            // Configura un icon de forma externa a la carpeta, sin ser portable
            icons = Icons.GetIconsPathInside(TestFolderConfiguredWithExternalIcon);
            if (icons.Length == 0)
            {
                string icon = Icons.GetIconsPathInside(TestIconsPath)[0];
                Icons.ConfigureIconToFolder(TestFolderConfiguredWithExternalIcon, icon);
            }
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test] // Ni tiene nada configurado ni un icon dentro, Carpeta Default
        public void UnconfiguredWithoutIcon()
        {
            Icons.FolderIconInfo info = Icons.AnalyzeFolder(TestFolderUnconfigured);
            Assert.That(info.HasConfiguredIcon, Is.False);
            Assert.That(info.HasIconFileInside, Is.False);
        }

        [Test] // No tiene un icon configurado aun y dentro hay uno cualquiera que queremos usar haciendo portable
        public void UnconfiguredWithIconInside()
        {
            Icons.FolderIconInfo info = Icons.AnalyzeFolder(TestFolderUnconfiguredWithIcon);
            Assert.That(info.HasIconFileInside, Is.True);
        }
        
        [Test] // No tiene el icon configurado aun y esta dentro como portable
        public void UnconfiguredWithPortableIcon()
        {
            Icons.FolderIconInfo info = Icons.AnalyzeFolder(TestFolderUnconfiguredWithPortableIcon);
            Assert.That(info.HasPortableIconInside, Is.True);
        }

        [Test] // Tiene un icon configurado y su ruta es externa a la carpeta
        public void ConfiguredExternalIcon()
        {
            Icons.FolderIconInfo result = Icons.AnalyzeFolder(TestFolderConfiguredWithExternalIcon);
            Assert.That(result.HasConfiguredIcon, Is.True);
            Assert.That(result.ConfiguredIconPath, Is.Not.Null);
            Assert.That(result.ConfiguredIconIsInside, Is.False);
        }

        [Test] // Tiene un Icon portable configurado que esta dentro con el prefijo que lo demarca como portable
        public void ConfiguredPortableIcon()
        {
            Icons.FolderIconInfo result = Icons.AnalyzeFolder(TestFolderConfiguredWithPortableIcon);
            Assert.That(result.HasPortableIconInside, Is.True);
            Assert.That(result.HasConfiguredIcon, Is.True);
            Assert.That(result.ConfiguredIconIsInside, Is.True);
        }
    }
}
