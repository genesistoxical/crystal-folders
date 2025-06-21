cd "..\src\CrystalFolders\bin\"
ren "Release" "Crystal Folders"
chcp 65001>nul
copy "..\..\..\installer src\Custom 📂 Folder.url" "Crystal Folders\"
"C:\Program Files\7-Zip\7z.exe" a "..\..\..\installer src\Output\Crystal-Folders.zip" "Crystal Folders\" -r
ren "Crystal Folders" "Release"
del "Release\Custom 📂 Folder.url"