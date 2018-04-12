using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protein_Multi_Level_Classification
{
    class Program
    {
        static void Main(string[] args)
        {

            BinaryRelAnalysis.BRRandomizeArffFile();

            string currentDirectory = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            currentDirectory = currentDirectory.TrimEnd('\\');
            currentDirectory = currentDirectory.Remove(currentDirectory.LastIndexOf('\\') + 1);

            string BaseFilePath = Path.Combine(currentDirectory, "Files\\");

            string today = DateTime.Now.ToString();

            string BR_resultFilePath = today + "_BR_finalresult.txt";

            string LP_resultFilePath = today + "_LP_finalresult.txt";

            //Weka.Test_predictClass(BaseFilePath + "BinaryRel_PSSMSS_1.arff");

            int numberOfEvaluation = 20;

            //BR(numberOfEvaluation, Path.Combine(BaseFilePath, Path.Combine(BaseFilePath, BR_resultFilePath)));

            //LP(numberOfEvaluation, Path.Combine(BaseFilePath, Path.Combine(BaseFilePath, LP_resultFilePath)));

            Pssm_File_Creation.pssmFilePreparation();


        }

        public static void LP(int evaluation, string LP_resultFilePath)
        {
            Dictionary<int, string> resultSet = new Dictionary<int, string>();

            for (int i = 0; i < evaluation; i++)
            {
                resultSet[i] = LabelPowersetAnalysis.labelPowersetData().ToString();
            }


            StringBuilder sb = new StringBuilder();

            foreach (var key in resultSet.Keys)
            {
                var data = key.ToString() + " time evaluation result \n " + resultSet[key] + "\n";
                sb.Append(data);
            }

            File_Helper.WriteToFile(sb, LP_resultFilePath);
            Console.ReadKey();
        }

        public static void BR(int evaluation, string BR_resultFilePath)
        {
            
            Dictionary<int, string> resultSet = new Dictionary<int, string>();

            for (int i = 0; i < evaluation; i++)
            {
                resultSet[i] = BinaryRelAnalysis.binaryRelevenceData().ToString();
            }


            StringBuilder sb = new StringBuilder();

            foreach (var key in resultSet.Keys)
            {
                var data = key.ToString() + " time evaluation result \n " + resultSet[key] + "\n";
                sb.Append(data);
            }

            File_Helper.WriteToFile(sb,BR_resultFilePath);
            Console.ReadKey();
        }
    }
}
