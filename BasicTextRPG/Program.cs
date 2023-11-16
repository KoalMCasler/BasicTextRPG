using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.SqlServer.Server;
using System.CodeDom;

namespace BasicTextRPG
{
    internal class Program
    {
        // Map Variables
        static string path;
        static string path1 = @"Floor1Map.txt";
        static string path2 = @"Floor2Map.txt";
        static string path3 = @"Floor3Map.txt";
        static char border = ((char)4);
        static char dungeonFloor = ((char)18);
        static char dungeonWall = ((char)35);
        static char spikeTrap = ((char)23);
        static char player = ((char)167);
        static char stairsDown = ((char)30);
        static char stairsUp = ((char)31);
        static char finalLoot = ((char)165);
        static char coin = ((char)164);
        static char enemy1 = ((char)4);
        static char enemy2 = ((char)6);
        static string[] floorMap;
        static int borderLength;
        static int levelNumber;
        //Player variables
        static bool gameIsOver;
        static int basePlayerHP;
        static int playerDamage;
        static int playerCoins;
        static ConsoleKeyInfo input;
        //Enemy Variables
        static int baseEnemyHP;
        static int enemyDamage;

        static void Main()
        {
            StartUp();
            DrawMap();
            Console.ReadKey(true);
            levelNumber = 2;
            ChangeLevels();
            DrawMap();
            Console.ReadKey(true);
            levelNumber = 3;
            ChangeLevels();
            DrawMap();
            Console.ReadKey(true);
            levelNumber = 0;
            ChangeLevels();
        }
        static void StartUp()
        {
            //Sets up starting state of the game.
            path = path1;
            basePlayerHP = 10;
            baseEnemyHP = 6;
            levelNumber = 1;
            floorMap = File.ReadAllLines(path);
            borderLength = floorMap[0].Length + 2;
            gameIsOver = false;
        }
        static void DrawMap()
        {
            //Draws the map of the current level
            Console.Clear();
            for(int i = 0; i < borderLength; i++)
            {
                DrawBorder();
            }
            Console.Write("\n");
            for(int y = 0; y < floorMap.Length; y++)
            {
                string mapRow = floorMap[y];
                DrawBorder();
                for(int x = 0; x < mapRow.Length; x++)
                {
                    char tile = mapRow[x];
                    DrawTile(tile);
                }
                DrawBorder();
                Console.Write("\n");
            }
            for(int i = 0; i < borderLength; i++)
            {
                DrawBorder();
            }
            WriteLegend();
            DrawHUD(); // Just here to test function
        }
        static void WriteLegend()
        {
            // Write out the legend for the map
            Console.Write("\n");
            Console.Write("Floor = ");
            DrawFloor();
            Console.Write("              ");
            Console.Write("Walls = ");
            DrawWall();
            Console.Write("\n");
            Console.Write("Border = ");
            DrawBorder();
            Console.Write("             ");
            Console.Write("Player = ");
            DrawPlayer();
            Console.Write("\n");
            Console.Write("The Grail = ");
            DrawFinalLoot();
            Console.Write("          ");
            Console.Write("Coin = ");
            DrawCoin();
            Console.Write("\n");
            Console.Write("Enemy 1 = ");
            DrawEnemy(1);
            Console.Write("            ");
            Console.Write("Enemy 2 = ");
            DrawEnemy(2);
            Console.Write("\n");
        }
        static void DrawHUD()
        {
            Console.Write(string.Format("HP:{0}  Damage:{1} Coins:{2}",basePlayerHP, playerDamage, playerCoins));
        }
        static void DrawFloor()
        {
            // used to draw a floor tile
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write(dungeonFloor);
            SetColorDefault();
        }
        static void DrawWall()
        {
            // used to draw a wall tile
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.BackgroundColor = ConsoleColor.DarkGray;
            Console.Write(dungeonWall);
            SetColorDefault();
        }
        static void DrawSpikes()
        {
            // used to draw a spikes tile
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write(spikeTrap);
            SetColorDefault();
        }
        static void DrawFinalLoot()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write(finalLoot);
            SetColorDefault();
        }
        static void DrawCoin()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write(coin);
            SetColorDefault();
        }
        static void SetColorDefault()
        {
            // sets console color back to default. 
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
        static void DrawBorder()
        {
            // used to draw border tiles
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(border);
            SetColorDefault();
        }
        static void DrawStairsDown()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write(stairsDown);
            SetColorDefault();
        }
        static void DrawStairsUp()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write(stairsUp);
            SetColorDefault();
        }
        static void DrawPlayer()
        {
            // used to draw the player
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write(player);
            SetColorDefault();
        }
        static void DrawEnemy(int enemyNumber)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            if(enemyNumber > 2 || enemyNumber < 1)
            {
                enemyNumber = 1;
            }
            if(enemyNumber == 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(enemy1);
            }
            if(enemyNumber == 2)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(enemy2);
            }
            SetColorDefault();
        }
        static void DrawTile(Char tile)
        {
            // draws the correct tile based on the floorMap
            if(tile == '-')
            {
                DrawFloor();
                return;
            }
            if(tile == dungeonWall)
            {
                DrawWall();
                return;
            }
            if(tile == '*')
            {
                DrawSpikes();
                return;
            }
            if(tile == '~')
            {
                DrawStairsDown();
                return;
            }
            if(tile == '=')
            {
                DrawStairsUp();
                return;
            }
            if(tile == '$')
            {
                DrawFinalLoot();
                return;
            }
            else
            {
                Console.Write(tile);
            }
        }
        static void ChangeLevels()
        {
            // used to change maps
            if(levelNumber == 1)
            {
                path = path1;
                floorMap = File.ReadAllLines(path);
            }
            if(levelNumber == 2)
            {
                levelNumber = 2;
                path = path2;
                floorMap = File.ReadAllLines(path);
            }
            if(levelNumber == 3)
            {
                levelNumber = 3;
                path = path3;
                floorMap = File.ReadAllLines(path);
            }
            if(levelNumber > 3 || levelNumber <= 0)
            {
                Console.Clear();
                Console.WriteLine("Level Out of range, closing");
                Console.ReadKey(true);
                Environment.Exit(0);
            }
        }
    }
}
