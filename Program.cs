using System;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace DesktopFilter
{
    class Program
    {
        static void Main(string[] args)
        {
            // read registry key (it's binary)
            byte[] wallpaperBytes = Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "TranscodedImageCache",0) as byte[];
            if (wallpaperBytes == null)
            {
                return;
            }

            // convert bytes to text
            const int shift = 24;
            char[] sourceChars = Encoding.UTF8.GetChars(wallpaperBytes, shift, wallpaperBytes.Length - shift);

            // trim empty chars at the end
            int pathLength = sourceChars.Length - 1;
            while (sourceChars[pathLength] == '\0')
            {
                pathLength--;
            }
            
            // clean strange spaces between chars
            bool copy = true;
            char[] targetChars = new char[pathLength / 2 + 1];
            for (int n = 0, m = 0; n <= pathLength; n++)
            {
                if (copy)
                {
                    targetChars[m] = sourceChars[n];
                }
                else
                {
                    m++;
                }
                copy = !copy;
            }
            string wallpaperPath = new string(targetChars);

            // delete the file
            if (File.Exists(wallpaperPath))
            {
                string isDebug = string.Empty;
#if DEBUG
                isDebug = " (debug)";
#endif
                Console.WriteLine($"You are about to delete{isDebug}: {wallpaperPath} (Y/N)?");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
#if DEBUG
                        Console.WriteLine("DELETED!");
                        Console.ReadKey();
#else
                        File.Delete(wallpaperPath);
#endif
                }
            }
        }
    }
}
