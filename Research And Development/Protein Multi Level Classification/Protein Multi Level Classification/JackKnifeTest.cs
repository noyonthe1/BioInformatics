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
    class JackKnifeTest
    {
        public static void JackKnife_Test_prepare(string classifierFileName, int baseClasses, Classifier _classifie)
        {
            for (int singleClass = 1; singleClass <= baseClasses; singleClass++)
            {
                string eachFileName = String.Format("{0}_{1}.arff", classifierFileName, singleClass);

                FileReader javaFileReader = new FileReader(eachFileName);
                weka.core.Instances insts = new weka.core.Instances(javaFileReader);
                javaFileReader.close();

                insts.setClassIndex(insts.numAttributes() - 1);

                var totalnstances = insts.numInstances();

                //insts.re
            }
        }
    }
}
