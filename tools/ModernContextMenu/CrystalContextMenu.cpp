#define WIN32_LEAN_AND_MEAN
#define STRICT_TYPED_ITEMIDS
#include <windows.h>
#include <shlobj.h>
#include <shlwapi.h>
#include <wrl/client.h>
#include <string>
#include <vector>

#pragma comment(lib, "shell32.lib")
#pragma comment(lib, "shlwapi.lib")
#pragma comment(lib, "ole32.lib")
#pragma comment(lib, "user32.lib")
#pragma comment(lib, "advapi32.lib")

// CLSID: {8A55B2E1-6548-4FA1-8D91-955B6BC883F2}
static const GUID CLSID_CrystalContextMenu = 
{ 0x8a55b2e1, 0x6548, 0x4fa1, { 0x8d, 0x91, 0x95, 0x5b, 0x6b, 0xc8, 0x83, 0xf2 } };

static volatile LONG g_cRef = 0;
static HMODULE g_hInst = NULL;
static wchar_t g_szDllPath[MAX_PATH] = { 0 };

// Cadenas precalculadas en formato DLL,-ID para que Windows Shell nunca toque el disco
static wchar_t g_szIconPurple[MAX_PATH + 16] = { 0 };
static wchar_t g_szIconAqua[MAX_PATH + 16]   = { 0 };
static wchar_t g_szIconBlue[MAX_PATH + 16]   = { 0 };
static wchar_t g_szIconYellow[MAX_PATH + 16] = { 0 };
static wchar_t g_szIconPink[MAX_PATH + 16]   = { 0 };
static wchar_t g_szIconBlack[MAX_PATH + 16]  = { 0 };
static wchar_t g_szIconSalmon[MAX_PATH + 16] = { 0 };

// Rutas fisicas en disco para desktop.ini (al hacer CLICK, nunca al abrir menu)
static wchar_t g_szDiskPurple[MAX_PATH] = { 0 };
static wchar_t g_szDiskAqua[MAX_PATH]   = { 0 };
static wchar_t g_szDiskBlue[MAX_PATH]   = { 0 };
static wchar_t g_szDiskYellow[MAX_PATH] = { 0 };
static wchar_t g_szDiskPink[MAX_PATH]   = { 0 };
static wchar_t g_szDiskBlack[MAX_PATH]  = { 0 };
static wchar_t g_szDiskSalmon[MAX_PATH] = { 0 };

const wchar_t* GetDiskIconPathFast(const wchar_t* color) {
    if (wcscmp(color, L"purple") == 0) return g_szDiskPurple;
    if (wcscmp(color, L"aqua") == 0)   return g_szDiskAqua;
    if (wcscmp(color, L"blue") == 0)   return g_szDiskBlue;
    if (wcscmp(color, L"yellow") == 0) return g_szDiskYellow;
    if (wcscmp(color, L"pink") == 0)   return g_szDiskPink;
    if (wcscmp(color, L"black") == 0)  return g_szDiskBlack;
    if (wcscmp(color, L"salmon") == 0) return g_szDiskSalmon;
    return g_szDiskPurple;
}

inline bool IsSpanishUI() {
    return PRIMARYLANGID(GetUserDefaultUILanguage()) == LANG_SPANISH;
}

const wchar_t* AutoClassifyFast(const wchar_t* folderName) {
    if (!folderName) return L"yellow";
    
    wchar_t lower[256];
    wcsncpy_s(lower, folderName, _TRUNCATE);
    _wcslwr_s(lower);

    // Blue: Development, code, applications, tools, system, sdk
    if (wcsstr(lower, L"dev") || wcsstr(lower, L"code") || wcsstr(lower, L"prog") || 
        wcsstr(lower, L"app") || wcsstr(lower, L"tool") || wcsstr(lower, L"sdk") || 
        wcsstr(lower, L"git") || wcsstr(lower, L"src") || wcsstr(lower, L"build")) {
        return L"blue";
    }
    // Purple: Documents, thesis, work, study, university, research, books
    if (wcsstr(lower, L"doc") || wcsstr(lower, L"work") || wcsstr(lower, L"trabaj") || 
        wcsstr(lower, L"tesis") || wcsstr(lower, L"thes") || wcsstr(lower, L"paper") || 
        wcsstr(lower, L"estudi") || wcsstr(lower, L"study") || wcsstr(lower, L"book") || 
        wcsstr(lower, L"libro") || wcsstr(lower, L"school") || wcsstr(lower, L"uni") || 
        wcsstr(lower, L"posgrad")) {
        return L"purple";
    }
    // Aqua / Cyan: Media, images, video, music, art, design, creative
    if (wcsstr(lower, L"media") || wcsstr(lower, L"music") || wcsstr(lower, L"video") || 
        wcsstr(lower, L"art") || wcsstr(lower, L"design") || wcsstr(lower, L"foto") || 
        wcsstr(lower, L"photo") || wcsstr(lower, L"img") || wcsstr(lower, L"ia") || wcsstr(lower, L"ai")) {
        return L"aqua";
    }
    // Pink: Personal, family
    if (wcsstr(lower, L"person") || wcsstr(lower, L"famili") || wcsstr(lower, L"life")) {
        return L"pink";
    }
    return L"yellow";
}

