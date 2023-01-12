using System.Security.Cryptography;
using System.Text;

namespace Shared
{

    /// <summary>
    /// Generate, import, export, validate maps for the ClearTheGrid 
    /// </summary>
    public class Map
    {

        private static int[] dx = { 0, 1, 0, -1 };
        private static int[] dy = { 1, 0, -1, 0 };
        //private static String[] dirs = { "D", "R", "U", "L" };

        public int w;
        public int h;
        public int[,] data;
        
        public Map(int w, int h)
        {
            this.w = w;
            this.h= h;
            this.data= new int[w, h];   
        }

        public static Map Parse(string[] lines)
        {
            var parts = lines[0].Split(" ");
            int w = int.Parse(parts[0]);
            int h = int.Parse(parts[1]);
            Map map = new Map(w, h);

            //read other lines
            for (int y = 0; y < h; y++)
            {
                var line = lines[y + 1];
                var lineparts = line.Split(" ");
                for (int x = 0; x < w; x++)
                {
                    map.data[x, y] = int.Parse(lineparts[x]);
                }
            }
            return map;
        }


        /// <summary>
        /// Generate a map based on the level (difficulty).
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static Map CreateMap(int level)
        {

            //determine size and complexity on level.
            int spawns = 3 + level / 2;
            if (level > 150) spawns = 3 + level - 75;

            int height = 5;
            int width = height * 16 / 9;
            while (width * height < spawns * 2)
            {
                spawns -= 2;
                height++;
                width = height * 16 / 9;
            }

            Map map = new Map(width, height);

            for (int i = 0; i < spawns; i++)
            {

                if (i == 0 || RandomNumberGenerator.GetInt32(5) == 0)
                {
                    // find pair of empty cells
                    while (true)
                    {
                        int x1 = RandomNumberGenerator.GetInt32(width);
                        int y1 = RandomNumberGenerator.GetInt32(height);
                        int dir = RandomNumberGenerator.GetInt32(4);
                        int length = 1 + RandomNumberGenerator.GetInt32(width);
                        int x2 = x1 - length * dx[dir];
                        int y2 = y1 - length * dy[dir];
                        if (x2 >= 0 && x2 < width && y2 >= 0 && y2 < height && map.data[x1, y1] == 0 && map.data[x2, y2] == 0)
                        {
                            map.data[x1, y1] = length;
                            map.data[x2, y2] = length;
                            break;
                        }
                    }
                }
                else
                {
                    // split number
                    while (true)
                    {
                        int x1 = RandomNumberGenerator.GetInt32(width);
                        int y1 = RandomNumberGenerator.GetInt32(height);
                        int dir = RandomNumberGenerator.GetInt32(4);
                        int length = 1 + RandomNumberGenerator.GetInt32(width);
                        int x2 = x1 - length * dx[dir];
                        int y2 = y1 - length * dy[dir];
                        bool add = (RandomNumberGenerator.GetInt32(2) == 0);
                        if (x2 >= 0 && x2 < width && y2 >= 0 && y2 < height &&
                            map.data[x1, y1] != 0 && map.data[x1, y1] != length &&
                            map.data[x2, y2] == 0)
                        {
                            map.data[x2, y2] = length;
                            if (add) { map.data[x1, y1] -= length; } else { map.data[x1, y1] += length; };
                            if (map.data[x1, y1] < 0)
                            {
                                map.data[x1, y1] = -map.data[x1, y1];
                            }
                            break;
                        }
                    }
                }
            }
            return map;
        }


        public string ApplyMove(Move move)
        {
            int sourcevalue = data[move.x, move.y];
            if (sourcevalue == 0)
            {
                return $"The move {move} is invalid. The source cell {move.x},{move.y} is empty.";
            }

            var dircoords = move.dir.AsCoords();
            int dx = move.x + dircoords.xstep * sourcevalue;
            int dy = move.y + dircoords.ystep * sourcevalue;

            if (dx >=0 && dy>=0 && dx<w && dy<h)
            {
                int destvalue = data[dx, dy];
                if (destvalue == 0)
                {
                    return $"The move {move} is invalid. The destination cell {move.x},{move.y} is empty.";
                }
                //all set.. 
                data[move.x, move.y] = 0;
                if (move.add)
                {
                    data[dx, dy] += sourcevalue;
                }
                else
                {
                    data[dx, dy] -= sourcevalue;
                    if (data[dx, dy]<0) { data[dx, dy] *= -1; }
                }
                return "";
            }
            else
            {
                //outside range?!
                return $"The move {move} is invalid. The destination cell {dx},{dy} is outside the grid boundaries.";
            }

        }


        /// <summary>
        /// Return true if the map is empty.
        /// </summary>
        /// <returns></returns>
        public bool IsSolved() 
        {
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (data[x, y] != 0) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Generate a string based export of the map
        /// </summary>
        /// <returns></returns>
        public string ExportMap()
        {
            var result = new StringBuilder();
            result.AppendLine($"{w} {h}");

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    result.Append($"{data[x, y]} ");
                }
                result.AppendLine();
            }

            return result.ToString();
        }

        /// <summary>
        /// Compare this instance to another map instance.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsEqual(Map other)
        {
            if (other == null) return false;
            if (other.w != w) return false;
            if (other.h != h) return false;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (other.data[x, y] != data[x, y]) return false;
                }
            }
            return true;
        }
    }
}