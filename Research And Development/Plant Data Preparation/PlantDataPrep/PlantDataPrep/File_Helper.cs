using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantDataPrep
{
    class File_Helper
    {
        public static void WriteToFile(StringBuilder sb, string writeFilePath)
        {
            if (File.Exists(writeFilePath))
            {
                File.Delete(writeFilePath);
            }
            System.Console.WriteLine("\nStat Writting to the file \n" + writeFilePath);
            StreamWriter sw1 = File.AppendText(writeFilePath);
            sw1.Write(sb);

            sw1.Close();

            System.Console.WriteLine("\nFinished Writting \n");
        }
    }
}