void ApplyColorFast(LPCWSTR folderPath, const wchar_t* colorKey) {
    if (!folderPath || !PathIsDirectoryW(folderPath)) return;

    const wchar_t* finalColor = colorKey;
    if (wcscmp(colorKey, L"auto") == 0) {
        LPCWSTR pName = PathFindFileNameW(folderPath);
        finalColor = AutoClassifyFast(pName);
    }

    bool isRestore = (wcscmp(finalColor, L"restore") == 0);

    if (isRestore) {
        wchar_t szIni[MAX_PATH];
        PathCombineW(szIni, folderPath, L"desktop.ini");
        if (PathFileExistsW(szIni)) {
            SetFileAttributesW(szIni, FILE_ATTRIBUTE_NORMAL);
            DeleteFileW(szIni);
        }
        DWORD attr = GetFileAttributesW(folderPath);
        if (attr != INVALID_FILE_ATTRIBUTES && (attr & FILE_ATTRIBUTE_READONLY)) {
            SetFileAttributesW(folderPath, attr & ~FILE_ATTRIBUTE_READONLY);
        }
    } else {
        SHFOLDERCUSTOMSETTINGS fcs = { sizeof(fcs) };
        fcs.dwMask = FCSM_ICONFILE;
        const wchar_t* iconPath = GetDiskIconPathFast(finalColor);
        fcs.pszIconFile = (LPWSTR)iconPath;
        fcs.iIconIndex = 0;

        SHGetSetFolderCustomSettings(&fcs, folderPath, FCS_FORCEWRITE);

        wchar_t szIni[MAX_PATH];
        PathCombineW(szIni, folderPath, L"desktop.ini");
        if (PathFileExistsW(szIni)) {
            DWORD iniAttr = GetFileAttributesW(szIni);
            if (iniAttr != INVALID_FILE_ATTRIBUTES && !(iniAttr & FILE_ATTRIBUTE_SYSTEM)) {
                SetFileAttributesW(szIni, iniAttr | FILE_ATTRIBUTE_HIDDEN | FILE_ATTRIBUTE_SYSTEM);
            }
            SHChangeNotify(SHCNE_UPDATEITEM, SHCNF_PATHW | SHCNF_FLUSH, szIni, NULL);
        }

        DWORD attr = GetFileAttributesW(folderPath);
        if (attr != INVALID_FILE_ATTRIBUTES && !(attr & FILE_ATTRIBUTE_READONLY)) {
            SetFileAttributesW(folderPath, attr | FILE_ATTRIBUTE_READONLY);
        }
    }

    // Tocar LastWriteTime del directorio para despertar el FileSystemWatcher de Windows Explorer
    HANDLE hDir = CreateFileW(
        folderPath, 
        FILE_WRITE_ATTRIBUTES, 
        FILE_SHARE_READ | FILE_SHARE_WRITE | FILE_SHARE_DELETE, 
        NULL, 
        OPEN_EXISTING, 
        FILE_FLAG_BACKUP_SEMANTICS, 
        NULL
    );
    if (hDir != INVALID_HANDLE_VALUE) {
        FILETIME ft;
        GetSystemTimeAsFileTime(&ft);
        ULARGE_INTEGER uli;
        uli.LowPart = ft.dwLowDateTime;
        uli.HighPart = ft.dwHighDateTime;
        uli.QuadPart += 10000000ULL; // +1 segundo
        ft.dwLowDateTime = uli.LowPart;
        ft.dwHighDateTime = uli.HighPart;
        SetFileTime(hDir, NULL, NULL, &ft);
        CloseHandle(hDir);
    }

    // Notificar al Shell con SHCNF_FLUSH para forzar repintado inmediato
    SHChangeNotify(SHCNE_ATTRIBUTES, SHCNF_PATHW | SHCNF_FLUSH, folderPath, NULL);
    SHChangeNotify(SHCNE_UPDATEITEM, SHCNF_PATHW | SHCNF_FLUSH, folderPath, NULL);

    wchar_t szParent[MAX_PATH];
    wcscpy_s(szParent, folderPath);
    if (PathRemoveFileSpecW(szParent)) {
        SHChangeNotify(SHCNE_UPDATEDIR, SHCNF_PATHW | SHCNF_FLUSH, szParent, NULL);
    }

    // Broadcast inmediato para que Explorer descarte el icono en cache y pinte el nuevo sin F5
    SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST | SHCNF_FLUSHNOWAIT, NULL, NULL);
}

