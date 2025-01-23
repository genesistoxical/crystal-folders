using System.Runtime.InteropServices;
using System.Text;

namespace CrystalFolders
{
    internal class ExtAppInfo
    {
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint AssocQueryString(string flags, uint str, string pszAssoc, string pszExtra, [Out] StringBuilder pszOut, [In][Out] ref uint pcchOut);

        public const int App = 20; //ProgID
        public const int Icn = 15; //DefaultIcon
        public const int Prog = 4;
        public const int Nam = 3;

        public static string ExtProgID_Icon(uint str, string extn)
        {
            uint pchOut = 0;
            _ = AssocQueryString(null, str, extn, null, null, ref pchOut);

            StringBuilder pszOut = new StringBuilder((int)pchOut);
            _ = AssocQueryString(null, str, extn, null, pszOut, ref pchOut);
            return pszOut.ToString();
        }
    }
}
