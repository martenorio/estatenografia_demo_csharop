using System.Drawing;
using System.Text;

// Esteganografía en imagen PNG
// Código basado del video de hdeleon.net
// https://youtu.be/AVMqnvkVocM?si=B2OqqiPyfkmNwfv2

// Ruta de la carpeta Descargas del usuario
string rutaDescargas = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

// Ruta de la imagen original y la imagen modificada
string myImage = Path.Combine(rutaDescargas, "FONDOPANTALLA.png");
string myImage_with_Msg = Path.Combine(rutaDescargas, "FONDOPANTALLA_CON_MSG.png");

// Escribe el mensaje en la imagen (puedes descomentar para probarlo)
// WriteMessageInPng(myImage, myImage_with_Msg, "I don’t know everything. I just know what I know");

// Extrae el mensaje oculto desde la imagen modificada
var message = ExtractMessageFromPng(myImage_with_Msg);
Console.WriteLine("El mensaje en la imagen es: " + message);

/// <summary>
/// Inserta un mensaje oculto en una imagen PNG utilizando esteganografía LSB.
/// </summary>
/// <param name="inputImgPath">Ruta de la imagen original.</param>
/// <param name="outputImgPath">Ruta donde se guardará la imagen modificada.</param>
/// <param name="msg">Mensaje a ocultar.</param>
void WriteMessageInPng(string inputImgPath, string outputImgPath, string msg)
{
    Bitmap bmp = new Bitmap(inputImgPath);

    // Convierte el mensaje a bytes y guarda también su longitud
    byte[] messageBytes = Encoding.UTF8.GetBytes(msg);
    byte[] lengthBytes = BitConverter.GetBytes(messageBytes.Length);

    // Asegura que los bytes estén en formato little endian
    if (!BitConverter.IsLittleEndian)
        Array.Reverse(lengthBytes);

    // Calcula los bits necesarios
    int totalBitsNeeded = (lengthBytes.Length + messageBytes.Length) * 8;

    // Verifica que la imagen tenga suficiente capacidad
    if (totalBitsNeeded > bmp.Width * bmp.Height * 3)
        throw new ArgumentException("El mensaje es demasiado grande para esta imagen.");

    int bitIndex = 0;

    // Inserta la longitud del mensaje primero
    foreach (byte b in lengthBytes)
        bitIndex = EmbedByte(b, bmp, bitIndex);

    // Inserta los bytes del mensaje
    foreach (byte b in messageBytes)
        bitIndex = EmbedByte(b, bmp, bitIndex);

    // Guarda la imagen modificada
    bmp.Save(outputImgPath, System.Drawing.Imaging.ImageFormat.Png);
    Console.WriteLine(@"Imagen guardada: " + outputImgPath);
}

/// <summary>
/// Extrae un mensaje oculto de una imagen PNG.
/// </summary>
/// <param name="inputImagePath">Ruta de la imagen con el mensaje oculto.</param>
/// <returns>El mensaje extraído como string.</returns>
string ExtractMessageFromPng(string inputImagePath)
{
    Bitmap bmp = new Bitmap(inputImagePath);
    int bitIndex = 0;

    // Extrae los primeros 4 bytes que indican la longitud del mensaje
    byte[] lengthBytes = new byte[4];
    for (int i = 0; i < 4; i++)
        lengthBytes[i] = ExtractByte(bmp, ref bitIndex);

    // Asegura el formato endian
    if (!BitConverter.IsLittleEndian)
        Array.Reverse(lengthBytes);

    int messageLength = BitConverter.ToInt32(lengthBytes, 0);

    // Extrae los bytes del mensaje
    byte[] messageBytes = new byte[messageLength];
    for (int i = 0; i < messageLength; i++)
        messageBytes[i] = ExtractByte(bmp, ref bitIndex);

    return Encoding.UTF8.GetString(messageBytes);
}

/// <summary>
/// Inserta 8 bits (1 byte) en los canales RGB de los píxeles de la imagen.
/// </summary>
/// <param name="b">Byte a insertar.</param>
/// <param name="bmp">Imagen donde se insertará.</param>
/// <param name="bitIndex">Índice actual de bit en la imagen.</param>
/// <returns>Índice actualizado después de insertar.</returns>
int EmbedByte(byte b, Bitmap bmp, int bitIndex)
{
    for (int i = 0; i < 8; i++)
    {
        int x = (bitIndex / 3) % bmp.Width;
        int y = (bitIndex / 3) / bmp.Width;

        Color pixel = bmp.GetPixel(x, y);
        int bit = (b >> (7 - i)) & 1; // Extrae el bit actual a insertar (de MSB a LSB)

        // Reemplaza el LSB del canal correspondiente
        switch (bitIndex % 3)
        {
            case 0:
                pixel = Color.FromArgb(pixel.A, (pixel.R & 0xFE) | bit, pixel.G, pixel.B);
                break;
            case 1:
                pixel = Color.FromArgb(pixel.A, pixel.R, (pixel.G & 0xFE) | bit, pixel.B);
                break;
            case 2:
                pixel = Color.FromArgb(pixel.A, pixel.R, pixel.G, (pixel.B & 0xFE) | bit);
                break;
        }

        bmp.SetPixel(x, y, pixel);
        bitIndex++;
    }

    return bitIndex;
}

/// <summary>
/// Extrae 8 bits (1 byte) desde los canales RGB de los píxeles de la imagen.
/// </summary>
/// <param name="bmp">Imagen de la que se extraerá el byte.</param>
/// <param name="bitIndex">Índice actual de bit en la imagen.</param>
/// <returns>Byte extraído.</returns>
byte ExtractByte(Bitmap bmp, ref int bitIndex)
{
    byte b = 0;

    for (int i = 0; i < 8; i++)
    {
        int x = (bitIndex / 3) % bmp.Width;
        int y = (bitIndex / 3) / bmp.Width;

        Color pixel = bmp.GetPixel(x, y);
        int bit = 0;

        // Lee el LSB del canal correspondiente
        switch (bitIndex % 3)
        {
            case 0: bit = pixel.R & 1; break;
            case 1: bit = pixel.G & 1; break;
            case 2: bit = pixel.B & 1; break;
        }

        b = (byte)((b << 1) | bit); // Agrega el bit al byte
        bitIndex++;
    }

    return b;
}