enum CmdIconId {
    CMD_ICON_PURPLE = 101,
    CMD_ICON_AQUA   = 102,
    CMD_ICON_BLUE   = 103,
    CMD_ICON_YELLOW = 104,
    CMD_ICON_PINK   = 105,
    CMD_ICON_BLACK  = 106,
    CMD_ICON_SALMON = 107,
    CMD_ICON_RESTORE = 0
};

struct SubCmdDef {
    const wchar_t* titleEs;
    const wchar_t* titleEn;
    const wchar_t* color;
    CmdIconId iconId;
};

// Etiquetas limpias y universales para cualquier usuario (ES/EN)
static const SubCmdDef S_COMMANDS[] = {
    { L"Morado",                     L"Purple",                L"purple",  CMD_ICON_PURPLE },
    { L"Celeste",                    L"Aqua",                  L"aqua",    CMD_ICON_AQUA },
    { L"Azul",                       L"Blue",                  L"blue",    CMD_ICON_BLUE },
    { L"Amarillo",                   L"Yellow",                L"yellow",  CMD_ICON_YELLOW },
    { L"Rosado",                     L"Pink",                  L"pink",    CMD_ICON_PINK },
    { L"Negro",                      L"Black",                 L"black",   CMD_ICON_BLACK },
    { L"Salm\u00F3n",                L"Salmon",                L"salmon",  CMD_ICON_SALMON },
    { L"Clasificar Autom\u00E1ticamente", L"Classify Automatically", L"auto", CMD_ICON_AQUA },
    { L"Restaurar Predeterminado",   L"Restore Default",       L"restore", CMD_ICON_RESTORE }
};
static const size_t S_COMMAND_COUNT = sizeof(S_COMMANDS) / sizeof(S_COMMANDS[0]);

class CSubExplorerCommandFast : public IExplorerCommand {
private:
    volatile LONG m_cRef;
    const SubCmdDef* m_def;

public:
    CSubExplorerCommandFast(const SubCmdDef* def) : m_cRef(1), m_def(def) {
        InterlockedIncrement(&g_cRef);
    }

