using System;
using System.Collections;
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
    class BinaryRelAnalysis
    {
        public static string BaseFilePath = Utils.currentDirectorypath();

        public static string labelFilName = "label.txt";
        public static string lineNumberLabelFileName = "lineNumber_label.txt";
        public static string multiLebelFileName = "class_for_multiple_protein_plant.txt";
        public static string multiLevelClassFileWritename = "MultiClass.txt";
        public static string bigramFromPSSM = "BigramFromPlant.txt";
        public static string binaryRelBaseFileName = "class";
        public static string binaryRelBaseArrfFileName = "BinaryRel_PSSMSS";

        public static string binaryRelevenceData()
        {

            int baseClasses = 12;

            FilePreparation.CreateMultiLevelClassFile();

            FilePreparation.PrepareLabelFileWithLineNumber(
                Path.Combine(BaseFilePath, labelFilName),
                Path.Combine(BaseFilePath, lineNumberLabelFileName));

            FilePreparation.MergeTwoClassFileWithMultiClass(
                new string[] { Path.Combine(BaseFilePath, multiLebelFileName), Path.Combine(BaseFilePath, lineNumberLabelFileName) },
                Path.Combine(BaseFilePath, multiLevelClassFileWritename));


            for (int eachClass = 1; eachClass <= baseClasses; eachClass++)
            {
                string eachClassFileName = String.Format("{0}_{1}.txt", binaryRelBaseFileName, eachClass);
                string eachArrfFileName = String.Format("{0}_{1}.arff", binaryRelBaseArrfFileName, eachClass);

                binaryRelevenceFeatureExterctionFile(
                    Path.Combine(BaseFilePath, bigramFromPSSM),
                    Path.Combine(BaseFilePath, eachClassFileName),
                    Path.Combine(BaseFilePath, eachArrfFileName),
                    eachClass);
            }


            //Weka.classifierTwo(
            //Path.Combine(BaseFilePath, arrfFileName),
            //Path.Combine(BaseFilePath, predictionModel));

            //Weka.classifyTest(Path.Combine(BaseFilePath, binaryRelBaseArrfFileName), baseClasses);


            string performance = String.Empty;
            string[] _classifierList = new string[] { "svm", "j48", "nb", "rf" };
            for (int loop = 0; loop < _classifierList.Length; loop++)
            {
                //performance += _classifierList[loop] + " - " + Weka.resultPrepare(Path.Combine(BaseFilePath, binaryRelBaseArrfFileName), baseClasses, _classifierList[loop]).ToString()+"\n ";
                performance += _classifierList[loop] + " - " + resultPrepareWithCrossFold(Path.Combine(BaseFilePath, binaryRelBaseArrfFileName), baseClasses, _classifierList[loop]).ToString() + "\n ";
                System.Console.WriteLine(performance);

            }
            //Weka.predictClass(Path.Combine(BaseFilePath, arrfFileName));

            return performance;

            //Console.ReadKey();
        }


        public static void binaryRelevenceFeatureExterctionFile(string pssmFile, string classFile, string arrfFile, int otherClass)
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

            //Randomized the data set
            //var rnd = new Random();
            //List<string> result = dataList.OrderBy(item => rnd.Next()).ToList();

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


                string classNames = String.Format("Class0,Class{0}", otherClass);


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


        public static void BRRandomizeArffFile()
        {
            //int baseClasses = 8;

            //for (int eachClass = 1; eachClass <= baseClasses; eachClass++)
            //{

            string currentDirectory = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            currentDirectory = currentDirectory.TrimEnd('\\');
            currentDirectory = currentDirectory.Remove(currentDirectory.LastIndexOf('\\') + 1);

            string classFilePath = Path.Combine(currentDirectory, "Files\\BR_ClassFIles\\");
            //string classFilePath = Path.Combine(currentDirectory, "pssmConvertedGN2\\pssm");

            int fileCount = (from file in Directory.EnumerateFiles(classFilePath, "class_*.txt", SearchOption.AllDirectories)
                             select file).Count();


            List<BrFilesData> listAll = new List<BrFilesData>();

            StreamReader file1 = new StreamReader(Path.Combine(BaseFilePath, bigramFromPSSM));
            List<string> instancesLines = new List<string>();
            string featureLine;
            while ((featureLine = file1.ReadLine()) != null) instancesLines.Add(featureLine);
            file1.Close();

            var rand = new Random(300);
            ArrayList allBrData = new ArrayList();
            ArrayList allRandBrData = new ArrayList();
            for (int j = 1; j <= fileCount; j++)
            {
                BrFilesData _singleData = new BrFilesData();


                string classFile = classFilePath + "class_" + j + ".txt";
                //string eachClassFileName = String.Format("{0}_{1}.txt", binaryRelBaseFileName, eachClass);
                StreamReader file2 = new StreamReader(classFile);
                string classLines;

                List<string> classLinesList = new List<string>();

                while ((classLines = file2.ReadLine()) != null) classLinesList.Add(classLines);
                List<string> dataList = new List<string>();

                file2.Close();

                for (int i = 0; i < instancesLines.Count; i++)
                {
                    string data = String.Empty;
                    data = instancesLines[i] + ",Class" + classLinesList[i];
                    dataList.Add(data);
                }
                allBrData.Add(dataList);

                //List<string> result = dataList.OrderBy(item => rand.Next()).ToList<string>();

                //allRandBrData.Add(result);
            }






            
            List<string> en0 = new List<string>(new string[] { "horse", "cat", "dog", "milk", "honey" });
            List<string> en1 = new List<string>(new string[] { "horse1", "cat2", "dog3", "milk4", "honey5" });
            //List<string> en2 = new List<string>(new string[] { "horse", "cat", "dog", "milk", "honey" });
            //List<string> en3 = new List<string>(new string[] { "horse1", "cat2", "dog3", "milk4", "honey5" });
            //List<string> en4 = new List<string>(new string[] { "horse", "cat", "dog", "milk", "honey" });
            //List<string> en5 = new List<string>(new string[] { "horse1", "cat2", "dog3", "milk4", "honey5" });
            //List<string> en6 = new List<string>(new string[] { "horse", "cat", "dog", "milk", "honey" });
            //List<string> en7 = new List<string>(new string[] { "horse1", "cat2", "dog3", "milk4", "honey5" });

            var joined = en0.Zip(en1, (x, y) => new { x, y });


            //list4 = list1.Zip(list2.Zip(list3, (b, c) => new { b, c }), (a, b) => new Item { Value1 = a, Value2 = b.b, Value3 = b.c })
            // .ToList();



            var shuffled = joined.OrderBy(x => Guid.NewGuid()).ToList();
            en0 = shuffled.Select(pair => pair.x).ToList();
            en1 = shuffled.Select(pair => pair.y).ToList();
            





            //var rnd = new Random();
            //var result = source.OrderBy(item => rnd.Next());






            //string eachArrfFileName = String.Format("{0}_{1}.arff", binaryRelBaseArrfFileName, eachClass);
            //}


            //StreamReader file2 = new StreamReader(classFile);






            //Randomized the data set
            //var rnd = new Random();
            //List<string> result = dataList.OrderBy(item => rnd.Next()).ToList();

            //List<string> result = Utils.ShuffleList(dataList);



            /*
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


                string classNames = String.Format("Class0,Class{0}", otherClass);


                file.WriteLine("@ATTRIBUTE class {" + classNames + "}");
                file.WriteLine("@data");
                file.WriteLine();

                for (int i = 0; i < lines1.Count; i++)
                {
                    file.WriteLine(result[i]);
                }
            }
            */

            //Console.WriteLine(arrfFile + "\n file Successfuly written!!!!!!!");
            //Console.ReadKey();
        }


        public static void binaryRelevenceArffFilePreperation()
        {
            string currentDirectory = Utils.currentDirectorypath() + "\\BR_Files";

            string[] fileEntries = Directory.GetFiles(currentDirectory);

        }



        public static double resultPrepareWithCrossFold(string classifierFileName, int baseClasses, string clasifier)
        {
            double performance = 0.0;

            if (clasifier == "svm")
            {
                weka.classifiers.Classifier svm = new SMO();

                performance = Weka.classifyCrossFold_Train_Test(classifierFileName, baseClasses, svm);

            }
            else if (clasifier == "nb")
            {
                weka.classifiers.Classifier nb = new NaiveBayes();
                performance = Weka.classifyCrossFold_Train_Test(classifierFileName, baseClasses, nb);
            }
            else if (clasifier == "rf")
            {
                weka.classifiers.Classifier rf = new RandomForest();
                performance = Weka.classifyCrossFold_Train_Test(classifierFileName, baseClasses, rf);
            }
            else if (clasifier == "j48")
            {
                weka.classifiers.Classifier j48 = new J48();
                performance = Weka.classifyCrossFold_Train_Test(classifierFileName, baseClasses, j48);
            }
            return performance;
        }


    }
}
