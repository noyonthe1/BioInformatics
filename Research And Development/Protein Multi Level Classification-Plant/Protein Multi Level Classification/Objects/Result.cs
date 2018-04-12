using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protein_Multi_Level_Classification
{
    public class Result
    {
        public string correct { get; set; }
        public int lineIndex { get; set; }
        public string classPredicted { get; set; }
        public string classActual { get; set; }
    }
}
