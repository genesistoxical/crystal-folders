cd "..\src\CrystalFolders\bin\"
ren "Release" "Crystal Folders"
"C:\Program Files\7-Zip\7z.exe" a "..\..\..\installer src\Output\Crystal-Folders.zip" "Crystal Folders\" -r
ren "Crystal Folders" "Release"