    virtual ~CSubExplorerCommandFast() {
        InterlockedDecrement(&g_cRef);
    }

    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv) {
        static const QITAB qit[] = {
            QITABENT(CSubExplorerCommandFast, IExplorerCommand),
            { 0 },
        };
        return QISearch(this, qit, riid, ppv);
    }

    IFACEMETHODIMP_(ULONG) AddRef() {
        return InterlockedIncrement(&m_cRef);
    }

    IFACEMETHODIMP_(ULONG) Release() {
        LONG cRef = InterlockedDecrement(&m_cRef);
        if (cRef == 0) delete this;
        return cRef;
    }

    IFACEMETHODIMP GetTitle(IShellItemArray *psiItemArray, LPWSTR *ppszName) {
        return SHStrDupW(IsSpanishUI() ? m_def->titleEs : m_def->titleEn, ppszName);
    }

    IFACEMETHODIMP GetIcon(IShellItemArray *psiItemArray, LPWSTR *ppszIcon) {
        switch (m_def->iconId) {
            case CMD_ICON_PURPLE: return SHStrDupW(g_szIconPurple, ppszIcon);
            case CMD_ICON_AQUA:   return SHStrDupW(g_szIconAqua,   ppszIcon);
            case CMD_ICON_BLUE:   return SHStrDupW(g_szIconBlue,   ppszIcon);
            case CMD_ICON_YELLOW: return SHStrDupW(g_szIconYellow, ppszIcon);
            case CMD_ICON_PINK:   return SHStrDupW(g_szIconPink,   ppszIcon);
            case CMD_ICON_BLACK:  return SHStrDupW(g_szIconBlack,  ppszIcon);
            case CMD_ICON_SALMON: return SHStrDupW(g_szIconSalmon, ppszIcon);
            case CMD_ICON_RESTORE:
            default:              return SHStrDupW(L"imageres.dll,-3", ppszIcon);
        }
    }

    IFACEMETHODIMP GetToolTip(IShellItemArray *psiItemArray, LPWSTR *ppszInfoTip) {
        return SHStrDupW(IsSpanishUI() ? m_def->titleEs : m_def->titleEn, ppszInfoTip);
    }

    IFACEMETHODIMP GetCanonicalName(GUID *pguidCommandName) {
        *pguidCommandName = GUID_NULL;
        return S_OK;
    }

    IFACEMETHODIMP GetState(IShellItemArray *psiItemArray, BOOL fOkToBeSlow, EXPCMDSTATE *pCmdState) {
        *pCmdState = ECS_ENABLED;
        return S_OK;
    }

    IFACEMETHODIMP Invoke(IShellItemArray *psiItemArray, IBindCtx *pbc) {
        if (!psiItemArray) return S_OK;
        DWORD count = 0;
        psiItemArray->GetCount(&count);
        for (DWORD i = 0; i < count; i++) {
            IShellItem *psi = nullptr;
            if (SUCCEEDED(psiItemArray->GetItemAt(i, &psi))) {
                LPWSTR pszPath = nullptr;
                if (SUCCEEDED(psi->GetDisplayName(SIGDN_FILESYSPATH, &pszPath))) {
                    ApplyColorFast(pszPath, m_def->color);
                    CoTaskMemFree(pszPath);
                }
                psi->Release();
            }
        }
        return S_OK;
    }

    IFACEMETHODIMP GetFlags(EXPCMDFLAGS *pFlags) {
        *pFlags = ECF_DEFAULT;
        return S_OK;
    }

    IFACEMETHODIMP EnumSubCommands(IEnumExplorerCommand **ppEnum) {
        *ppEnum = nullptr;
        return S_OK;
    }
};

class CEnumExplorerCommandFast : public IEnumExplorerCommand {
private:
    volatile LONG m_cRef;
    size_t m_index;

public:
    CEnumExplorerCommandFast() : m_cRef(1), m_index(0) {
        InterlockedIncrement(&g_cRef);
    }

    virtual ~CEnumExplorerCommandFast() {
        InterlockedDecrement(&g_cRef);
    }

    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv) {
        static const QITAB qit[] = {
            QITABENT(CEnumExplorerCommandFast, IEnumExplorerCommand),
            { 0 },
        };
        return QISearch(this, qit, riid, ppv);
    }

    IFACEMETHODIMP_(ULONG) AddRef() {
        return InterlockedIncrement(&m_cRef);
    }

    IFACEMETHODIMP_(ULONG) Release() {
        LONG cRef = InterlockedDecrement(&m_cRef);
        if (cRef == 0) delete this;
        return cRef;
    }

    IFACEMETHODIMP Next(ULONG celt, IExplorerCommand **pUICommand, ULONG *pceltFetched) {
        if (!pUICommand) return E_POINTER;
        ULONG fetched = 0;
        while (fetched < celt && m_index < S_COMMAND_COUNT) {
            pUICommand[fetched] = new CSubExplorerCommandFast(&S_COMMANDS[m_index]);
            m_index++;
            fetched++;
        }
        if (pceltFetched) *pceltFetched = fetched;
        return (fetched == celt) ? S_OK : S_FALSE;
    }

    IFACEMETHODIMP Skip(ULONG celt) {
        m_index = min(m_index + celt, S_COMMAND_COUNT);
        return S_OK;
    }

    IFACEMETHODIMP Reset() {
        m_index = 0;
        return S_OK;
    }

    IFACEMETHODIMP Clone(IEnumExplorerCommand **ppEnum) {
        if (!ppEnum) return E_POINTER;
        *ppEnum = new CEnumExplorerCommandFast();
        return *ppEnum ? S_OK : E_OUTOFMEMORY;
    }
};

