using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetExtractor
{
    class FileReader
    {
        public List<string> filenames;

        public FileReader()
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Multiselect = true;
            if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filenames = ofd.FileNames.ToList<string>();
            }
        }

        public FileReader(string[] args)
        {
            filenames = args.ToList<string>();
        }
    }
}
