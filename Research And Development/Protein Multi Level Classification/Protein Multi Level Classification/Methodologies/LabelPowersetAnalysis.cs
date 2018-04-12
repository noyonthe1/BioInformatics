using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weka.classifiers.bayes;
using weka.classifiers.functions;
using weka.classifiers.trees;

namespace Protein_Multi_Level_Classification
{
    class LabelPowersetAnalysis
    {
        public static string labelPowersetData()
        {
            string currentDirectory = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            currentDirectory = currentDirectory.TrimEnd('\\');
            currentDirectory = currentDirectory.Remove(currentDirectory.LastIndexOf('\\') + 1);

            string BaseFilePath = Path.Combine(currentDirectory, "Files\\");
            string arrfFileName = "MultiClass_PSSMSS.arff";
            string multiLevelClassFileWithLineNumberWritename = "MultiClassWithLine.txt";
            string classFileWritename = "finalClass.txt";
            string predictionModel = "predictionModel";
            string classMappingFile = "classMappingFile.txt";
            string multiLevelClassFileWritename = "MultiClass.txt";
            string bigramFromPSSM = "BigramFromPSSM_Hasnaeen.txt";
            string binaryRelBaseFileName = "class";



            int numberOfClass = FilePreparation.MultiClassToSingleClassConverter(
                Path.Combine(BaseFilePath, multiLevelClassFileWritename),
                Path.Combine(BaseFilePath, classFileWritename),
                Path.Combine(BaseFilePath, classMappingFile),
                Path.Combine(BaseFilePath, multiLevelClassFileWithLineNumberWritename),
                Path.Combine(BaseFilePath, binaryRelBaseFileName));

            labelPowersetFeatureExterctionFile(
                Path.Combine(BaseFilePath, bigramFromPSSM),
                Path.Combine(BaseFilePath, classFileWritename),
                Path.Combine(BaseFilePath, arrfFileName),
                numberOfClass);



            string performance = String.Empty;
            string[] _classifierList = new string[] { "svm", "j48", "nb", "rf" };
            for (int loop = 0; loop < _classifierList.Length; loop++)
            {
                performance += _classifierList[loop] + " - " + resultPrepare(Path.Combine(BaseFilePath, arrfFileName), _classifierList[loop]).ToString() + "\n ";
                System.Console.WriteLine(performance);

            }
            //Weka.predictClass(Path.Combine(BaseFilePath, arrfFileName));

            return performance;


        }

        public static void labelPowersetFeatureExterctionFile(string pssmFile, string classFile, string arrfFile, int numberOfClass)
        {
            StreamReader file1 = new StreamReader(pssmFile);
            StreamReader file2 = new StreamReader(classFile);

            string line;
            List<string> lines1 = new List<string>();
            List<string> lines2 = new List<string>();

            List<string> dataList = new List<string>();


            while ((line = file1.ReadLine()) != null) lines1.Add(line);
            while ((line = file2.ReadLine()) != null) lines2.Add(line);

            file1.Close();
            file2.Close();

            for (int i = 0; i < lines1.Count; i++)
            {
                string data = String.Empty;
                data = lines1[i] + ",Class" + lines2[i];
                dataList.Add(data);
            }

            //var rnd = new Random();
            //List<string> result = dataList.OrderBy(item => rnd.Next()).ToList();


            //Randomized the data set
            List<string> result = Utils.ShuffleList(dataList);

            if (File.Exists(arrfFile))
            {
                File.Delete(arrfFile);
            }
            using (StreamWriter file = new StreamWriter(arrfFile))
            {
                file.WriteLine("@relation tt");
                file.WriteLine();

                for (int i = 0; i < 400; i++)
                {
                    file.WriteLine("@attribute " + "Z" + i + " numeric");
                }

                string classNames = string.Empty;
                for (int i = 1; i <= numberOfClass; i++)
                {
                    classNames += "Class" + i + ",";
                }

                classNames = classNames.Remove(classNames.Length - 1);

                file.WriteLine("@ATTRIBUTE class {" + classNames + "}");
                file.WriteLine("@data");
                file.WriteLine();

                for (int i = 0; i < lines1.Count; i++)
                {
                    file.WriteLine(result[i]);
                }
            }

            Console.WriteLine(arrfFile + "\n file Successfuly written!!!!!!!");
            //Console.ReadKey();
        }


        public static double resultPrepare(string classifierFileName, string clasifier)
        {
            double performance = 0.0;

            if (clasifier == "svm")
            {
                weka.classifiers.Classifier svm = new SMO();
                performance = Weka.classifyTrain_Test(classifierFileName, svm);

            }
            else if (clasifier == "nb")
            {
                weka.classifiers.Classifier nb = new NaiveBayes();
                performance = Weka.classifyTrain_Test(classifierFileName, nb);
            }
            else if (clasifier == "rf")
            {
                weka.classifiers.Classifier rf = new RandomForest();
                performance = Weka.classifyTrain_Test(classifierFileName, rf);
            }
            else if (clasifier == "j48")
            {
                weka.classifiers.Classifier j48 = new J48();
                performance = Weka.classifyTrain_Test(classifierFileName, j48);
            }
            return performance;
        }
    }
}
