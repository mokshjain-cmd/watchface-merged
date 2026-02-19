using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WatchBasic.Tool
{
    public class NumTool
    {
        public static bool IsInt(string code)
        {
            Regex rg = new Regex(@"^[0-9]*$");
            return rg.IsMatch(code);
        }
    }
}
