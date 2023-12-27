using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

namespace AssetExtractor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main(string[] args)
        {
            /*Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());*/

            AllocConsole();
            FileReader fileReader;

            if(args.Length < 1)
            {
                Console.WriteLine("Select file(s) to extract...");
                fileReader = new FileReader();
            }
            else
            {
                fileReader = new FileReader(args);
            }

            try
            {
                foreach (string file in fileReader.filenames)
                {
                    Console.WriteLine("-----------------------");
                    Console.WriteLine("Checking " + file);
                    Console.WriteLine("");
                    byte[] data;

                    try
                    {
                        data = File.ReadAllBytes(file);
                    }
                    catch
                    {
                        Console.WriteLine(file + " not found.");
                        continue;
                    }

                    AssetUtils.CheckG1M(data, file);
                    AssetUtils.CheckG1T(data, file);
                    AssetUtils.CheckG2A(data, file);

                }
            }
            catch
            {
                Console.WriteLine("Could not read files.");
            }
            Console.WriteLine("");
            Console.WriteLine("Done. Press any key");
            Console.ReadKey();
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
    }
}
