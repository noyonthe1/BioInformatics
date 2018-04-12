using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.sun.tools.@internal.ws.processor.model;
using java.io;
using weka.classifiers;
using weka.classifiers.trees;
using java.util;
using weka.classifiers.evaluation;
using weka.classifiers.functions;
using weka.core;
using Evaluation = weka.classifiers.Evaluation;
using com.sun.source.tree;
using com.sun.tools.@internal.xjc.reader;
using ikvm.extensions;
using weka.classifiers.bayes;
using weka.core.neighboursearch;
using static ICSharpCode.SharpZipLib.Zip.Compression.DeflaterHuffman;
using Model = com.sun.tools.@internal.xjc.model.Model;

namespace Protein_Multi_Level_Classification
{
    class Weka
    {
        const int percentSplit = 80;

        #region classifierTesting
        public static void classifierOne(string classifierFileName, string predictionModel)
        {
            FileReader javaFileReader = new FileReader(classifierFileName);
            weka.core.Instances wekaInsts = new weka.core.Instances(javaFileReader);
            javaFileReader.close();

            wekaInsts.setClassIndex(wekaInsts.numAttributes() - 1);
            Classifier cl = new SMO();
            //Classifier cl = new NaiveBayes();
            java.util.Random random = new java.util.Random(1);
            Evaluation evaluation = new Evaluation(wekaInsts);

            evaluation.crossValidateModel(cl, wekaInsts, 10, random);

            foreach (object o in evaluation.getMetricsToDisplay().toArray())
            {

            }
            int count = 0;
            StringBuilder sb = new StringBuilder();
            foreach (object o in evaluation.predictions().toArray())
            {
                NominalPrediction prediction = o as NominalPrediction;
                if (prediction != null)
                {
                    double[] distribution = prediction.distribution();
                    double predicted = prediction.predicted();
                    double actual = prediction.actual();
                    string revision = prediction.getRevision();
                    double weight = prediction.weight();
                    double margine = prediction.margin();
                    //bool equals = prediction.@equals();

                    string distributions = String.Empty;
                    for (int i = 0; i < distribution.Length; i++)
                    {
                        //System.Console.WriteLine(distribution[i]);
                        distributions += distribution[i];
                    }
                    var predictionLine = String.Format("{0} - {1} - {2} - {3} - {4} - {5}\n", actual, predicted, revision, weight, margine, distributions);
                    sb.Append(predictionLine);
                    //System.Console.WriteLine(predicted);

                }
                count++;
            }
            File_Helper.WriteToFile(sb, predictionModel + "NbCl.txt");
            System.Console.WriteLine(count);
            System.Console.ReadKey();
        }
        #endregion

        #region classifierTwo
        public static void classifierTwo(string classifierFileName, string predictionModel)
        {
            FileReader javaFileReader = new FileReader(classifierFileName);
            weka.core.Instances wekaInsts = new weka.core.Instances(javaFileReader);
            javaFileReader.close();

            wekaInsts.setClassIndex(wekaInsts.numAttributes() - 1);



            //Classifier nbTree = (Classifier)SerializationHelper.read(Model) as J48;

            Instances testDataSet = new Instances(new BufferedReader(new FileReader(classifierFileName)));
            testDataSet.setClassIndex(wekaInsts.numAttributes() - 1);
            //testDataSet.setClassIndex(10);
            Evaluation evaluation = new Evaluation(testDataSet);


            J48 model = new J48();

            //Classifier myClassifier = (Classifier)SerializationHelper.read(Model) as NaiveBayes;
            //Classifier myClassifier = new NaiveBayes();


            for (int i = 0; i < testDataSet.numInstances(); i++)
            {
                Instance instance = testDataSet.instance(i);
                //evaluation.evaluateModelOnceAndRecordPrediction(myClassifier, instance);
                //evaluation.evaluateModelOnce(myClassifier, instance);
            }

            foreach (object o in evaluation.predictions().toArray())
            {
                NominalPrediction prediction = o as NominalPrediction;
                if (prediction != null)
                {
                    double[] distribution = prediction.distribution();
                    double predicted = prediction.predicted();

                    for (int i = 0; i < distribution.Length; i++)
                    {
                        System.Console.WriteLine(distribution[i]);
                    }

                    System.Console.WriteLine(predicted);

                }
            }

            System.Console.WriteLine(evaluation);
            System.Console.ReadKey();
        }
        #endregion

