using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protein_Multi_Level_Classification
{
    class Utils
    {
        public static List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }

        public static string currentDirectorypath()
        {
            string currentDirectory = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));
            currentDirectory = currentDirectory.TrimEnd('\\');
            currentDirectory = currentDirectory.Remove(currentDirectory.LastIndexOf('\\') + 1);

            string BaseFilePath = Path.Combine(currentDirectory, "Files\\");

            return BaseFilePath;
        }

        //public static IEnumerable<TResult> Zip<T, TResult>(this IEnumerable<T>[] sequences, Func<T[], TResult> resultSelector)
        //{
        //    var enumerators = sequences.Select(s => s.GetEnumerator()).ToArray();
        //    while (enumerators.All(e => e.MoveNext()))
        //        yield return resultSelector(enumerators.Select(e => e.Current).ToArray());
        //}

        //public static void Shuffle<T>(this IList<T> list, Random rnd)
        //{
        //    for (var i = 0; i < list.Count; i++)
        //        list.Swap(i, rnd.Next(i, list.Count));
        //}

        //public static void Swap<T>(this IList<T> list, int i, int j)
        //{
        //    var temp = list[i];
        //    list[i] = list[j];
        //    list[j] = temp;
        //}
    }
}
