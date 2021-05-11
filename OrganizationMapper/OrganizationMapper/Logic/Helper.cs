using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrganizationMapper.Logic
{
    public class Helper
    {
        static readonly Regex csvSplit = new Regex(";(?=(?:[^\"\"]*\"\"[^\"\"]*\"\")*[^\"\"]*$)");

        public Helper()
        {

        }

        public string[] SplitCsvString(string input)
        {
            return csvSplit.Split(input.Trim('"'));
        }
    }
}