class CRootExplorerCommandFast : public IExplorerCommand {
private:
    volatile LONG m_cRef;

public:
    CRootExplorerCommandFast() : m_cRef(1) {
        InterlockedIncrement(&g_cRef);
    }

    virtual ~CRootExplorerCommandFast() {
        InterlockedDecrement(&g_cRef);
    }

    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv) {
        static const QITAB qit[] = {
            QITABENT(CRootExplorerCommandFast, IExplorerCommand),
            { 0 },
        };
        return QISearch(this, qit, riid, ppv);
    }

    IFACEMETHODIMP_(ULONG) AddRef() {
        return InterlockedIncrement(&m_cRef);
    }

    IFACEMETHODIMP_(ULONG) Release() {
        LONG cRef = InterlockedDecrement(&m_cRef);
        if (cRef == 0) delete this;
        return cRef;
    }

    IFACEMETHODIMP GetTitle(IShellItemArray *psiItemArray, LPWSTR *ppszName) {
        return SHStrDupW(IsSpanishUI() ? L"Color de Carpeta" : L"Folder Color", ppszName);
    }

    IFACEMETHODIMP GetIcon(IShellItemArray *psiItemArray, LPWSTR *ppszIcon) {
        return SHStrDupW(g_szIconPurple, ppszIcon);
    }

    IFACEMETHODIMP GetToolTip(IShellItemArray *psiItemArray, LPWSTR *ppszInfoTip) {
        return SHStrDupW(IsSpanishUI() ? L"Personalizar color de carpeta" : L"Customize folder color", ppszInfoTip);
    }

    IFACEMETHODIMP GetCanonicalName(GUID *pguidCommandName) {
        *pguidCommandName = CLSID_CrystalContextMenu;
        return S_OK;
    }

    IFACEMETHODIMP GetState(IShellItemArray *psiItemArray, BOOL fOkToBeSlow, EXPCMDSTATE *pCmdState) {
        if (!pCmdState) return E_POINTER;
        *pCmdState = ECS_HIDDEN;
        if (!psiItemArray) return S_OK;

        DWORD count = 0;
        if (FAILED(psiItemArray->GetCount(&count)) || count == 0) return S_OK;

        IShellItem *psi = nullptr;
        if (SUCCEEDED(psiItemArray->GetItemAt(0, &psi))) {
            SFGAOF attrs = 0;
            if (SUCCEEDED(psi->GetAttributes(SFGAO_FOLDER, &attrs)) && (attrs & SFGAO_FOLDER)) {
                *pCmdState = ECS_ENABLED;
            }
            psi->Release();
        }
        return S_OK;
    }

    IFACEMETHODIMP Invoke(IShellItemArray *psiItemArray, IBindCtx *pbc) {
        return S_OK;
    }

    IFACEMETHODIMP GetFlags(EXPCMDFLAGS *pFlags) {
        *pFlags = ECF_HASSUBCOMMANDS;
        return S_OK;
    }

    IFACEMETHODIMP EnumSubCommands(IEnumExplorerCommand **ppEnum) {
        if (!ppEnum) return E_POINTER;
        *ppEnum = new CEnumExplorerCommandFast();
        return *ppEnum ? S_OK : E_OUTOFMEMORY;
    }
};

class CClassFactoryFast : public IClassFactory {
private:
    volatile LONG m_cRef;

public:
    CClassFactoryFast() : m_cRef(1) {
        InterlockedIncrement(&g_cRef);
    }

    virtual ~CClassFactoryFast() {
        InterlockedDecrement(&g_cRef);
    }

    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv) {
        static const QITAB qit[] = {
            QITABENT(CClassFactoryFast, IClassFactory),
            { 0 },
        };
        return QISearch(this, qit, riid, ppv);
    }

    IFACEMETHODIMP_(ULONG) AddRef() {
        return InterlockedIncrement(&m_cRef);
    }

    IFACEMETHODIMP_(ULONG) Release() {
        LONG cRef = InterlockedDecrement(&m_cRef);
        if (cRef == 0) delete this;
        return cRef;
    }

    IFACEMETHODIMP CreateInstance(IUnknown *pUnkOuter, REFIID riid, void **ppv) {
        if (!ppv) return E_POINTER;
        *ppv = nullptr;
        if (pUnkOuter) return CLASS_E_NOAGGREGATION;
        CRootExplorerCommandFast *pCmd = new CRootExplorerCommandFast();
        if (!pCmd) return E_OUTOFMEMORY;
        HRESULT hr = pCmd->QueryInterface(riid, ppv);
        pCmd->Release();
        return hr;
    }

    IFACEMETHODIMP LockServer(BOOL fLock) {
        if (fLock) InterlockedIncrement(&g_cRef);
        else InterlockedDecrement(&g_cRef);
        return S_OK;
    }
};

