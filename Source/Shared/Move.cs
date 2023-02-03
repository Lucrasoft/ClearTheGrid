using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Move
    {

        public int x;
        public int y;
        public Direction dir;
        public bool add;

        public Move(int x, int y, Direction dir, bool add)
        {
            this.x = x;
            this.y = y;
            this.dir = dir;
            this.add = add;
        }

        public override string ToString()
        {
            var addstr = add ? "+" : "-";
            return $"{x} {y} {dir} {addstr}";
        }

        /// <summary>
        /// Parse a move line. The line should be : X Y DIRECTION SIGN
        /// Direction is a single character : U D L R (up down left right)
        /// Sign is either + or -
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Move Parse(string line)
        {
            line = line.Trim();
            var parts = line.Split(" ");
            var x = int.Parse(parts[0]);
            var y = int.Parse(parts[1]);
            var ds = Enum.Parse<Direction>(parts[2]);
            var sign = parts[3];

            return new Move(x, y, ds, sign == "+");

        }

        public Move Clone()
        {
            return new Move(this.x, this.y, this.dir, this.add);
        }
    }

  
}