        public static double classifyTrain_Test(string classifierFileName, Classifier _classifier)
        {
            double performance = 0.0;
            try
            {
                FileReader javaFileReader = new FileReader(classifierFileName);
                weka.core.Instances insts = new weka.core.Instances(javaFileReader);
                javaFileReader.close();

                insts.setClassIndex(insts.numAttributes() - 1);

                System.Console.WriteLine("Performing " + percentSplit + "% split evaluation.");

                int trainSize = insts.numInstances() * percentSplit / 100;
                int testSize = insts.numInstances() - trainSize;
                weka.core.Instances train = new weka.core.Instances(insts, 0, trainSize);

                _classifier.buildClassifier(train);

                int numCorrect = 0;
                var numnerOfInst = insts.numInstances();
                int dataIndex = 0;
                for (int i = trainSize; i < numnerOfInst; i++)
                {
                    dataIndex++;
                    weka.core.Instance currentInst = insts.instance(i);

                    double predictClass = _classifier.classifyInstance(currentInst);
                    double[] dist = _classifier.distributionForInstance(currentInst);


                    string actualClass = insts.classAttribute().value((int)insts.instance(i).classValue());
                    string predictedClass = insts.classAttribute().value((int)predictClass);


                    var abcd = _classifier.getClass();

                    if (predictedClass == actualClass)
                    {
                        numCorrect++;
                    }
                }
                performance = (double)((double)numCorrect / (double)testSize) * 100;

                System.Console.WriteLine(numCorrect + " out of " + testSize + " correct (" + performance.toString() + "%)");
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
            }

            return performance;
        }


        public static double resultPrepare(string classifierFileName, int baseClasses, string clasifier)
        {
            double performance = 0.0;

            if (clasifier == "svm")
            {
                weka.classifiers.Classifier svm = new SMO();
                performance = classifyTrain_Test(classifierFileName, baseClasses, svm); 
            }
            else if (clasifier == "nb")
            {
                weka.classifiers.Classifier nb = new NaiveBayes();
                performance = classifyTrain_Test(classifierFileName, baseClasses, nb);
            }
            else if (clasifier == "rf")
            {
                weka.classifiers.Classifier rf = new RandomForest();
                performance = classifyTrain_Test(classifierFileName, baseClasses, rf);
            }
            else if (clasifier == "j48")
            {
                weka.classifiers.Classifier j48 = new J48();
                performance = classifyTrain_Test(classifierFileName, baseClasses, j48);
            }
            return performance;
        }

