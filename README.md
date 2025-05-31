<p align="center"></p>
<p align="center"><a href="#"><img width="115px" src="docs/assets/Logo-115px.png" align="center" alt="Crystal Folders"/></a></p>
<h1 align="center">Crystal Folders</h1>
<p align="center">Crystal Folders is an app to customize your folder icons in a few seconds, for Windows.</p>

<p align="center">
 <a href="LICENSE"><img alt="License" src="https://img.shields.io/badge/License-MIT-FF69B4?style=flat-square&labelColor=343B45"/></a>
 <a href="https://github.com/genesistoxical/crystal-folders/releases/latest"><img src="https://img.shields.io/github/v/release/genesistoxical/crystal-folders.svg?color=FF69B4&label=Release&style=flat-square&labelColor=343B45"/></a>
 <a href="#"><img alt="NET" src="https://img.shields.io/badge/.NET_Framework-4.8-FF69B4?style=flat-square&labelColor=343B45"/></a> 
 <a href="/installer%20src"><img alt="Installer" src="https://img.shields.io/badge/Installer-ISS-FF69B4?style=flat-square&labelColor=343B45"/></a>
 <a href="#"><img alt="Languages" src="https://img.shields.io/badge/Languages-2-FF69B4?style=flat-square&labelColor=343B45"/></a>
</p>

<p align="center">
<a href="README.md">English</a> :speech_balloon: <a href="README-es.md">Español</a>
</p>

