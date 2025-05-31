<p align="center"></p>
<p align="center"><a href="#"><img width="115px" src="docs/assets/Logo-115px.png" align="center" alt="Crystal Folders"/></a></p>
<h1 align="center">Crystal Folders</h1>
<p align="center">Crystal Folders es una aplicación para personalizar los iconos de tus carpetas en segundos, en Windows.</p>

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

## Características
* Personaliza muchas carpetas en segundos.
* Agrega carpetas dentro de Documentos, Imágenes, etc. con un clic.
* Posibilidad de arrastrar y soltar.
* Elegir entre diferentes iconos predeterminados o uno propio.
* Opción para incluir subcarpetas.
* Posibilidad de quitar directorios de la lista una vez agregados.
* Número de carpetas y subcarpetas a personalizar.
* Opción para crear carpetas portables con icono personalizado.
* Evita la duplicación de carpetas que se han añadido previamente. 
* Notificaciones directamente en el escritorio.
* Evita la personalización de carpetas con iconos del sistema.
* Cambia entre el idioma Inglés y Español.
* Guarda la configuración en un archivo .ini, excepto para los switches.
* Personaliza el color del tema.
* Modificación del archivo Desktop.ini sin perder información anterior.
* Esquinas adaptables (redondeadas o metro), dependiendo la versión del sistema operativo.

## Previa
<a href="#"><img src="docs/assets/Crystal-Folders-App.gif"/></a>

## Uso
En el panel izquierdo hay un área donde se mostrará la lista de directorios o carpetas a personalizar. Más abajo podemos encontrar iconos con sus respectivas casillas, los cuales son: Documentos, Imágenes, Música, Videos y Escritorio.

<br/>

𝟭. **Agregar carpetas**, existen dos formas:
* Activa la o las casillas que desees personalizar, por ejemplo: si seleccionaste _Documentos_, se agregarán a la lista todas las carpetas que se encuentran dentro. Puedes desactivarlas en cualquier momento para quitar esos directorios.
* Arrastrando y soltando carpetas en el área mencionada anteriormente, incluso si ya has activado casillas.

Para quitar carpetas una por una, debes seleccionarla en la lista y cliquear **⦵**.

𝟮. **Elegir un icono:**
   En el panel derecho oprime el botón `Elegir…`; se abrirá una ventana con iconos por defecto en diferentes colores o podrías usar uno propio, solo debe estar en formato ICO. Una vez que lo selecciones y oprimas en `Abrir` aparecerá en la vista previa.

𝟯. **Agregar subcarpetas:**
   Más abajo puedes activar el interruptor de `Agregar subcarpetas`, el cual también indicará la cantidad de subcarpetas totales. Si consideras que son demasiados o no te interesa, podrías simplemente desactivarlo, esto únicamente ayuda a tener una mejor experiencia de personalización.

<br/>

>Vamos a omitir por ahora el segundo switch.

<br/>

𝟰. **Personalizar todo:**
   Por último, oprime el botón `Personalizar` que, de igual manera, indica la cantidad total de carpetas a las que se les cambiará el icono. Hecho eso, se mostrará una pequeña ventana de espera y posteriormente, una notificación en tu escritorio confirmando que se ha completado correctamente.
   
Si la cantidad de carpetas supera los 600, se mostrará un mensaje preguntando si deseas continuar, sin embargo, el botón recomendado por defecto es `No` en caso de que prefieras disminuirla.
☑️ ¡Listo, así de fácil!, ya solo queda mirar tus carpetas personalizadas; en caso de no ver cambios, solo actualiza la ventana y/o escritorio.

Recuerda que no es necesario cerrar la aplicación para limpiar la lista o las opciones que hayas elegido, basta con hacer clic en **⟳**.

<br/>

### Restaurar carpetas:

𝟭. Deberás seguir los primeros pasos nuevamente; activando las casillas o arrastrando y soltandolos a la lista. La diferencia es: en vez de elegir un nuevo icono, deberás hacer clic en el botón `Restaurar` y como podrás notar aparecerá una flechita en lo que sería la vista previa, para indicarnos dicha acción.

𝟮. Podrás notar en el switch de abajo que ha cambiado a `Restaurar subfolders`, actívalo en caso de que los hayas personalizado antes.

𝟯. Finalmente da clic en el botón `Restaurar`. Volverá a aparecer la pequeña ventana de espera, posteriormente la notificación en el escritorio de que se ha completado y listo, tus carpetas vuelven a ser normales.

<br/>

<a href="#"><img src="docs/assets/Cystal-Folders-Usage.gif"/></a>

<br id="details"/>

### Carpetas con iconos personalizados en otra PC:

El segundo switch `Configurar a portable` permite personalizar el icono a máximo 30 carpetas (incluyendo subcarpetas) para que se muestren con ese icono en cualquier otra computadora, debido a que lo copia dentro. Esta opción solo funciona arrastrándolas al área de la lista; si hay alguna casilla activada del panel izquierdo, el switch no podrá activarse.

De la misma manera, al restaurar una carpeta con icono portable deberás seguir los mismos pasos, solo recuerda activar el switch `Restaurar de portable`.

<br/>

### Botón de Info:

Dando click en el botón superior de Info, aparecerá una nueva ventana en la que puedes:

- **Cambiar el tema:** Haz clic sobre `Cambiar Tema` para abrir el selector de color y elegir uno personalizado, cuando lo tengas da clic en Aplicar. Para regresar al color por defecto repite los pasos anteriores y presiona el botón morado debajo del selector de tono, finalmente haz clic en Aplicar.

