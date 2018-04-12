using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Protein_Multi_Level_Classification
{
    class Pssm_File_Creation
    {
        public static void pssmFilePreparation()
        {
            string currentDirectory = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            currentDirectory = currentDirectory.TrimEnd('\\');
            currentDirectory = currentDirectory.Remove(currentDirectory.LastIndexOf('\\') + 1);

            string BaseFilePath = Path.Combine(currentDirectory, "plant2_pssm\\");
            string pssmConvertedFilePath = Path.Combine(currentDirectory, "pssmConvertedPlant2\\pssm");

            int fileCount = (from file in Directory.EnumerateFiles(BaseFilePath, "pssm*.txt", SearchOption.AllDirectories)
                             select file).Count();

            for (int i = 1; i <= fileCount; i++)
            {
                string filePath = BaseFilePath + "pssm" + i + ".txt";
                if (File.Exists(filePath))
                {
                    string[] fileContents = File.ReadAllLines(filePath);
                    string[] slicedAllFileContents = fileContents.Skip(3).Take(fileContents.Length).ToArray();
                    //string[] slicedfileContents = slicedAllFileContents.Substring(10);
                    string[] resultLines = new string[fileContents.Length];
                    int lineNumber = 0;

                    foreach (string line in slicedAllFileContents)
                    {
                        if (!(string.IsNullOrEmpty(line)) && (line.Length > 9))
                        {
                            string eachLine = line.Substring(9);
                            string eachLineWithComma = Regex.Replace(eachLine, ".{3}", "$0,");
                            string eachLineWithoutWhiteSpace = Regex.Replace(eachLineWithComma, @"\s+", "");

                            string[] lineCharArray = eachLineWithoutWhiteSpace.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            string[] cleanedArray = lineCharArray.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                            string[] slicedArray = cleanedArray.Take(20).ToArray();

                            string resultString = string.Join(",", slicedArray);
                            int n;
                            if (int.TryParse(slicedArray[0], out n))
                            {
                                int test;
                                if (int.TryParse(resultString.Split(',').Sum(x => int.Parse(x)).ToString(), out test))
                                {
                                    resultLines[lineNumber] = resultString;
                                }
                            }
                            lineNumber++;
                        }
                    }
                    string[] cleanedresultLines = resultLines.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    string allLinesOfSingleFile = string.Join("\n", cleanedresultLines);

                    StringBuilder sb = new StringBuilder();
                    sb.Append(allLinesOfSingleFile);
                    File_Helper.WriteToFile(sb, (pssmConvertedFilePath + i + ".txt"));
                }
                else
                {
                    Console.WriteLine("File doesn't exist\n" + filePath);
                }
            }

        }
    }
}