STDAPI DllGetClassObject(REFCLSID rclsid, REFIID riid, void **ppv) {
    if (!ppv) return E_POINTER;
    *ppv = nullptr;

    if (IsEqualCLSID(rclsid, CLSID_CrystalContextMenu)) {
        CClassFactoryFast *pFactory = new CClassFactoryFast();
        if (!pFactory) return E_OUTOFMEMORY;
        HRESULT hr = pFactory->QueryInterface(riid, ppv);
        pFactory->Release();
        return hr;
    }

    return CLASS_E_CLASSNOTAVAILABLE;
}

STDAPI DllCanUnloadNow(void) {
    return S_FALSE;
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
    if (ul_reason_for_call == DLL_PROCESS_ATTACH) {
        g_hInst = hModule;
        DisableThreadLibraryCalls(hModule);

        GetModuleFileNameW(hModule, g_szDllPath, MAX_PATH);
        swprintf_s(g_szIconPurple, L"%s,-101", g_szDllPath);
        swprintf_s(g_szIconAqua,   L"%s,-102", g_szDllPath);
        swprintf_s(g_szIconBlue,   L"%s,-103", g_szDllPath);
        swprintf_s(g_szIconYellow, L"%s,-104", g_szDllPath);
        swprintf_s(g_szIconPink,   L"%s,-105", g_szDllPath);
        swprintf_s(g_szIconBlack,  L"%s,-106", g_szDllPath);
        swprintf_s(g_szIconSalmon, L"%s,-107", g_szDllPath);

        wchar_t localAppData[MAX_PATH] = { 0 };
        if (GetEnvironmentVariableW(L"LOCALAPPDATA", localAppData, MAX_PATH)) {
            swprintf_s(g_szDiskPurple, L"%s\\CrystalFolders\\Folders\\Purple - Pixie Folders N1.ico", localAppData);
            swprintf_s(g_szDiskAqua,   L"%s\\CrystalFolders\\Folders\\Aqua - Pixie Folders N1.ico", localAppData);
            swprintf_s(g_szDiskBlue,   L"%s\\CrystalFolders\\Folders\\Blue - Pixie Folders N1.ico", localAppData);
            swprintf_s(g_szDiskYellow, L"%s\\CrystalFolders\\Folders\\Yellow - Pixie Folders N1.ico", localAppData);
            swprintf_s(g_szDiskPink,   L"%s\\CrystalFolders\\Folders\\Pink - Pixie Folders N1.ico", localAppData);
            swprintf_s(g_szDiskBlack,  L"%s\\CrystalFolders\\Folders\\Black - Pixie Folders N1.ico", localAppData);
            swprintf_s(g_szDiskSalmon, L"%s\\CrystalFolders\\Folders\\Salmon - Pixie Folders N1.ico", localAppData);
        } else {
            wchar_t szModuleDir[MAX_PATH];
            wcscpy_s(szModuleDir, g_szDllPath);
            PathRemoveFileSpecW(szModuleDir);
            swprintf_s(g_szDiskPurple, L"%s\\Folders\\Purple - Pixie Folders N1.ico", szModuleDir);
            swprintf_s(g_szDiskAqua,   L"%s\\Folders\\Aqua - Pixie Folders N1.ico", szModuleDir);
            swprintf_s(g_szDiskBlue,   L"%s\\Folders\\Blue - Pixie Folders N1.ico", szModuleDir);
            swprintf_s(g_szDiskYellow, L"%s\\Folders\\Yellow - Pixie Folders N1.ico", szModuleDir);
            swprintf_s(g_szDiskPink,   L"%s\\Folders\\Pink - Pixie Folders N1.ico", szModuleDir);
            swprintf_s(g_szDiskBlack,  L"%s\\Folders\\Black - Pixie Folders N1.ico", szModuleDir);
            swprintf_s(g_szDiskSalmon, L"%s\\Folders\\Salmon - Pixie Folders N1.ico", szModuleDir);
        }
    }
    return TRUE;
}
