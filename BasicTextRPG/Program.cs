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
        static int playerHP;
        static int playerDamage;
        static int playerCoins;
        static int playerX;
        static int playerY;
        static bool gameWon;
        static ConsoleKeyInfo playerInput;
        //Enemy Variables
        static int baseEnemyHP;
        static int enemy1HP;
        static int enemy2HP;
        static int enemyDamage;
        static int enemy1X;
        static int enemy1Y;
        static int enemy2X;
        static int enemy2Y;
        static bool enemy1IsActive;
        static bool enemy2IsActive;

        static void Main()
        {
            StartUp();
            Intro();
            Console.Clear();
            while(gameIsOver != true)
            {
                DrawMap();
                GetInput();
                MoveEnemy1();
                MoveEnemy2();
            }
            Console.Clear();
            if(gameWon == true)
            {
                Console.WriteLine(string.Format("Congratulations! You won with {0} coins!", playerCoins));
                Console.WriteLine("Press any key to close.");
                Console.ReadKey(true);
            }
            else
            {
                Console.WriteLine("You have DIED!");
                Console.WriteLine("Press any key to close.");
                Console.ReadKey(true);
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
            enemyDamage = 1;
            playerHP = basePlayerHP;
            SetEnemyHP();
            SetEnemyActive();
            path = path1;
            floorMap = File.ReadAllLines(path);
            dungeonMap = new char[floorMap.Length, floorMap[0].Length];
            MakeDungeonMap();
            mapX = dungeonMap.GetLength(1);
            mapY = dungeonMap.GetLength(0);
            playerMaxY = mapY -1;
            playerMaxX = mapX -1;
            levelNumber = 1;
            gameIsOver = false;
            levelChanged = false;
        }
        static void DrawMap()
        {
            //Draws the map of the current level
            Console.SetCursorPosition(0,0);
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
                    if(tile == '!' && levelChanged == false || tile == '?' && levelChanged == false)
                    {
                        if(tile == '?')
                        {
                            enemy1X = x;
                            enemy1Y = y;
                        }
                        if(tile == '!')
                        {
                            enemy2X = x;
                            enemy2Y = y;
                        }
                    }
                }
                Console.Write("\n");
            }
            WriteLegend();
            Console.Write("\n");
            DrawHUD();
            SetPLayerPosition();
            SetEnemyPosition(1);
            SetEnemyPosition(2);
            Console.SetCursorPosition(0,0);
            
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
            Console.Write("             ");
            Console.Write("Spikes = ");
            DrawSpikes();
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
            Console.WriteLine(string.Format("HP:{0}  Damage:{1}  Coins:{2}             ",playerHP, playerDamage, playerCoins));
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
            DrawPlayer();
        }
        static void SetEnemyPosition(int enemyNumber)
        {
            if(enemyNumber > 2 || enemyNumber < 1)
            {
                enemyNumber = 1;
            }
            if(enemyNumber == 1 && enemy1IsActive == true)
            {
                Console.SetCursorPosition(enemy1X,enemy1Y);
                DrawEnemy(enemyNumber);
            }
            if(enemyNumber == 2 && enemy2IsActive == true)
            {
                Console.SetCursorPosition(enemy2X,enemy2Y);
                DrawEnemy(enemyNumber);
            }
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
                DrawWall();
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
                DrawFloor();
                return;
            }
            if(tile == '?')
            {
                DrawFloor();
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
            SetEnemyHP();
            SetEnemyActive();
            MakeDungeonMap();
        }
        static void SetEnemyHP()
        {
            enemy1HP = baseEnemyHP/2*levelNumber;
            enemy2HP = baseEnemyHP/3*levelNumber;
        }
        static void GetInput()
        {
            SetPlayerDamage();
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
                    if(moveY == enemy1Y && playerX == enemy1X)
                    {
                        DoDamage(playerDamage, 1);
                        return;
                    }
                    if(moveY == enemy2Y && playerX == enemy2X)
                    {
                        DoDamage(playerDamage, 2);
                        return;
                    }
                    if(dungeonMap[moveY,playerX] == '#')
                    {
                        moveY = playerY;
                        playerY = moveY;
                        return;
                    }
                    else
                    {
                        playerMoved = true;
                        playerY = moveY;
                        if(playerY <= 0)
                        {
                            playerY = 0;
                        }
                    }
                }
                if(playerInput.Key == ConsoleKey.S || playerInput.Key == ConsoleKey.DownArrow)
                {
                    //Moves player down
                    moveY = (playerY + 1);
                    if(moveY >= playerMaxY)
                    {
                        moveY = playerMaxY; //Locks top of screen
                    }
                    if(moveY == enemy1Y && playerX == enemy1X)
                    {
                        DoDamage(playerDamage, 1);
                        return;
                    }
                    if(moveY == enemy2Y && playerX == enemy2X)
                    {
                        DoDamage(playerDamage, 2);
                        return;
                    }
                    if(dungeonMap[moveY,playerX] == '#')
                    {
                        moveY = playerY;
                        playerY = moveY;
                        return;
                    }
                    else
                    {
                        playerMoved = true;
                        playerY = moveY;
                        if(playerY >= playerMaxY)
                        {
                            playerY = playerMaxY;
                        }
                    }
                }
                if(playerInput.Key == ConsoleKey.A || playerInput.Key == ConsoleKey.LeftArrow)
                {
                    //Moves player left
                    moveX = (playerX - 1);
                    if(moveX <= 0)
                    {
                        moveX = 0; //Locks top of screen
                    }
                    if(moveX == enemy1X && playerY == enemy1Y)
                    {
                        DoDamage(playerDamage, 1);
                        return;
                    }
                    if(moveX == enemy2X && playerY == enemy2Y)
                    {
                        DoDamage(playerDamage, 2);
                        return;
                    }
                    if(dungeonMap[playerY,moveX] == '#')
                    {
                        moveX = playerX;
                        playerX = moveX;
                        return;
                    }
                    else
                    {
                        playerMoved = true;
                        playerX = moveX;
                        if(playerX <= 0)
                        {
                            playerX = 0;
                        }
                    }
                }
                if(playerInput.Key == ConsoleKey.D || playerInput.Key == ConsoleKey.RightArrow)
                {
                    //Moves player right
                    moveX = (playerX + 1);
                    if(moveX >= playerMaxX)
                    {
                        moveX = playerMaxX; //Locks top of screen
                    }
                    if(moveX == enemy1X && playerY == enemy1Y)
                    {
                        DoDamage(playerDamage, 1);
                        return;
                    }
                    if(moveX == enemy2X && playerY == enemy2Y)
                    {
                        DoDamage(playerDamage, 2);
                        return;
                    }
                    if(dungeonMap[playerY,moveX] == '#')
                    {
                        moveX = playerX;
                        playerX = moveX;
                        return;
                    }
                    else
                    {
                        playerMoved = true;
                        playerX = moveX;
                        if(playerX >= playerMaxX)
                        {
                            playerX = playerMaxX;
                        }
                    }
                }
                if(dungeonMap[playerY,playerX] == '$')
                {
                    gameWon = true;
                    gameIsOver = true;
                }
                if(dungeonMap[playerY,playerX] == '~')
                {
                    levelNumber += 1;
                    ChangeLevels();
                }
                if(dungeonMap[playerY,playerX] == '@')
                {
                    playerCoins += 1;
                    dungeonMap[playerY,playerX] = '-';
                }
                if(dungeonMap[playerY,playerX] == '*')
                    {
                        playerHP -= 1;
                        if(playerHP <= 0)
                        {
                            gameIsOver = true;
                            gameWon = false;
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
            Console.WriteLine("Try to get to the 3rd floor and collect the grail!");
            Console.WriteLine("Collect gold along the way to increase your power");
            Console.WriteLine("Press any key to get stated, Escape will exit once in game.");
            Console.ReadKey(true);
        }
        static void SetPlayerDamage()
        {
            playerDamage = playerCoins;
            if(playerDamage <= 0)
            {
                playerDamage = 1;
            }
        }
        static void DoDamage(int damage, int enemyNumber)
        {
            if(enemyNumber == 1)
            {
                enemy1HP -= damage;
                if(enemy1HP <= 0)
                {
                    enemy1X = 0;
                    enemy1Y = 0;
                    enemy1IsActive = false;
                }
            }
            if(enemyNumber == 2)
            {
                enemy2HP -= damage;
                if(enemy2HP <= 0)
                {
                    enemy2X = 0;
                    enemy2Y = 0;
                    enemy2IsActive = false;
                }
            }
            else
            {return;}
        }
        static void SetEnemyActive()
        {
            enemy1IsActive = true;
            enemy2IsActive = true;
        }
        static void TakeDamage(int damage)
        {
            //Enemies ability to damage the players
            playerHP -= damage;
            if(playerHP <= 0)
            {
                gameWon = false;
                gameIsOver = true;
            }
        }
        static void MoveEnemy1()
        {
            int enemyMoveX;
            int enemyMoveY;
            Random moveRoll = new Random();
            int moveResult = moveRoll.Next(1,5);
            if(moveResult == 1)
            {
                enemyMoveY = enemy1Y - 1;
                if(enemyMoveY <= 0)
                {
                    enemyMoveY = 0;
                }
                if(enemyMoveY == playerY && enemy1X == playerX)
                {
                    TakeDamage(enemyDamage);
                    return;
                }
                if(dungeonMap[enemyMoveY,enemy1X] == '#')
                {
                    enemyMoveY = enemy1Y;
                    enemy1Y = enemyMoveY;
                    return;
                }
                else
                {
                    enemy1Y = enemyMoveY;
                    if(enemy1Y <= 0)
                    {
                        enemy1Y = 0;
                    }
                }
            }
            if(moveResult == 2)
            {
                enemyMoveY = enemy1Y + 1;
                if(enemyMoveY >= playerMaxY)
                {
                    enemyMoveY = playerMaxY;
                }
                if(enemyMoveY == playerY && enemy1X == playerX)
                {
                    TakeDamage(enemyDamage);
                    return;
                }
                if(dungeonMap[enemyMoveY,enemy1X] == '#')
                {
                    enemyMoveY = enemy1Y;
                    enemy1Y = enemyMoveY;
                    return;
                }
                else
                {
                    enemy1Y = enemyMoveY;
                    if(enemy1Y >= playerMaxY)
                    {
                        enemy1Y = playerMaxY;
                    }
                }
            }
            if(moveResult == 3)
            {
                enemyMoveX = enemy1X - 1;
                if(enemyMoveX >= playerMaxX)
                {
                    enemyMoveX = playerMaxX;
                }
                if(enemyMoveX <= 0)
                {
                    enemyMoveX = 0;
                }
                if(enemyMoveX == playerX && enemy1Y == playerY)
                {
                    TakeDamage(enemyDamage);
                    return;
                }
                if(dungeonMap[enemy1Y,enemyMoveX] == '#')
                {
                    enemyMoveX = enemy1X;
                    enemy1X = enemyMoveX;
                    return;
                }
                else
                {
                    enemy1X = enemyMoveX;
                    if(enemy1X <= 0)
                    {
                        enemy1X = 0;
                    }
                }
            }
            if(moveResult == 4)
            {
                enemyMoveX = enemy1X + 1;
                if(enemyMoveX == playerX && enemy1Y == playerY)
                {
                    TakeDamage(enemyDamage);
                    return;
                }
                if(dungeonMap[enemy1Y,enemyMoveX] == '#')
                {
                    enemyMoveX = enemy1X;
                    enemy1X = enemyMoveX;
                    return;
                }
                else
                {
                    enemy1X = enemyMoveX;
                    if(playerX >= playerMaxX)
                    {
                        enemy1X = playerMaxX;
                    }
                }
            }
        }
        static void MoveEnemy2()
        {
            int enemyMoveX;
            int enemyMoveY;
            int rangeMaxX = 7;
            int rangeMaxY = 5;
            int rangeX = enemy2X - playerX;
            int rangeY = enemy2Y - playerY;
            if((rangeX < rangeMaxX && rangeX > -rangeMaxX)&&(rangeY < rangeMaxY && rangeY > -rangeMaxY))
            {
                if(rangeX < rangeMaxX && rangeX > 0)
                {
                    enemyMoveX = enemy2X - 1;
                    if(enemyMoveX == playerX && enemy2Y == playerY)
                    {
                        TakeDamage(enemyDamage);
                        return;
                    }
                    if(dungeonMap[enemy2Y,enemyMoveX] == '#')
                    {
                        enemyMoveX = enemy2X;
                        enemy2X = enemyMoveX;
                        return;
                    }
                    else
                    {
                        enemy2X = enemyMoveX;
                        if(playerX >= playerMaxX)
                        {
                            enemy2X = playerMaxX;
                        }
                        return;
                    }
                }
                if(rangeX > -rangeMaxX && rangeX < 0)
                {
                    enemyMoveX = enemy2X + 1;
                    if(enemyMoveX >= playerMaxX)
                    {
                        enemyMoveX = playerMaxX;
                    }
                    if(enemyMoveX <= 0)
                    {
                        enemyMoveX = 0;
                    }
                    if(enemyMoveX == playerX && enemy2Y == playerY)
                    {
                        TakeDamage(enemyDamage);
                        return;
                    }
                    if(dungeonMap[enemy2Y,enemyMoveX] == '#')
                    {
                        enemyMoveX = enemy2X;
                        enemy2X = enemyMoveX;
                        return;
                    }
                    else
                    {
                        enemy2X = enemyMoveX;
                        if(enemy2X <= 0)
                        {
                            enemy2X = 0;
                        }
                        return;
                    }
                }
            }
            if((rangeX < rangeMaxX && rangeX > -rangeMaxX)&&(rangeY < rangeMaxY && rangeY > -rangeMaxY))
            {
                if(rangeY < rangeMaxY && rangeY > 0)
                {
                    enemyMoveY = enemy2Y - 1;
                    if(enemyMoveY >= playerMaxY)
                    {
                        enemyMoveY = playerMaxY;
                    }
                    if(enemyMoveY == playerY && enemy2X == playerX)
                    {
                        TakeDamage(enemyDamage);
                        return;
                    }
                    if(dungeonMap[enemyMoveY,enemy2X] == '#')
                    {
                        enemyMoveY = enemy2Y;
                        enemy2Y = enemyMoveY;
                        return;
                    }
                    else
                    {
                        enemy2Y = enemyMoveY;
                        if(enemy2Y >= playerMaxY)
                        {
                            enemy2Y = playerMaxY;
                        }
                        return;
                    }
                }
                if(rangeY > -rangeMaxY && rangeY < 0)
                {
                    enemyMoveY = enemy2Y + 1;
                    if(enemyMoveY <= 0)
                    {
                        enemyMoveY = 0;
                    }
                    if(enemyMoveY == playerY && enemy2X == playerX)
                    {
                        TakeDamage(enemyDamage);
                        return;
                    }
                    if(dungeonMap[enemyMoveY,enemy2X] == '#')
                    {
                        enemyMoveY = enemy2Y;
                        enemy2Y = enemyMoveY;
                        return;
                    }
                    else
                    {
                        enemy2Y = enemyMoveY;
                        if(enemy2Y <= 0)
                        {
                            enemy2Y = 0;
                        }
                        return;
                    }
                }
                else
                {return;}
            }
        }
    }
}
