using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BasicTextRPG
{
    internal class Program
    {
        static string path;
        static string path1 = @"Floor1Map.txt";
        static string path2 = @"Floor2Map.txt";
        static string path3 = @"Floor3Map.txt";
        static int basePlayerHP;
        static int baseEnemyHP;
        static string[] floorMap;
        static int borderLength;
        static int levelNumber;
        static char border = ((char)4);
        static void Main()
        {
            StartUp();
            DrawMap();
            Console.ReadKey(true);
        }
        static void StartUp()
        {
            basePlayerHP = 10;
            baseEnemyHP = 6;
            floorMap = File.ReadAllLines(path1);
            borderLength = floorMap[0].Length + 2;
            levelNumber = 1;
        }
        static void DrawMap()
        {
            for(int i = 0; i < borderLength; i++)
            {
                Console.Write(border);
            }
            Console.Write("\n");
            for(int y = 0; y < floorMap.Length; y++)
            {
                string mapRow = floorMap[y];
                Console.Write(border);
                for(int x = 0; x < mapRow.Length; x++)
                {
                    char tile = mapRow[x];
                    Console.Write(tile);
                }
                Console.Write(border);
                Console.Write("\n");
            }
            for(int i = 0; i < borderLength; i++)
            {
                Console.Write(border);
            }
        }
        static void WriteLegend()
        {
            
        }
    }
}