## Features
* Customize many folders in seconds.
* Add folders within Documents, Pictures, etc. with one click.
* Drag and drop option.
* Choose between different default icons or your own.
* Option to include subfolders.
* Possibility to remove directories from the list once added.
* Number of folders and subfolders to customize.
* Option to create portable folders with a custom icon. [𝐢](#details)
* Avoid duplication of folders that have been previously added.
* Notifications directly on the desktop.
* Avoid customizing folders with system icons.
* Switch between English and Spanish language.
* Save settings in a .ini file, except for switches.
* Customize theme color.
* Modifying the Desktop.ini file without losing previous information.
* Adaptive corners (rounded or metro), depending on the OS version.

## Preview
<a href="#"><img src="docs/assets/Crystal-Folders-App.gif"/></a>

## Usage
On the left panel there is an area where the directories or folder list to customize will be displayed. Below we can find icons with their respective checkboxes, which are: Documents, Pictures, Music, Videos and Desktop.

<br/>

𝟭. **Add folders**, there are two ways:
* Check the box(es) you want to customize, for example: if you selected _Documents_, all the folders inside will be added to the list. You can uncheck them at any time to remove those directories.
* By dragging and dropping folders into the area mentioned above, even if you have already checked boxes.

To remove folders one by one, you must select it in the list and click on **⦵**.

𝟮. **Choose an icon:**
   On the right panel press the `Choose…` button, a window will open with default icons in different colors or you could use your own, it just needs to be in ICO format. Once you select it and press `Open` it will appear in the preview.

𝟯. **Add subfolders:**
   Further down you can turn on the `Add subfolders` switch, which will also indicate the total number of. If you find there are too many or you are not interested, you could simply turn it off, this only helps to have a better customization experience.

<br/>

>We will skip the second switch for now.

<br/>

𝟰. **Customize all:**
   Finally, press the `Customize` button, which, likewise, indicates the total number of folders whose icons will be changed. Once this is done, a small waiting window will be displayed and then a notification on your desktop confirming that it has completed successfully.
   
If the number of folders exceeds 600, a message will appear asking if you want to continue, however, the default recommended button is `No` in case you prefer to decrease it.

☑️ That’s it, it’s that easy! Now all that’s left to do is look at your customized folders; if you don’t see any changes, just refresh the window and/or desktop.

Remember, is not necessary to close the application to clear the list or options you have chosen, just click on **⟳**.

<br/>

### Restore folders:

𝟭. You will have to follow the first steps again; checking the boxes or dragging and dropping them to the list. The difference is: instead of choosing a new icon, you will have to click on the `Restore` button and as you can see, a little arrow will appear in what would be the preview, to indicate this action.

𝟮. You will notice the switch below has changed to `Restore subfolders`, turn that on if you’ve customized them before.

𝟯. Finally, click on the second `Restore` button. The small waiting window will appear again, then the notification on the desktop that it has been completed and that’s it, your folders are back to normal.

<br/>

<a href="#"><img src="docs/assets/Cystal-Folders-Usage.gif"/></a>

<br id="details"/>

### Folders with customized icons on another PC:

The second switch `Configure to portable` allows you to customize the icon for up to 30 folders (including subfolders) to be displayed with that icon on any other computer, since it copies it inside. This option only works by dragging them to the list area; If any box is checked on the left panel, the switch cannot be turned on.

In the same way, when restoring a folder with a portable icon you will need to follow the same steps, just remember to turn on the `Restore from portable` switch.

<br/>

### Info button:

Click on the upper Info button, a new window will appear where you can:

- **Change theme:** Click on `Change Theme` to open the color picker and choose a custom one, then click Apply. To return to default values, repeat the previous steps and press the purple button below the tone selector. Finally click Apply.

- **Change language:** In the **Language** section below, click on the little arrows icon to change between English or Español, finally click on the return button to apply and see changes.
     
- **Third-party content:** On the left side are the names of each library, project, icons or fonts that were used, click one to read its license and author(s), as well as a short description. You can click on the clip icon to go to each repository/official site and change pages with **Next 🢖🢖** or **🢔🢔 Back**.

<br/>

<a href="#"><img src="docs/assets/Cystal-Folders-Options.gif"/></a>

<br/>

### Security measures:

* You cannot customize special folders that already have a system icon, such as _Documents_ or _Pictures_, only directories within it.
* There are folders whose icon cannot be changed due to their protection level or privileges, so a notification will be displayed on the desktop warning about how many have been skipped.
* It directly modifies the Desktop.ini file, thus avoiding deleting it and creating a new one, without losing its information.
* Folders limit message, once the amount of 600 has been exceeded; to disable this message, open the `Config.ini` file and change `Message = true` to `Message = false`.

<br/>

## Installer
To compile the installer you need [Inno Setup](https://jrsoftware.org/isinfo.php), the files are located in the [installer src](/installer%20src) folder. You just need to open the project (Installer.iss) and compile it, unless you want to make a change. When finished, it will leave a folder called Output in the same location.

>* Please note that you must first compile the source code in Visual Studio to get the binaries, otherwise, you will get a [📍Compiler Error](https://github.com/genesistoxical/drop-icons/issues/3).
>* Alternatively, you can download the compiled app from the Crystal Folders [page](https://genesistoxical.github.io/crystal-folders/).

**Portable with 7zip.bat** allows you to quickly zip the portable version. It only works with [7zip](https://www.7-zip.org/).

Both .iss and .bat get the files that are needed within their folder and/or the Release folder (due to relative paths).

<br>

## Contributing
* **HandyControls** package will no longer be updated (for now or permanently) as the latest version does not allow resizing of ToggleButtonSwitch.
* If you make comments in the code, preferably in Spanish, please.
* Variable names must be in English.
* If you open an **Issue**, it can be in English o Spanish.
* **Pull request** in English, in the description you can add details in English or Spanish.
  
## Config
`Config.ini` file stores information about the language, theme color and folders limit message.

~~~
[Options]
Language = en
Message = true

[Theme]
#FF69B4
~~~

>Note: The two switch-type options are not saved because they are not options that are usually activated all the time.

## Credits
* [HandyControls](https://github.com/ghost1372/HandyControls) under [MIT License](https://github.com/ghost1372/HandyControls/blob/develop/LICENSE).

* [FolderBrowserEx](https://github.com/evaristocuesta/FolderBrowserEx) library under [MIT License](https://github.com/evaristocuesta/FolderBrowserEx/blob/master/LICENSE).

* [Noto Music](https://fonts.google.com/noto/specimen/Noto+Music) under [SIL Open Font License](/src/DropIcons/Docs/Noto%20Music/OFL.txt).

* Some icons are part of [Teenyicons](https://github.com/teenyicons/teenyicons) under [MIT License](https://github.com/teenyicons/teenyicons/blob/master/LICENSE).

* Checkboxes icons are part of [Jam icons](https://github.com/michaelampr/jam) under [MIT License](https://github.com/michaelampr/jam/blob/master/LICENSE).

* [WinVersion](https://github.com/shaovoon/win_version_detection) detection under [MIT License](https://github.com/shaovoon/win_version_detection/blob/main/LICENSE).
 
* [Pixie Folders](https://genesistoxical.github.io/pixie-folders/) under [MIT License](https://github.com/genesistoxical/pixie-folders/blob/main/LICENSE).

*You can find all licenses [here](/src/CystalFolders/Docs).*

## What's new?
`Version 1.2.0` prevents the folder modification date from changing when customizing them. A couple of visual improvements have been added to **About** window.

## License
**MIT License**

Copyright (c) 2025 Génesis Toxical ([read here](LICENSE)).

<br>

## Related:
`🩷 Drop Icons` Utility to convert images to icons (.ico) for Windows: [`Download`](https://genesistoxical.github.io/drop-icons/) or [`Repository`](https://github.com/genesistoxical/drop-icons/).

`🩷 Image to Icon` Convert image to icon online: [`imagetoicon.glitch.me`](https://imagetoicon.glitch.me/) or [`Repository`](https://github.com/genesistoxical/imagetoicon).

`🩷 Pixie Folders` Set of six minimalist and editable folder icon designs: [`Download`](https://genesistoxical.github.io/pixie-folders/) or [`Repository`](https://github.com/genesistoxical/pixie-folders).