- **Cambiar el idioma:** En la sección inferior **Idioma**, haz clic en el icono de las flechitas para cambiar entre English o Español, por último da clic en el botón de regresar para aplicar y ver los cambios.

- **Contenido de terceros:** En la parte izquierda están los nombres de cada librería, proyecto, iconos o tipografías que fueron utilizados, cliquea uno para leer su licencia y autor(a) o autores(as), además de una corta descripción. Puedes hacer clic sobre el icono de clip para ir al repositorio/sitio oficial de cada uno y cambiar de página con **Siguiente 🢖🢖** o **🢔🢔 Atrás**.

<br/>

<a href="#"><img src="docs/assets/Cystal-Folders-Options.gif"/></a>

<br/>

### Medidas de seguridad:

* No puedes personalizar carpetas especiales que ya tengan un icono del sistema, como por ejemplo _Documentos_ o _Imágenes_, solo los directorios dentro del mismo.
* Hay carpetas a las que no se les puede cambiar el icono debido a su nivel de protección o privilegios, por lo tanto se mostrará una notificación en el escritorio avisando sobre cuántas se han omitido.
* Modifica directamente el archivo Desktop.ini, evitando así eliminarlo y creando uno nuevo, sin perder información del mismo.
* Mensaje límite de carpetas, una vez superada la cantidad de 600; para desactivar este mensaje, abre el archivo Config.ini y cambia `Message = true` por `Message = false`.

<br>

## Instalador

Para compilar el instalador es necesario [Inno Setup](https://jrsoftware.org/isinfo.php), los archivos se encuentran en la carpeta [installer src](/installer%20src). Solo debes abrir el proyecto (Installer.iss) y compilarlo, a menos que desees hacer una modificación. Al finalizar, en la misma ubicación dejará una carpeta llamada Output.

>* Por favor, toma en cuenta que primero debes compilar el código de fuente en Visual Studio para tener los binarios, de lo contrario obtendrás un [📍Compiler Error](https://github.com/genesistoxical/drop-icons/issues/3).
>* Si lo prefieres, puedes descargar la app compilada en la [página](https://genesistoxical.github.io/crystal-folders/) de Crystal Folders.

**Portable with 7zip.bat** permite comprimir en .zip rápidamente la versión portable. Únicamente funciona con [7zip](https://www.7-zip.org/).

Ambos .iss y .bat obtienen los archivos que son necesarios dentro de su carpeta y/o la carpeta Release (debido a las rutas relativas).

<br>

## Contribuciones
* El paquete **HandyControls** dejará de actualizarse (por ahora o permanentemente) ya que la última versión no permite cambiar el tamaño de ToggleButtonSwitch.
* Si haces comentarios en el código, preferiblemente en Español, por favor.
* Los nombres de las variables deben estar en Inglés.
* Si abres un **Issue**, puede ser en Inglés o Español.
* **Pull request** en Inglés, en la descripción puedes agregar detalles en Inglés o Español.
  
## Config
El archivo `Config.ini` almacena información del lenguaje, color del tema y el mensaje límite de carpetas.

~~~
[Options]
Language = en
Message = true

[Theme]
#FF69B4
~~~

>Nota: las dos opciones de tipo switch no se guardan porque no son opciones que suelan activarse todo el tiempo.

## Créditos
* [HandyControls](https://github.com/ghost1372/HandyControls) bajo [MIT License](https://github.com/ghost1372/HandyControls/blob/develop/LICENSE).

* [FolderBrowserEx](https://github.com/evaristocuesta/FolderBrowserEx) library bajo [MIT License](https://github.com/evaristocuesta/FolderBrowserEx/blob/master/LICENSE).

* [Noto Music](https://fonts.google.com/noto/specimen/Noto+Music) bajo [SIL Open Font License](/src/DropIcons/Docs/Noto%20Music/OFL.txt).

* Algunos iconos son parte de [Teenyicons](https://github.com/teenyicons/teenyicons) bajo [MIT License](https://github.com/teenyicons/teenyicons/blob/master/LICENSE).

* Iconos de checkboxes son parte de [Jam icons](https://github.com/michaelampr/jam) bajo [MIT License](https://github.com/michaelampr/jam/blob/master/LICENSE).

* [WinVersion](https://github.com/shaovoon/win_version_detection) detection bajo [MIT License](https://github.com/shaovoon/win_version_detection/blob/main/LICENSE).

* [Pixie Folders](https://genesistoxical.github.io/pixie-folders/) bajo [MIT License](https://github.com/genesistoxical/pixie-folders/blob/main/LICENSE).

*Puedes encontrar todas las licencias [aquí](/src/CystalFolders/Docs).*

## ¿Qué es lo nuevo?
`Versión 1.2.0` evita que la fecha de modificación de la carpeta cambie al momento de personalizarlas. Y se agregaron un par de mejoras visuales a la ventana **Acerca de**.

## Licencia
**MIT License**

Copyright (c) 2025 Génesis Toxical ([read here](LICENSE)).

<br>

## Relacionado:
`🩷 Drop Icons` Aplicación para convertir imágenes a iconos (.ico) para Windows: [`Descargar`](https://genesistoxical.github.io/drop-icons/) or [`Repositorio`](https://github.com/genesistoxical/drop-icons/).

`🩷 Image to Icon` Convertir imagen a icono online: [`imagetoicon.glitch.me`](https://imagetoicon.glitch.me/) o [`Repositorio`](https://github.com/genesistoxical/imagetoicon).

`🩷 Pixie Folders` Set con seis diseños de iconos de carpetas minimalistas y editables: [`Descargar`](https://genesistoxical.github.io/pixie-folders/) o [`Repositorio`](https://github.com/genesistoxical/pixie-folders).
