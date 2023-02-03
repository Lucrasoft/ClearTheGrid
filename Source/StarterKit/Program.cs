using Shared;
using System.Diagnostics;

namespace StarterKit
{
    /// <summary>
    /// A very slowww, inefficient, starterkit solution to solve the clear te grid levels.
    /// Provided as a starting point, initial direction. Get's you to level +/- 24 ?!
    /// </summary>
    internal class Program
    {

        static Random rnd = new Random();

        /// <summary>
        /// An empty and invalid move, we use it as template to force a 'random' move.
        /// </summary>
        static Move emptyMove = new Move(0, 0, Direction.U, false);


        static void Main(string[] args)
        {

            //You might want to change this your own level path? :-)
            string LevelPath = "../../../../../Levels/";

            for (int level = 1; level < 100; level++)
            {

                Console.WriteLine($"Loading level {level}");

                //load a level map
                var currentMap = LoadLevel(Path.Join(LevelPath, "0XX",$"{level}.txt"));

                //The basic idea of the Starterkit is (but other idea's exists!) :

                //- You need a solution => A solution is a set of moves that clears the board.
                //- Start with a completly random solution = set of random moves
                //- A random move means => just pick an available move of the current board.
                //- Apply the moves till you're stuck.
                //- Score the board (multiple idea's exists!) 
                //- If the score is better then previous -> keep the moves!
                //- Otherwise -> change something in the moves = mutate the moves.
                //            -> for example : delete a random move while keeping the others.
                //            -> multiple idea's exists! 

                //The approach above is called HillClimbing and it has known problems on its own.
                //Google is your friend to overcome these HillClimb problems :-)

                //TIPS to improve
                //- Write MUCH faster code!! You need a LOT of simulations to complete higher levels.
                //- TIP : Use a profiler.
                //- Find a solution for the HillClimb-Is-Stuck problem (called : a local optimum instead of global optimum) 
                //- Find more "mutate" methods to make more variants of the moves
                //- Find more score functions
                //- Find heuristics to further guide the search process
                //- Monitor, watch, look, debug, log, to find other insights.
                //- As a last resort : Try to scale your solution..



                //Maximum number of moves on a map are at most the number of cells.
                var moves = new List<Move>();
                for (int i = 0; i < currentMap.w * currentMap.h ; i++)
                {
                    moves.Add(emptyMove);
                }


                //We keep track of the 'best score sofar' and look for solutions which have a lower score.
                int bestScore = 999999;

                
                var isDone = false;
                while (!isDone)
                {
                    //Play out the current moves on the map.
                    var (newMap, actualmoves) = PlayMovesOnMap(currentMap, moves);

                    //How good are the moves = how good is the new map = give it a score!
                    int newScore = GetMapScore(newMap);
                    if (newScore < bestScore)
                    {
                        Console.WriteLine($"New highscore : {newScore}");
                        bestScore = newScore;
                        //Yeah, new highscore, we keep the moves!! 
                        for (int i = 0; i < actualmoves.Count; i++)
                        {
                            moves[i] = actualmoves[i];
                        }
                    }

                    if (newScore == 0)
                    {
                        //we have found a solution!
                        Console.WriteLine("Solution found!");
                        isDone = true;
                    }
                    else
                    {
                        //We change a little thing in the moves and try again!
                        //But more type of changes can help!
                        moves[rnd.Next(moves.Count)] = emptyMove;
                    }
                }
            }
        }


        /// <summary>
        /// Some map score, but others exist.. test them all :-)
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        static int GetMapScore(Map map)
        {
            int result = 0;
            //we count the sum of the values left on the map..
            for (int x = 0; x < map.w; x++)
            {
                for (int y = 0; y < map.h; y++)
                {
                    if (map.data[x, y]>0) result++;
                }
            }
            return result;
        }


        /// <summary>
        /// Execute the provided moves on the map. 
        /// Pick a random move if the provided move is not valid.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="moves"></param>
        /// <returns></returns>
        static (Map finalMap, List<Move> actualMoves) PlayMovesOnMap(Map m , List<Move> moves)
        {
            //Collect the moves we are actually making this playout.
            var actualMoves = new List<Move>();

            //We don't wanna mess up the original map.. 
            var localMap = CloneMap(m);

            //try to "play" the provided moves, are move 
            foreach (var move in moves)
            {
                Move actualMove = move;

                //make sure this move is valid, otherwise, pick a random valid move..
                if (!IsValidMove(localMap, actualMove))
                {
                    //Oops, the move is not valid. Lets find a random move/
                    var validMoves = GetValidMoves(localMap);
                    if (validMoves.Count == 0)
                    {
                        //Surprise. It seems we are done! No more valid moves are found..
                        break;
                    }
                    actualMove = validMoves[rnd.Next(validMoves.Count)];
                } 

                //Execute the move on our map
                localMap.ApplyMove(actualMove);
                
                //Save a cloned version of the actualMove in our list. 
                actualMoves.Add(actualMove.Clone());
            }
            return (localMap, actualMoves);
        }


        /// <summary>
        /// Get Valid Moves based on the provided Map
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        static List<Move> GetValidMoves(Map map)
        {
            var result = new List<Move>();
            //Let's try all positions, all directions and all add/subtract options. 
            //And send back the valid moves.
            for (int x = 0; x < map.w; x++)
            {
                for (int y = 0; y < map.h; y++)
                {
                    for (int d = 0; d < 4; d++)
                    {
                        for (int s = 0; s < 2; s++)
                        {
                            var move = new Move(x, y, (Direction)d, s == 0);
                            if (IsValidMove(map, move))
                            {
                                result.Add(move);
                            }
                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Check if the Move is a valid move on the map.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        static bool IsValidMove(Map map, Move move)
        {
            int sourcevalue = map.data[move.x, move.y];
            if (sourcevalue == 0) return false;

            var dircoords = move.dir.AsCoords();
            int dx = move.x + dircoords.xstep * sourcevalue;
            int dy = move.y + dircoords.ystep * sourcevalue;

            if (dx >= 0 && dy >= 0 && dx < map.w && dy < map.h)
            {
                int destvalue = map.data[dx, dy];
                if (destvalue == 0) return false;
                return true;
            }
            return false;
        }




        static Map CloneMap(Map source)
        {
            var result = new Map(source.w, source.h);
            result.data = (int[,])source.data.Clone();
            return result;
        }



        static Map LoadLevel(string filename)
        {
            var levelLines = File.ReadAllLines(filename);
            Map m = Map.Parse(levelLines);
            return m;
        }


    }
}