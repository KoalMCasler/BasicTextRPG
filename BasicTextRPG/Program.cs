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
        static char[,] dungeonMap;
        static int levelNumber;
        static bool levelChanged;
        static int playerMaxX;
        static int playerMaxY;  
        static int mapX;
        static int mapY;      
        //Player variables
        static bool gameIsOver;
        static int basePlayerHP;
        static int playerDamage;
        static int playerCoins;
        static int playerX;
        static int playerY;
        static ConsoleKeyInfo playerInput;
        //Enemy Variables
        static int baseEnemyHP = 5;
        static int enemyDamage = 1;
        static int enemyCount;

        static void Main()
        {
            StartUp();
            Intro();
            Console.Clear();
            while(gameIsOver != true)
            {
                DrawMap();
                GetInput();
                DrawPlayer();
            }
        }
        static void StartUp()
        {
            //Sets up starting state of the game.
            basePlayerHP = 10;
            baseEnemyHP = 6;
            levelNumber = 1;
            playerDamage = 1;
            playerCoins = 0;
            enemyCount = 0;
            path = path1;
            floorMap = File.ReadAllLines(path);
            dungeonMap = new char[floorMap.Length, floorMap[0].Length];
            MakeDungeonMap();
            mapX = dungeonMap.GetLength(1);
            mapY = dungeonMap.GetLength(0);
            gameIsOver = false;
            levelChanged = false;
        }
        static void DrawMap()
        {
            //Draws the map of the current level
            Console.SetCursorPosition(0,0);
            if(levelChanged == true)
            {
                enemyCount = 0;
            }
            for(int y = 0; y < mapY; y++)
            {
                for(int x = 0; x < mapX; x++)
                {
                    char tile = dungeonMap[y,x];
                    DrawTile(tile);
                    if(tile == '=' && levelChanged == false)
                    {
                        playerX = x;
                        playerY = y-1;
                        levelChanged = true;
                    }
                    if(tile == '!' || tile == '?')
                    {
                        enemyCount += 1;
                    }
                }
                Console.Write("\n");
            }
            WriteLegend();
            DrawHUD(); // Just here to test function
            SetPLayerPosition();
            DrawPlayer();
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
            Console.WriteLine(string.Format("HP:{0}  Damage:{1} Coins:{2}",basePlayerHP, playerDamage, playerCoins));
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
        static void SetPLayerPosition()
        {
            Console.SetCursorPosition(playerX,playerY);
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
            if(tile == '#')
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
            if(tile == '@')
            {
                DrawCoin();
                return;
            }
            if(tile == '!')
            {
                DrawEnemy(1);
                return;
            }
            if(tile == '?')
            {
                DrawEnemy(2);
                return;
            }
            if(tile == '+')
            {
                DrawPlayer();
                return;
            }
            else
            {
                Console.Write(tile);
            }
        }
        static void ChangeLevels()
        {
            levelChanged = false;
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
                Console.WriteLine("Level Out of range, Loading level 1");
                path = path1;
                floorMap = File.ReadAllLines(path);
            }
        }
        static void GetInput()
        {
            int moveX;
            int moveY;
            bool playerMoved;
            playerMoved = false;
            playerInput = Console.ReadKey(true);
            //Console.WriteLine(playerInput.Key); //debug to see what key is pressed
            if(playerMoved == false)
            {
                if(playerInput.Key == ConsoleKey.W || playerInput.Key == ConsoleKey.UpArrow)
                {
                    //Moves player up
                    moveY = (playerY - 1);
                    if(moveY <= 0)
                    {
                        moveY = 0; //Locks top of screen
                    }
                    if(dungeonMap[moveY,playerX] == '#')
                    {
                        moveY = playerY;
                        playerY = moveY;
                        return;
                    }
                    else
                    {
                        //if(dungeonMap[moveY,playerX] == '-')
                        //{
                            playerMoved = true;
                            playerY = moveY;
                            if(playerY <= 0)
                            {
                                playerY = 0;
                            }
                        //}
                    }


                }if(playerInput.Key == ConsoleKey.S || playerInput.Key == ConsoleKey.DownArrow)
                {
                    //Moves player down

                    if(dungeonMap[playerY,playerX] == '#')
                    {return;}

                }if(playerInput.Key == ConsoleKey.A || playerInput.Key == ConsoleKey.LeftArrow)
                {
                    //Moves player left
                    if(dungeonMap[playerY,playerX] == '#')
                    {return;}
                    else
                    {

                    }
                }if(playerInput.Key == ConsoleKey.D || playerInput.Key == ConsoleKey.RightArrow)
                {
                    //Moves player right
                    if(dungeonMap[playerY,playerX] == '#')
                    {return;}
                    else
                    {

                    }
                }
                if(playerInput.Key == ConsoleKey.Escape)
                {
                Environment.Exit(0);
                }
            }
        }
        static void MakeDungeonMap()
        {
            for (int i = 0; i < floorMap.Length; i++)
            {
                for (int j = 0; j < floorMap[i].Length; j++)
                {
                 dungeonMap[i, j] = floorMap[i][j];
                }
            }
        }
        static void Intro()
        {
            Console.WriteLine("Kill all they enemies on each floor to get to the next");
            Console.WriteLine("Collect as much gold as you can along the way");
            Console.WriteLine("Press any key to get stated, Escape will exit once in game.");
            Console.ReadKey(true);
        }
    }
}
