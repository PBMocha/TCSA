using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Net.Mime;

namespace GenetecChallenge.N1.Repository
{
    public class FileRepository
    {

        public FileRepository()
        {

        }

        public void ByteToImage(byte[] imgBits)
        {
            
            

        }

        public string ReadImage(byte[] bits, string plate)
        {
            string path = plate + ".jpg";

            File.WriteAllBytes(path, bits);
            return path;
        }

        public void CleanFiles(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

    }
}
