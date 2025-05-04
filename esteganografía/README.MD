# 🕵️‍♂️ Esteganografía en Imágenes PNG (C#)

Este proyecto permite ocultar y extraer mensajes de texto en imágenes PNG utilizando **esteganografía de bit menos significativo (LSB)**, todo implementado en **C#**.

---

## 🧠 ¿Cómo funciona?

La técnica utilizada consiste en modificar el **último bit** de los canales de color **(Rojo, Verde y Azul)** de cada píxel para insertar bits del mensaje. Estos cambios son imperceptibles al ojo humano.

- Se almacenan 3 bits por píxel (uno por canal RGB).
- Los primeros 32 bits (4 bytes) codifican la **longitud del mensaje**.
- Luego se codifica el mensaje en sí.

---

## 📁 Estructura del proyecto

- `WriteMessageInPng`: Inserta el mensaje en la imagen.
- `ExtractMessageFromPng`: Recupera el mensaje de una imagen.
- `EmbedByte`: Inserta un byte en los LSB de varios píxeles.
- `ExtractByte`: Extrae un byte de los LSB de varios píxeles.

---

## 📦 Requisitos

- .NET Core o .NET Framework
- C#
- `System.Drawing.Common` (puede requerir instalación en .NET Core)

### Instalación en .NET Core:

```bash
dotnet add package System.Drawing.Common
```

---

## ▶️ Uso
1. Prepara tus imágenes.

Usa una imagen PNG de buena resolución (ej. FONDOPANTALLA.png)

Copia la imagen a la carpeta de Descargas de tu usuario.

2. Modifica la línea para insertar un mensaje:
```c#
WriteMessageInPng(myImage, myImage_with_Msg, "Este es un mensaje oculto");
```
3. Ejecuta el proyecto
Compila y corre tu aplicación:

```bash
dotnet run
```

4. Resultado
Se genera una imagen FONDOPANTALLA_CON_MSG.png con el mensaje oculto.

El programa también puede leer y mostrar el mensaje escondido:

```bash
El mensaje en la imagen es: Este es un mensaje oculto
```
---

## 📌 Notas importantes
Asegúrate de usar imágenes PNG sin compresión con pérdida (NO JPEG).

La capacidad máxima de almacenamiento depende del tamaño de la imagen:

Ej. Una imagen de 1000x1000 puede almacenar hasta ~375,000 bits (~46 KB).

---
## 📷 Créditos
Creado como ejemplo de implementación de técnicas de esteganografía en C# para fines educativos.

