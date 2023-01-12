using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Solution
    {
        public List<Move> Moves = new List<Move>();



        public static Solution Parse(string[] lines)
        {
            var result = new Solution();
            foreach (var line in lines)
            {
                var trimmedline = line.Trim();
                if (!string.IsNullOrEmpty(trimmedline))
                {
                    var move = Move.Parse(trimmedline);
                    result.Moves.Add(move);
                }
            }
            return result;
        }
    }
}