        public static double classifyTrain_Test(string classifierFileName, int baseClasses, Classifier _classifier)
        {
            double performance = 0.0;
            try
            {
                List<BrResult> results = new List<BrResult>();
                for (int singleClass = 1; singleClass <= baseClasses; singleClass++)
                {
                    string eachFileName = String.Format("{0}_{1}.arff", classifierFileName, singleClass);

                    BrResult result = new BrResult();
                    result.classNumber = singleClass;

                    FileReader javaFileReader = new FileReader(eachFileName);
                    weka.core.Instances insts = new weka.core.Instances(javaFileReader);
                    javaFileReader.close();

                    insts.setClassIndex(insts.numAttributes() - 1);

                    System.Console.WriteLine("Performing " + percentSplit + "% split evaluation.");

                    int trainSize = insts.numInstances() * percentSplit / 100;
                    int testSize = insts.numInstances() - trainSize;
                    weka.core.Instances train = new weka.core.Instances(insts, 0, trainSize);

                    _classifier.buildClassifier(train);

                    int numCorrect = 0;
                    var numnerOfInst = insts.numInstances();
                    List<Result> eachResults = new List<Result>();
                    int dataIndex = 0;
                    for (int i = trainSize; i < numnerOfInst; i++)
                    {
                        dataIndex++;
                        Result eachRow = new Result();
                        eachRow.lineIndex = 0;
                        weka.core.Instance currentInst = insts.instance(i);

                        double predictClass = _classifier.classifyInstance(currentInst);
                        double[] dist = _classifier.distributionForInstance(currentInst);

                        string actualClass = insts.classAttribute().value((int)insts.instance(i).classValue());
                        string predictedClass = insts.classAttribute().value((int)predictClass);


                        var abcd = _classifier.getClass();

                        if (predictedClass == actualClass)
                        {
                            eachRow.correct = "1";
                            numCorrect++;
                        }
                        else
                        {
                            eachRow.correct = "0";
                        }
                        eachRow.lineIndex = dataIndex;
                        eachRow.classActual = actualClass;
                        eachRow.classPredicted = predictedClass;

                        eachResults.Add(eachRow);

                    }
                    result.classResult = eachResults;
                    results.Add(result);

                    System.Console.WriteLine(numCorrect + " out of " + testSize + " correct (" + (double)((double)numCorrect / (double)testSize * 100.0) + "%)");
                }

                #region Evaludation Matrix
                var evaluationMatrix = new Dictionary<int, string>();

                foreach (var res in results)
                {
                    foreach (var classRes in res.classResult)
                    {
                        if (!evaluationMatrix.Keys.Contains(classRes.lineIndex))
                        {
                            evaluationMatrix[classRes.lineIndex] = classRes.correct.toString();
                        }
                        else
                        {
                            evaluationMatrix[classRes.lineIndex] = evaluationMatrix[classRes.lineIndex].toString() + "," + classRes.correct.toString();
                        }
                    }
                }
                #endregion

                #region
                int correnctlyClassified = 0;
                int incorrenctlyClassified = 0;
                int totalData = evaluationMatrix.Count;
                foreach (var key in evaluationMatrix.Keys)
                {
                    string multiLevelClass = evaluationMatrix[key].ToString();
                    string[] a = multiLevelClass.Split(',');

                    int classPredect = 0;
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (a[i] == "0")
                        {
                            classPredect++;
                        }
                    }
                    if (classPredect == 0)
                    {
                        correnctlyClassified++;
                    }
                    else if (classPredect > 0)
                    {
                        incorrenctlyClassified++;
                    }
                }

                performance = (double)((double)correnctlyClassified / (double)totalData) * 100;
                System.Console.WriteLine(performance);
                #endregion
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
            }
            return performance;
        }
        
