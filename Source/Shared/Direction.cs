using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{

    public enum Direction
    {
        D = 0,
        R = 1,
        U = 2,
        L = 3
    }

    static class DirectionMethods
    {
        public static (int xstep,int ystep) AsCoords(this Direction dir)
        {
            switch (dir)
            {
                case Direction.D: return (0, 1);
                case Direction.R: return (1, 0);
                case Direction.U: return (0, -1);
                case Direction.L: return (-1, -0);
                default:
                    return (0, 0);
            }
        }
    }

}
