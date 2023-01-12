using Shared;

namespace Checker
{
    internal class Program
    {

        static string levelFilename = "";
        static string solutionFilename = "";

        static Map? map;
        static Solution? solution;

        static int Main(string[] args)
        {
            Console.WriteLine("Checker tool for ClearTheGrid.");

            //process the arguments,
            //- check for file existence 
            //- check correct parsing of the files
            var status = ProcessArg(args);
            if (status < 0) { return status; }

            //Execute the moves in the solutions file!
            foreach (var move in solution.Moves)
            {
                var moveResult = map.ApplyMove(move);
                if (moveResult != "")
                {
                    Console.WriteLine(moveResult);
                    return -3;
                }
            }

            //Check if the map is solved.
            if (!map.IsSolved())
            {
                Console.WriteLine("Solution was not correct. There are still non empty cells.");
                return -4;
            }

            //Yes! It is solved.
            Console.WriteLine("You solved it!");
            return 0;
        }


        static int ProcessArg(string[] args)
        {
            //Should be exactly 2 arguments .. <level> <solution>
            if (args.Length != 2)
            {
                Console.WriteLine("Usage : checker.exe <level-text-file> <solutions-text-file>");
                return -2;
            }

            //check the existence of the files.
            levelFilename = args[0];
            solutionFilename = args[1];

            if (!File.Exists(levelFilename))
            {
                Console.WriteLine($"Could not find the file : {levelFilename}");
                return -2;
            }
            if (!File.Exists(solutionFilename))
            {
                Console.WriteLine($"Could not find the file : {solutionFilename}");
                return -2;
            }

            //check the parsing of the level and solution file.
            map = TryLoadLevel(levelFilename);
            if (map == null)
            {
                Console.WriteLine($"Could not parse the level file: {levelFilename}");
                return -2;
            }
       

            solution= TryLoadSolution(solutionFilename);
            if (solution == null)
            {
                Console.WriteLine($"Could not parse the solution file: {solutionFilename}");
                return -2;
            }
 
            return 0;
        }


        static Map? TryLoadLevel(string filename)
        {
            try
            {
                var levelLines = File.ReadAllLines(filename);
                Map m = Map.Parse(levelLines);
                return m;
            }
            catch (Exception)
            {
            }
            return null;
        }

        static Solution? TryLoadSolution(string filename)
        {
            try
            {
                var solutionLines = File.ReadAllLines(filename);
                Solution s = Solution.Parse(solutionLines);
                return s;
            }
            catch (Exception)
            {
            }
            return null;
        }
    }

}