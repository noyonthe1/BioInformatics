using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlantDataPrep
{
    class Program
    {
        static void Main(string[] args)
        {
            string currentDirectory = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            currentDirectory = currentDirectory.TrimEnd('\\');
            currentDirectory = currentDirectory.Remove(currentDirectory.LastIndexOf('\\') + 1);

            string BaseFilePath = Path.Combine(currentDirectory, "Files\\");

            string today = DateTime.Now.ToString();

            string sequesceFileName = Path.Combine(BaseFilePath, "sequence.txt");
            string labelFileName = Path.Combine(BaseFilePath, "label.txt");
            string labelFileNameWIthLine = Path.Combine(BaseFilePath, "lineNumber_label.txt");
            string writeFilePath = Path.Combine(BaseFilePath, "result.txt");

            string class_for_multiple_protein_plant = Path.Combine(BaseFilePath, "class_for_multiple_protein_GN.txt");






            StreamReader sequesceFile = new StreamReader(sequesceFileName);
            StreamReader labelFile = new StreamReader(labelFileName);

            string line;
            List<string> sequesceFilelines = new List<string>();
            List<string> labelFilelines = new List<string>();

            List<string> dataList = new List<string>();


            while ((line = sequesceFile.ReadLine()) != null) sequesceFilelines.Add(line);
            while ((line = labelFile.ReadLine()) != null) labelFilelines.Add(line);

            sequesceFile.Close();
            labelFile.Close();

            //Dictionary<string, string> sequenceVsLabel = new Dictionary<string, string>();

            List<string> result = new List<string>();

            var duplicatesWithIndices = sequesceFilelines
                .Select((Name, Index) => new { Name, Index })
                .GroupBy(x => x.Name)
                .Select(xg => new {
                    Name = xg.Key,
                    Indices = xg.Select(x => x.Index)
                })
                .Where(x => x.Indices.Count() > 1);


            StringBuilder sb = new StringBuilder();

            foreach (var g in duplicatesWithIndices)
            {
                string write = String.Format("{0} : {1} \n", g.Indices.ToArray()[0], string.Join(",", g.Indices.ToArray()));
                Console.WriteLine(write);
                sb.Append(write);
            }

            File_Helper.WriteToFile(sb, writeFilePath);


            PrepareLabelFileWithLineNumber(labelFileName, labelFileNameWIthLine);
            MultiClassPrepare(writeFilePath, class_for_multiple_protein_plant);



            Console.WriteLine();






        }
        public static void MultiClassPrepare(string writeFilePath, string class_for_multiple_protein_plant)
        {

            StreamReader sequesceFile = new StreamReader(writeFilePath);

            string line;
            List<string> sequesceFilelines = new List<string>();
            
            while ((line = sequesceFile.ReadLine()) != null) sequesceFilelines.Add(line);
            foreach (string eachline in sequesceFilelines)
            {
                string[] a = eachline.Split(':');
            }
            Console.WriteLine();

        }

        public static void PrepareLabelFileWithLineNumber(string readFilePath, string writeFilePath)
        {

            StringBuilder sb = new StringBuilder();

            if (File.Exists(readFilePath))
            {
                StreamReader sr1 = File.OpenText(readFilePath);

                string s = "";
                int counter = 1;
                while ((s = sr1.ReadLine()) != null)
                {
                    var lineOutput = counter++ + " " + s + "\n";
                    Console.WriteLine(lineOutput);

                    sb.Append(lineOutput);
                }
                sr1.Close();
            }
            else
            {
                Console.WriteLine("File not found");
                Console.ReadLine();
            }

            Console.WriteLine();
            File_Helper.WriteToFile(sb, writeFilePath);
        }
    }
}