        public static double classifyCrossFold_Train_Test(string classifierFileName, int baseClasses, Classifier _classifier)
        {
            double performance = 0.0;
            try
            {
                List<BrResult> results = new List<BrResult>();
                for (int singleClass = 1; singleClass <= baseClasses; singleClass++)
                {
                    string eachFileName = String.Format("{0}_{1}.arff", classifierFileName, singleClass);

                    BrResult result = new BrResult();
                    result.classNumber = singleClass;

                    FileReader javaFileReader = new FileReader(eachFileName);
                    weka.core.Instances insts = new weka.core.Instances(javaFileReader);
                    javaFileReader.close();

                    insts.setClassIndex(insts.numAttributes() - 1);


                    List<Result> eachResults = new List<Result>();

                    var totalnstances = insts.numInstances();
                    var foldsInstances = totalnstances / 10;
                    Instances foldsData = new Instances(insts);
                    var folds = 10;
                    int numCorrect = 0;
                    int dataIndex = 0;
                    for (int n = 0; n < folds; n++)
                    {
                        System.Console.WriteLine("Performing " + n + " folds");

                        Instances trainFold = foldsData.trainCV(folds, n);
                        var numnerOfTrainInst = trainFold.numInstances();

                        Instances testFold = foldsData.testCV(folds, n);
                        var numnerOfTestInst = testFold.numInstances();


                        _classifier.buildClassifier(trainFold);

                        //List<Result> eachResults = new List<Result>();
                        for (int test = 0; test < numnerOfTestInst; test++)
                        {
                            dataIndex++;
                            Result eachRow = new Result();
                            eachRow.lineIndex = 0;
                            weka.core.Instance currentInst = testFold.instance(test);

                            double predictClass = _classifier.classifyInstance(currentInst);
                            //double[] dist = _classifier.distributionForInstance(currentInst);

                            string actualClass = testFold.classAttribute().value((int)testFold.instance(test).classValue());
                            string predictedClass = testFold.classAttribute().value((int)predictClass);

                            //var abcd = _classifier.getClass();

                            if (predictedClass == actualClass)
                            {
                                eachRow.correct = "1";
                                numCorrect++;
                            }
                            else
                            {
                                eachRow.correct = "0";
                            }
                            eachRow.lineIndex = dataIndex;
                            eachRow.classActual = actualClass;
                            eachRow.classPredicted = predictedClass;

                            eachResults.Add(eachRow);
                        }
                    }
                    result.classResult = eachResults;
                    results.Add(result);
                    //System.Console.WriteLine(numCorrect + " out of " + testSize + " correct (" + (double)((double)numCorrect / (double)testSize * 100.0) + "%)");
                }

                #region Evaludation Matrix
                var evaluationMatrix = new Dictionary<int, string>();

                foreach (var res in results)
                {
                    foreach (var classRes in res.classResult)
                    {
                        if (!evaluationMatrix.Keys.Contains(classRes.lineIndex))
                        {
                            evaluationMatrix[classRes.lineIndex] = classRes.correct.toString();
                        }
                        else
                        {
                            evaluationMatrix[classRes.lineIndex] = evaluationMatrix[classRes.lineIndex].toString() + "," + classRes.correct.toString();
                        }
                    }
                }
                #endregion

                #region
                int correnctlyClassified = 0;
                int incorrenctlyClassified = 0;
                int totalData = evaluationMatrix.Count;
                foreach (var key in evaluationMatrix.Keys)
                {
                    string multiLevelClass = evaluationMatrix[key].ToString();
                    string[] a = multiLevelClass.Split(',');

                    int classPredect = 0;
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (a[i] == "0")
                        {
                            classPredect++;
                        }
                    }
                    if (classPredect == 0)
                    {
                        correnctlyClassified++;
                    }
                    else if (classPredect > 0)
                    {
                        incorrenctlyClassified++;
                    }
                }

                performance = (double)((double)correnctlyClassified / (double)totalData) * 100;
                System.Console.WriteLine(performance);
                #endregion
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
            }
            return performance;
        }
        
        public static double classifyCrossFold_Train_Test_onlySelectedClass(string classifierFileName, int baseClasses, Classifier _classifier)
        {
            double performance = 0.0;
            try
            {
                List<BrResult> results = new List<BrResult>();
                for (int singleClass = 1; singleClass <= baseClasses; singleClass++)
                {
                    string eachFileName = String.Format("{0}_{1}.arff", classifierFileName, singleClass);

                    BrResult result = new BrResult();
                    result.classNumber = singleClass;

                    FileReader javaFileReader = new FileReader(eachFileName);
                    weka.core.Instances insts = new weka.core.Instances(javaFileReader);
                    javaFileReader.close();

                    insts.setClassIndex(insts.numAttributes() - 1);


                    List<Result> eachResults = new List<Result>();

                    var totalnstances = insts.numInstances();
                    var foldsInstances = totalnstances / 10;
                    Instances foldsData = new Instances(insts);
                    var folds = 10;
                    int numCorrect = 0;
                    int dataIndex = 0;
                    for (int n = 0; n < folds; n++)
                    {
                        System.Console.WriteLine("Performing " + n + " folds");

                        Instances trainFold = foldsData.trainCV(folds, n);
                        var numnerOfTrainInst = trainFold.numInstances();

                        Instances testFold = foldsData.testCV(folds, n);
                        var numnerOfTestInst = testFold.numInstances();


                        _classifier.buildClassifier(trainFold);

                        //List<Result> eachResults = new List<Result>();
                        for (int test = 0; test < numnerOfTestInst; test++)
                        {
                            dataIndex++;
                            Result eachRow = new Result();
                            eachRow.lineIndex = 0;
                            weka.core.Instance currentInst = testFold.instance(test);

                            double predictClass = _classifier.classifyInstance(currentInst);
                            //double[] dist = _classifier.distributionForInstance(currentInst);

                            string actualClass = testFold.classAttribute().value((int)testFold.instance(test).classValue());
                            string predictedClass = testFold.classAttribute().value((int)predictClass);

                            //var abcd = _classifier.getClass();

                            if (predictedClass == actualClass)
                            {
                                eachRow.correct = "1";
                                numCorrect++;
                            }
                            else
                            {
                                eachRow.correct = "0";
                            }
                            eachRow.lineIndex = dataIndex;
                            eachRow.classActual = actualClass;
                            eachRow.classPredicted = predictedClass;

                            eachResults.Add(eachRow);
                        }
                    }
                    result.classResult = eachResults;
                    results.Add(result);
                    //System.Console.WriteLine(numCorrect + " out of " + testSize + " correct (" + (double)((double)numCorrect / (double)testSize * 100.0) + "%)");
                }

                #region Evaludation Matrix
                var evaluationMatrix = new Dictionary<int, string>();

                foreach (var res in results)
                {
                    foreach (var classRes in res.classResult)
                    {
                        if (!evaluationMatrix.Keys.Contains(classRes.lineIndex))
                        {
                            evaluationMatrix[classRes.lineIndex] = classRes.correct.toString();
                        }
                        else
                        {
                            evaluationMatrix[classRes.lineIndex] = evaluationMatrix[classRes.lineIndex].toString() + "," + classRes.correct.toString();
                        }
                    }
                }
                #endregion

                #region
                int correnctlyClassified = 0;
                int incorrenctlyClassified = 0;
                int totalData = evaluationMatrix.Count;
                foreach (var key in evaluationMatrix.Keys)
                {
                    string multiLevelClass = evaluationMatrix[key].ToString();
                    string[] a = multiLevelClass.Split(',');

                    int classPredect = 0;
                    for (int i = 0; i < a.Length; i++)
                    {
                        if (a[i] == "0")
                        {
                            classPredect++;
                        }
                    }
                    if (classPredect == 0)
                    {
                        correnctlyClassified++;
                    }
                    else if (classPredect > 0)
                    {
                        incorrenctlyClassified++;
                    }
                }

                performance = (double)((double)correnctlyClassified / (double)totalData) * 100;
                System.Console.WriteLine(performance);
                #endregion
            }
            catch (java.lang.Exception ex)
            {
                ex.printStackTrace();
            }
            return performance;
        }

        public static void Test_predictClass(string classifierFileName)
        {

            FileReader javaFileReader = new FileReader(classifierFileName);
            weka.core.Instances insts = new weka.core.Instances(javaFileReader);
            javaFileReader.close();

            insts.setClassIndex(insts.numAttributes() - 1);

            weka.classifiers.Classifier cl = new weka.classifiers.trees.J48();
            System.Console.WriteLine("Performing " + percentSplit + "% split evaluation.");



            #region Manual Cross Fold
            Instances foldsData = new Instances(insts);
            int folds = 10;
            for (int n = 0; n < folds; n++)
            {
                Instances trainFold = foldsData.trainCV(folds, n);
                Instances testFold = foldsData.testCV(folds, n);


            }
            #endregion




            #region
            int trainSize = insts.numInstances() * percentSplit / 100;
            int testSize = insts.numInstances() - trainSize;
            weka.core.Instances train = new weka.core.Instances(insts, 0, trainSize);

            cl.buildClassifier(train);
            #endregion

            //Classifier cls = new J48();
            Evaluation eval = new Evaluation(insts);
            java.util.Random rand = new java.util.Random(1);  // using seed = 1
            int fold = 10;
            eval.crossValidateModel(cl, insts, fold, rand);
            System.Console.WriteLine("toClassDetailsString" + eval.toClassDetailsString());
            System.Console.WriteLine("toMatrixString\n" + eval.toMatrixString());
            System.Console.WriteLine("toCumulativeMarginDistributionString\n" + eval.toCumulativeMarginDistributionString());
            //System.Console.WriteLine("predictions\n" + eval.predictions());
            System.Console.ReadKey();

            //var numnerOfInst = insts.numInstances();

            //for (int i = trainSize; i < numnerOfInst; i++)
            //{
            //    weka.core.Instance currentInst = insts.instance(i);

            //    double pred = cl.classifyInstance(currentInst);
            //    System.Console.WriteLine("class Index: " + insts.instance(i).classIndex());
            //    System.Console.WriteLine(", class value: " + insts.instance(i).classValue());
            //    System.Console.WriteLine(", ID: " + insts.instance(i).value(0));
            //    System.Console.WriteLine(", actual: " + insts.classAttribute().value((int)insts.instance(i).classValue()));
            //    System.Console.WriteLine(", predicted: " + insts.classAttribute().value((int)pred));

            //}

        }
    }
}
