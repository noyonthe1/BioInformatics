using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protein_Multi_Level_Classification
{
    class FilePreparation
    {

        public static void CreateMultiLevelClassFile()
        {

        }

        public static void classMapingToFile()
        {

        }

        public static void binaryRelevenceDataPreparation(Dictionary<string, string> MultiLevelClass, string binaryRelBaseFileName)
        {
            var binaryRelevenceClass = new Dictionary<int, string>();
            int classes = 12;
            var fileName = "";
            for (int oneClass = 1; oneClass <= classes; oneClass++)
            {
                var binaryFileEach = "";
                StringBuilder sb = new StringBuilder();
                foreach (var key in MultiLevelClass.Keys)
                {

                    var multiLevelClass = MultiLevelClass[key].ToString();
                    string[] a = multiLevelClass.Split(' ');

                    if (oneClass == Convert.ToInt32(a[0]))
                    {
                        binaryRelevenceClass[Convert.ToInt32(key)] = a[0];
                    }
                    else if (Convert.ToInt32(a[1]) != 0 && oneClass == Convert.ToInt32(a[1]))
                    {
                        binaryRelevenceClass[Convert.ToInt32(key)] = a[1];
                    }
                    else if (Convert.ToInt32(a[2]) != 0 && oneClass == Convert.ToInt32(a[2]))
                    {
                        binaryRelevenceClass[Convert.ToInt32(key)] = a[2];
                    }
                    else
                    {
                        binaryRelevenceClass[Convert.ToInt32(key)] = "0";
                    }
                    binaryFileEach = binaryRelevenceClass[Convert.ToInt32(key)].ToString() + "\n";
                    sb.Append(binaryFileEach);
                }
                fileName = String.Format("{0}_{1}.txt", binaryRelBaseFileName, oneClass);

                File_Helper.WriteToFile(sb, fileName);
            }
            Console.Write("");

        }
        public static int MultiClassToSingleClassConverter(string multiLevelClassFileName, string ClassFileWritename, string classMappingFile, string multiLevelClassFileWithLineNumberWritename, string binaryRelFilePath)
        {
            var MultiLevelClass = new Dictionary<string, string>();
            var NewMultiLevelClass = new Dictionary<int, string>();
            var classMapping = new Dictionary<string, string>();

            string[] fileContents = File.ReadAllLines(multiLevelClassFileName);
            foreach (string line in fileContents)
            {
                string[] a = line.Split(':');
                if (!MultiLevelClass.Keys.Contains(a[0]))
                {
                    MultiLevelClass[a[0]] = a[1];
                    NewMultiLevelClass[Convert.ToInt32(a[0])] = a[1];
                }
            }

            var distinctList = MultiLevelClass.Values.Distinct().ToList();
            int numberOfClass = Convert.ToInt32(distinctList.Count);
            int MakeNewClass = 0;
            foreach (var distinct in distinctList)
            {
                MakeNewClass++;
                foreach (KeyValuePair<string, string> eachDictionaryItem in MultiLevelClass)
                {
                    if (eachDictionaryItem.Value == distinct)
                    {
                        NewMultiLevelClass[Convert.ToInt32(eachDictionaryItem.Key)] = MakeNewClass.ToString();
                        Console.WriteLine("");
                    }

                }
                classMapping["Class" + MakeNewClass.ToString()] = distinct.ToString();
            }


            //classMapingToFile(NewMultiLevelClass, distinctList);

            StringBuilder sb = new StringBuilder();
            StringBuilder sbLine = new StringBuilder();

            foreach (var key in NewMultiLevelClass.Keys)
            {
                var multiLevelClass = NewMultiLevelClass[key].ToString() + "\n";
                var multiLevelClassWithLineNumber = key + ":" + NewMultiLevelClass[key].ToString() + "\n";
                Console.WriteLine(key + ":" + NewMultiLevelClass[key]);

                sb.Append(multiLevelClass);
                sbLine.Append(multiLevelClassWithLineNumber);
            }

            File_Helper.WriteToFile(sb, ClassFileWritename);
            File_Helper.WriteToFile(sbLine, multiLevelClassFileWithLineNumberWritename);

            StringBuilder sb1 = new StringBuilder();
            foreach (var key in classMapping.Keys)
            {
                var multiLevelClass = key + ":" + classMapping[key].ToString() + "\n";
                Console.WriteLine(key + ":" + classMapping[key]);

                sb1.Append(multiLevelClass);
            }

            File_Helper.WriteToFile(sb1, classMappingFile);


            binaryRelevenceDataPreparation(MultiLevelClass, binaryRelFilePath);

            return numberOfClass;
        }

        public static void MergeTwoClassFileWithMultiClass(string[] files, string writeFilePath)
        {
            var hash = new Dictionary<int, string>();
            foreach (var file in files)
            {
                string[] fileContents = File.ReadAllLines(file);
                foreach (string line in fileContents)
                {
                    string[] a = line.Split(' ');
                    if (!hash.Keys.Contains(Convert.ToInt32(a[0])))
                    {
                        if (a.Length > 2)
                        {
                            //hash[Convert.ToInt32(a[0])] = a[1] + " " + a[2];

                            hash[Convert.ToInt32(a[0])] = a[1] + " " + a[2] + " " + a[3];

                        }
                        else
                        {
                            hash[Convert.ToInt32(a[0])] = a[1] + " " + "0" + " " + "0";
                        }
                    }
                }
            }

            var orderedWithLineNumber = hash.OrderBy(key => key.Key);
            var orderedWithLineNumberDictionary = orderedWithLineNumber.ToDictionary((keyItem) => keyItem.Key, (valueItem) => valueItem.Value);

            StringBuilder sb = new StringBuilder();
            foreach (var key in orderedWithLineNumberDictionary.Keys)
            {
                var multiLevelClass = key.ToString() + ":" + orderedWithLineNumberDictionary[key].ToString() + "\n";
                Console.WriteLine(key + ":" + orderedWithLineNumberDictionary[key]);

                sb.Append(multiLevelClass);
            }
            File_Helper.WriteToFile(sb, writeFilePath);
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
