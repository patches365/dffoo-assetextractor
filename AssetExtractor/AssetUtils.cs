using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AssetExtractor
{
    public static class AssetUtils
    {
        public static void CheckG1M(byte[] data, string filename)
        {
            byte[] G1MHeader = { 0x5F, 0x4D, 0x31, 0x47 };
            long startOffset = 0;
            int filenum = 0;

            while (!(startOffset < 0 || startOffset >= data.Length - 16))
            {
                startOffset = IndexOfBytes(data, G1MHeader, startOffset);
                if (startOffset < 0)
                {
                    Console.WriteLine("No G1M models found");
                    break;
                }
                else
                {
                    Console.WriteLine("G1M model found");

                    byte[] lengthbytes = new byte[4];
                    Array.Copy(data, startOffset + 8, lengthbytes, 0, 4);
                    long length = BitConverter.ToInt32(lengthbytes, 0);

                    byte[] g1mFile = new byte[length];
                    Array.Copy(data, startOffset, g1mFile, 0, length);
                    string g1mFilename = NewFilename(filename, filenum, "g1m");
                    File.WriteAllBytes(g1mFilename, g1mFile);
                    Console.WriteLine("G1M Model saved to: " + g1mFilename);
                    filenum++;
                    startOffset += length;
                }
            }
        }

        public static void CheckG1T(byte[] data, string filename)
        {
            byte[] G1THeader = { 0x47, 0x54, 0x31, 0x47 };
            long startOffset = 0;
            int filenum = 0;

            while (!(startOffset < 0 || startOffset >= data.Length - 16))
            {
                startOffset = IndexOfBytes(data, G1THeader, startOffset);
                if (startOffset < 0)
                {
                    Console.WriteLine("No G1T textures found");
                    break;
                }
                else
                {
                    Console.WriteLine("G1T texture found");

                    byte[] tableStart = new byte[4];
                    byte[] numTexture = new byte[4];
                    byte resolution;
                    long filesize = 0;
                    long pointer = 0;

                    Array.Copy(data, startOffset + 0x0C, tableStart, 0, 4);
                    Array.Copy(data, startOffset + 0x10, numTexture, 0, 4);
                    long tableOffset = BitConverter.ToInt32(tableStart, 0);
                    int textureCount = BitConverter.ToInt32(numTexture, 0);
                    pointer = startOffset + tableOffset + (textureCount * 4);

                    for (int i = 0; i < textureCount; i++)
                    {
                        resolution = data[pointer + 2];
                        byte type = data[pointer + 1];
                        pointer += 0x14;
                        double width = (resolution >> 4) & 0x0F;
                        double height = resolution & 0x0F;
                        width = Math.Pow(2, width);
                        height = Math.Pow(2, height);
                        long bytesize = 0;
                        if (type < 0x0F || type == 0x47 || type == 0x48)
                        {
                            bytesize = (long)(width * height) * 4;
                        }
                        else if (type == 0x34 || type == 035 || type == 0x36)
                        {
                            bytesize = (long)(width * height) * 4;
                        }
                        else if (type == 0x56 || type == 0x58)
                        {
                            bytesize = (long)(width * height) / 2;
                        }
                        else if (type == 0x57)
                        {
                            bytesize = (long)(width * height) / 4;
                        }
                        else
                        {
                            bytesize = (long)(width * height);
                        }
                        pointer += bytesize;
                    }

                    long length = pointer - startOffset;
                    byte[] g1tFile = new byte[length];
                    Array.Copy(data, startOffset, g1tFile, 0, length);
                    string g1tFilename = NewFilename(filename, filenum, "g1t");
                    File.WriteAllBytes(g1tFilename, g1tFile);
                    Console.WriteLine("G1T Texture saved to: " + g1tFilename);
                    filenum++;
                    startOffset += length;
                }
            }
        }

        public static void CheckG2A(byte[] data, string filename)
        {
            byte[] G2AHeader = { 0x5F, 0x41, 0x32, 0x47 };
            long startOffset = 0;
            int filenum = 0;

            while (!(startOffset < 0 || startOffset >= data.Length - 16))
            {
                startOffset = IndexOfBytes(data, G2AHeader, startOffset);
                if (startOffset < 0)
                {
                    Console.WriteLine("No G2A animations found");
                    break;
                }
                else
                {
                    Console.WriteLine("G2A animation found");

                    byte[] lengthbytes = new byte[4];
                    Array.Copy(data, startOffset + 8, lengthbytes, 0, 4);
                    long length = BitConverter.ToInt32(lengthbytes, 0);

                    byte[] g2aFile = new byte[length];
                    Array.Copy(data, startOffset, g2aFile, 0, length);
                    string g2aFilename = NewFilename(filename, filenum, "g2a");
                    File.WriteAllBytes(g2aFilename, g2aFile);
                    Console.WriteLine("G2A Animation saved to: " + g2aFilename);
                    filenum++;
                    startOffset += length;
                }
            }
        }

        private static string NewFilename(string oldFilename, int index, string extension)
        {
            int oldExtension = oldFilename.LastIndexOf('.');
            string newFilename = oldFilename.Substring(0, oldExtension) + "_" + index.ToString("000") + "." + extension;
            return newFilename;
        }

        public static long IndexOfBytes(this byte[] data, byte[] pattern, long startIndex)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (pattern == null) throw new ArgumentNullException(nameof(pattern));
            if (pattern.LongLength > data.LongLength) return -1;

            var cycles = data.LongLength - pattern.LongLength + 1;
            long patternIndex;
            for (var dataIndex = startIndex; dataIndex < cycles; dataIndex++)
            {
                if (data[dataIndex] != pattern[0]) continue;
                for (patternIndex = pattern.Length - 1; patternIndex >= 1; patternIndex--) if (data[dataIndex + patternIndex] != pattern[patternIndex]) break;
                if (patternIndex == 0) return dataIndex;
            }
            return -1;
        }
    }
}
