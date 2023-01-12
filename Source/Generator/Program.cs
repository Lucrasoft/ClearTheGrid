using System;
using Shared;

namespace Generator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Let's generate the 999 levels 

            for (int l = 1; l < 1000; l++)
            {
                var map = Map.CreateMap(l);
                var content = map.ExportMap();
                
                //Make sure it parses back the same map.
                var map2 = Map.Parse(content.Split(Environment.NewLine));
                if (!map2.IsEqual(map))
                {
                    throw new Exception("What?!?! This should not happen.");
                }

                //Store the level to disk.
                File.WriteAllText($"{l}.txt", content);

            }

        }
    }
}