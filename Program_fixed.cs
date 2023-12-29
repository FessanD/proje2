using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;


namespace ConsoleApp7
{
    class Program
    {
        //Config
        const String soundEffectPath = "soundeffect.wav"; 
        const char wallChar = '#';
        static char[] numbersChar = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        const char emptyChar = ' ';
        const char playerChar = 'A';
        static int gameBoardWidth = 53;
        static int gameBoardHeight = 23;

        static int[] wallLenghts = { 3, 5, 7 };
        static int[] difficultyStartLife = { 7, 5, 1 };
        static string[] difficulties = { "easy", "medium", "hard" };
        const int wallCount = 20;
        const int numberCount = 70;

        static int startLife = 5;

        //Colors
        static String lightMagenta = "\x1b[95m";
        static String green = "\x1b[32m";
        static String lightBlue = "\x1b[94m";
        static String defaultColor = "\x1b[94m";



        static void Main(string[] args)
        {
            SoundPlayer sp = new SoundPlayer(soundEffectPath);
            sp.Load();


            Console.CursorVisible = false;
            while (true)
            {
                selectDifficulty(sp);
                char[,] gameBoard = new char[gameBoardHeight, gameBoardWidth];
                int[,] posZero = new int[gameBoardHeight * gameBoardWidth, 2]; ;
                int[] positions_0 = new int[70];
                int score = 0;
                int timer = 0;


                initializeboard(gameBoard, posZero);//✅
                BoardUpdater(gameBoard);//✅
                InfoUpdater(score: 0, life: startLife);//✅         
                playGame(posZero: posZero, gameBoard: gameBoard, positions_0: positions_0, timer: ref timer, score: ref score);
                if (!gameEndingScreen(score, sp))
                {
                    break;
                }
            }
            Console.Clear();

            Console.ReadLine();
        }

        static void selectDifficulty(SoundPlayer sp)
        {
            int selectedDifficulty = 0;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Select Difficulty");
                Console.WriteLine(" ");
                for (int i = 0; i < difficulties.Length; i++)
                {
                    if (selectedDifficulty == i)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine(value: difficulties[i]);
                    Console.ResetColor();
                }

                var ch = Console.ReadKey(false).Key;
                switch (ch)
                {
                    case ConsoleKey.DownArrow:
                        sp.Play();
                        selectedDifficulty = Math.Min(selectedDifficulty + 1, difficulties.Length - 1);
                        break;
                    case ConsoleKey.UpArrow:
                        sp.Play();
                        selectedDifficulty = Math.Max(selectedDifficulty - 1, 0);
                        break;
                    case ConsoleKey.Enter:
                        startLife = difficultyStartLife[selectedDifficulty];
                        return;
                }
            }
        }
        static bool gameEndingScreen(int score, SoundPlayer sp)
        {
            bool playAgain = true;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("               GAME OVER");
                Console.WriteLine(" ");
                Console.WriteLine("               SCORE: " + score);
                Console.WriteLine(" ");
                Console.WriteLine("        Do you want to Play Again ?                   ");
                Console.WriteLine("");
                Console.Write("           ");


                if (playAgain)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.Write(value: "Yes");
                Console.ResetColor();
                Console.Write(value: "                ");

                if (!playAgain)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(value: "No");
                Console.ResetColor();


                var ch = Console.ReadKey(false).Key;
                switch (ch)
                {
                    case ConsoleKey.LeftArrow:
                        sp.Play();
                        playAgain = true;

                        break;
                    case ConsoleKey.RightArrow:
                        sp.Play();
                        playAgain = false;
                        break;
                    case ConsoleKey.Enter:

                        return playAgain;
                }
            }
        }
        static void playGame(int[,] posZero, char[,] gameBoard, int[] positions_0, ref int timer, ref int score)
        {
            DateTime old = DateTime.Now;
            int pX = 16;
            int pY = 8;
            bool protection = false;
            bool flag0 = false;
            int life = startLife;

            while (life > 0)
            {

                if ((DateTime.Now - old).TotalMilliseconds > 50)
                {

                    if (timer % 20 == 0)
                    {
                        InfoUpdater(time: (timer / 20));
                    }


                    if (protection && timer % 60 == 0) //protection timeout
                        protection = false;

                    Console.SetCursorPosition(52, 0);
                    ConsoleKeyInfo key1 = new ConsoleKeyInfo();
                    while (Console.KeyAvailable)
                        key1 = Console.ReadKey();

                    PlayerMove(ref gameBoard, ref posZero, ref pX, ref pY, ref score, ref life, ref protection, key1);

                    if (timer % 20 == 0)
                    {
                        zeroPosFinder(gameBoard, ref positions_0);
                        zeroMovement(ref flag0, ref gameBoard, ref positions_0, ref life, ref protection);
                    }

                    if (timer % 300 == 0)
                    {
                        Countdown(ref gameBoard);
                        BoardUpdater(gameBoard);
                    }

                    old = DateTime.Now;
                    timer++;
                }
            }
        }
        static void PosUpdater(ref char[,] gameBoard, char value, int x, int y)
        {
            gameBoard[y, x] = value;
            Console.SetCursorPosition(x, y);
            Console.Write(value + "\x1b[94m");
        }
        static void Countdown(ref char[,] gameBoard)
        {
            Random random = new Random();
            for (int i = 1; i < gameBoard.GetLength(0) - 1; i++)
            {
                for (int j = 1; j < gameBoard.GetLength(1) - 1; j++)
                {

                    if (48 < gameBoard[i, j] && gameBoard[i, j] < 58)
                    {
                        if (Convert.ToInt32(gameBoard[i, j]) > 49)
                        {
                            gameBoard[i, j] = Convert.ToChar(gameBoard[i, j] - 1);
                        }

                        else if (gameBoard[i, j] == 49)
                        {

                            int randomNumber = random.Next(0, 99);
                            if (randomNumber < 3)
                                gameBoard[i, j] = '0';
                        }
                    }
                }
            }
        }

        //Update Board
        static void BoardUpdater(char[,] gameBoard)
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    switch (gameBoard[i, j])
                    {
                        case wallChar:
                            Console.Write(lightMagenta + gameBoard[i, j]);
                            break;
                        case playerChar:
                            Console.Write(green + gameBoard[i, j]);
                            break;
                        case emptyChar:
                            Console.Write(lightBlue + gameBoard[i, j]);
                            break;
                        default:
                            Console.Write(gameBoard[i, j]);
                            break;

                    }
                }
                Console.WriteLine();
            }
        }

        //Update Info
        static void InfoUpdater(int time = -1, int life = -1, int score = -1)
        {
            Console.Write(green);
            if (time != -1)
            {
                Console.SetCursorPosition(54, 0); Console.WriteLine("                    ");
                Console.SetCursorPosition(54, 0); Console.WriteLine("Time Elapsed:" + time);
            }
            if (life != -1)
            {
                Console.SetCursorPosition(54, 1); Console.WriteLine("                    ");
                Console.SetCursorPosition(54, 1); Console.WriteLine("Life:" + life);
            }
            if (score != -1)
            {
                Console.SetCursorPosition(54, 2); Console.WriteLine("                    ");
                Console.SetCursorPosition(54, 2); Console.WriteLine("Score:" + score);
            }
            Console.Write(defaultColor);

        }

        //Find Zero Positions      
        static void FindZeroPositions(char[,] gameBoard, int[,] posZero)
        {
            int zeroCount = 0;
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    if (gameBoard[i, j] == '0')
                    {
                        posZero[zeroCount, 0] = i;
                        posZero[zeroCount, 1] = j;
                        zeroCount++;
                    }

                }
            }
        }



        static void PlayerMove(ref char[,] gameBoard, ref int[,] zeroPos, ref int pX, ref int pY, ref int score, ref int life, ref bool protection, ConsoleKeyInfo key)
        {
            int dirX = 0, dirY = 0;
            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W) dirY = -1;
            else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S) dirY = 1;
            else if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.A) dirX = -1;
            else if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.D) dirX = 1;

            PosUpdater(ref gameBoard, ' ', pX, pY);

            if (gameBoard[pY + dirY, pX + dirX] == '0' && !protection)
            {
                Console.Beep();
                life--;
                protection = true;
                InfoUpdater(life: life);
            }

            if (gameBoard[pY + dirY, pX + dirX] == ' ')
            {
                pX += dirX;
                pY += dirY;
            }
            else if (gameBoard[pY + dirY, pX + dirX] != '#')
            {
                int min = 10;
                int borderX = 0;
                int borderY = 0;
                bool pushFlag = false;
                bool smashFlag = false;
                int tempScore;
                for (int tempX = pX + dirX, tempY = pY + dirY; gameBoard[tempY, tempX] - 48 <= min; tempX += dirX, tempY += dirY)
                {
                    char currSqr = gameBoard[tempY, tempX];
                    min = currSqr - 48;

                    if (currSqr == ' ')
                    {
                        borderX = tempX;
                        borderY = tempY;
                        pushFlag = true;
                        break;
                    }
                    if (currSqr == '#')
                    {
                        if (!(Math.Abs(pX - tempX) > 2 || Math.Abs(pY - tempY) > 2))
                        {
                            break;
                        }
                        borderX = tempX - dirX;
                        borderY = tempY - dirY;
                        smashFlag = true;
                        break;
                    }
                }

                if (pushFlag || smashFlag)
                {
                    if (smashFlag)
                    {
                        tempScore = gameBoard[borderY, borderX] - 48;
                        if (tempScore == 0) score += 20;
                        else if (tempScore < 5) score += 2;
                        else if (tempScore < 10) score += 1;
                        InfoUpdater(score: score);

                        Random random = new Random();
                        int newX, newY;
                        do
                        {
                            newY = random.Next(1, gameBoard.GetLength(0) - 1);
                            newX = random.Next(1, gameBoard.GetLength(1) - 1);
                        } while (gameBoard[newY, newX] != ' ');
                        PosUpdater(ref gameBoard, Convert.ToChar(random.Next(5, 10) + 48), newX, newY);
                    }

                    for (int tempX = borderX, tempY = borderY; (pX + dirX != tempX) || (pY + dirY != tempY); tempX -= dirX, tempY -= dirY)
                    {
                        PosUpdater(ref gameBoard, gameBoard[tempY - dirY, tempX - dirX], tempX, tempY);
                    }
                    pX += dirX;
                    pY += dirY;
                    PosUpdater(ref gameBoard, ' ', pX, pY);
                    FindZeroPositions(gameBoard, zeroPos);
                }
            }
            Console.Write("\x1b[32m");
            PosUpdater(ref gameBoard, playerChar, pX, pY);
            Console.Write("\x1b[39m");

        }
        static int[] positions_00(int[] positions_0)
        {
            int[] positions_0previous = new int[positions_0.Length];
            for (int i = 0; i < positions_0.Length; i += 2)
            {
                positions_0previous[i] = positions_0[i];
                positions_0previous[i + 1] = positions_0[i + 1];


            }
            return positions_0previous;
        }
        static bool boslukmu(char[,] gameboard, int[] positions_0, int i, int direction)
        {

            if (direction == 1)
            {
                if (gameboard[positions_0[i], positions_0[i + 1] + 1] == ' ')
                    return true;
                else
                    return false;
            }
            else if (direction == 2)
            {
                if (gameboard[positions_0[i] - 1, positions_0[i + 1]] == ' ')
                    return true;
                else
                    return false;
            }
            else if (direction == 3)
            {
                if (gameboard[positions_0[i], positions_0[i + 1] - 1] == ' ')
                    return true;
                else
                    return false;
            }

            else if (direction == 4)
            {
                if (gameboard[positions_0[i] + 1, positions_0[i + 1]] == ' ')

                    return true;
                else
                    return false;
            }
            return false;



        }
        static char[,] zeromovement(char[,] gameBoard, int[] positions_0, int i, int direction)
        {
            if (direction == 1)
            {
                gameBoard[positions_0[i], positions_0[i + 1]] = ' ';
                gameBoard[positions_0[i], positions_0[i + 1] + 1] = '0';
            }
            else if (direction == 2)
            {
                gameBoard[positions_0[i], positions_0[i + 1]] = ' ';
                gameBoard[positions_0[i] - 1, positions_0[i + 1]] = '0';
            }
            else if (direction == 3)
            {
                gameBoard[positions_0[i], positions_0[i + 1]] = ' ';
                gameBoard[positions_0[i], positions_0[i + 1] - 1] = '0';
            }
            else if (direction == 4)
            {
                gameBoard[positions_0[i], positions_0[i + 1]] = ' ';
                gameBoard[positions_0[i] + 1, positions_0[i + 1]] = '0';
            }
            return gameBoard;
        }
        static int[] zeronewposition(int[] positions_0, int i, int direction)
        {
            if (direction == 1)
            {
                positions_0[i + 1]++;
            }
            else if (direction == 2)
            {
                positions_0[i]--;
            }
            else if (direction == 3)
            {
                positions_0[i + 1]--;
            }
            else if (direction == 4)
            {
                positions_0[i]++;
            }

            return positions_0;
        }

        static void zeroPosFinder(char[,] gameBoard, ref int[] positions_0)
        {
            int k = 0;
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    if (gameBoard[i, j] == '0')
                    {

                        {
                            positions_0[k] = i;
                            positions_0[k + 1] = j;
                        }
                        k += 2;
                    }
                }
            }
        }
        static void zeroMovement(ref bool flag0, ref char[,] gameBoard, ref int[] positions_0, ref int life, ref bool prot)
        {

            Random which_direction = new Random();
            int[] positions_0previous = positions_00(positions_0);
            for (int i = 0; i < positions_0.GetLength(0); i += 2)
            {


                bool control1 = true;
                bool control2 = true;
                bool control3 = true;
                bool control4 = true;
                flag0 = false;
                while ((gameBoard[positions_0[i], positions_0[i + 1]] == '0') && (!flag0))
                {



                    bool flag1 = false;
                    int direction = 0;

                    do
                    {
                        bool ctrl = false;
                        if (positions_0[i] != -1)
                        {
                            if ((gameBoard[positions_0[i], positions_0[i + 1] + 1] == ' ') || (gameBoard[positions_0[i], positions_0[i + 1] + 1] == 'P'))
                                ctrl = true;
                            if ((gameBoard[positions_0[i] - 1, positions_0[i + 1]] == ' ') || (gameBoard[positions_0[i] - 1, positions_0[i + 1]] == 'P'))
                                ctrl = true;
                            if ((gameBoard[positions_0[i], positions_0[i + 1] - 1] == ' ') || (gameBoard[positions_0[i], positions_0[i + 1] - 1] == 'P'))
                                ctrl = true;
                            if ((gameBoard[positions_0[i] + 1, positions_0[i + 1]] == ' ') || (gameBoard[positions_0[i] + 1, positions_0[i + 1]] == 'P'))
                                ctrl = true;
                        }
                        if (!ctrl)
                            continue;
                        direction = which_direction.Next(1, 5);
                        flag1 = boslukmu(gameBoard, positions_0, i, direction);

                    } while (flag1 == false);
                    if (flag1 && direction == 1)
                    {
                        gameBoard = zeromovement(gameBoard, positions_0, i, direction);
                        flag0 = true;
                        positions_0 = zeronewposition(positions_0, i, direction);
                    }
                    else if (!prot && (gameBoard[positions_0[i], positions_0[i + 1] + 1] == playerChar))
                    {
                        life--;
                        prot = true;
                        flag0 = true;
                    }
                    else
                        control1 = false;

                    if (flag1 && direction == 2)
                    {
                        gameBoard = zeromovement(gameBoard, positions_0, i, direction);
                        flag0 = true;
                        positions_0 = zeronewposition(positions_0, i, direction);
                    }
                    else if (!prot && (gameBoard[positions_0[i] - 1, positions_0[i + 1]] == playerChar))
                    {
                        life--;
                        prot = true;
                        flag0 = true;
                    }
                    else
                        control2 = false;

                    if (flag1 && direction == 3)
                    {


                        gameBoard = zeromovement(gameBoard, positions_0, i, direction);
                        flag0 = true;
                        positions_0 = zeronewposition(positions_0, i, direction);
                    }
                    else if (!prot && (gameBoard[positions_0[i], positions_0[i + 1] - 1] == playerChar))
                    {
                        life--;
                        prot = true;
                        flag0 = true;
                    }
                    else
                        control3 = false;

                    if (flag1 && direction == 4)
                    {
                        gameBoard = zeromovement(gameBoard, positions_0, i, direction);
                        flag0 = true;
                        positions_0 = zeronewposition(positions_0, i, direction);
                    }
                    else if (!prot && (gameBoard[positions_0[i] + 1, positions_0[i + 1]] == playerChar))
                    {
                        life--;
                        prot = true;
                        flag0 = true;
                    }
                    else
                        control4 = false;

                    if ((!control1) && (!control2) && ((!control3) && (!control4)))
                    {

                        flag0 = true;

                    }
                }


            }
            {
                for (int i = 0; i < positions_0.GetLength(0); i += 2)
                {
                    if (positions_0[i] != -1)
                    {
                        Console.SetCursorPosition(positions_0previous[i + 1], positions_0previous[i]);
                        Console.Write(gameBoard[positions_0previous[i], positions_0previous[i + 1]]);
                        Console.SetCursorPosition(positions_0[i + 1], positions_0[i]);
                        Console.Write(gameBoard[positions_0[i], positions_0[i + 1]]);
                    }
                }
            }
        }



        //Initializing Board
        static void initializeboard(char[,] gameBoard, int[,] posZero)
        {
            createEmptyBoard(gameBoard);
            initializeWall(gameBoard);
            addRandomNumbers(gameBoard: gameBoard, count: numberCount);
            FindZeroPositions(gameBoard: gameBoard, posZero: posZero);
        }

        //Adding Random Numbers
        static void addRandomNumbers(char[,] gameBoard, int count)
        {
            for (int i = 0; i < count; i++)
            {
                addRandomNumber(gameBoard);
            }
        }
        static void addRandomNumber(char[,] gameBoard)
        {
            Random random = new Random();
            while (true)
            {
                int x = random.Next(1, 22);
                int y = random.Next(1, 52);
                char number = numbersChar[random.Next(0, 10)];

                if (gameBoard[x, y] == emptyChar)
                {
                    gameBoard[x, y] = number;
                    break;
                }
            }
        }

        //Creating Empty Board
        static void createEmptyBoard(char[,] gameBoard)
        {
            for (int i = 0; i < 23; i++)
            {
                for (int j = 0; j < 53; j++)
                {
                    if (i == 0 || i == 22 || j == 0 || j == 52)
                        gameBoard[i, j] = wallChar;
                    else
                        gameBoard[i, j] = emptyChar;
                }
            }
        }

        //Adding Walls
        static void initializeWall(char[,] gameBoard)
        {
            Random random = new Random();
            int counter = 0;
            int counter1 = 0;
            for (int wallCounter = 0; wallCounter < wallCount; wallCounter++)
            {
                bool isWallDirectionVertical = (random.Next(0, 2) == 0);
                while (true)
                {

                    int wallLength = wallLenghts[counter1];
                    if (counter == 20)
                        counter1++;
                    else if (counter == 25)
                        counter1++;
                    else if (counter == 28)
                        break;
                    counter++;
                    int startX = isWallDirectionVertical ? random.Next(2, 51) : random.Next(2, 51 - wallLength);
                    int endX = isWallDirectionVertical ? startX : startX + wallLength - 1;

                    int startY = isWallDirectionVertical ? random.Next(2, 21 - wallLength) : random.Next(2, 21);
                    int endY = isWallDirectionVertical ? startY + wallLength - 1 : startY;

                    if (isAroundEmpty(table: gameBoard, startX: startX, startY: startY, endX: endX, endY: endY))
                    {
                        for (int x = startX; x <= endX; x++)
                        {
                            for (int y = startY; y <= endY; y++)
                            {
                                gameBoard[y, x] = wallChar;
                            }
                        }
                        break;
                    }
                }
            }
        }

        //Checking Is Around Empty
        static bool isAroundEmpty(char[,] table, int startX, int startY, int endX, int endY)
        {
            startX = Math.Max(startX - 1, 0);
            endX = Math.Min(endX + 1, table.GetLength(1) - 1);
            startY = Math.Max(startY - 1, 0);
            endY = Math.Min(endY + 1, table.GetLength(0) - 1);

            for (int i = startX; i < endX; i++)
            {
                if (table[startY, i] == wallChar || table[endY, i] == wallChar)
                {
                    return false;
                }
            }

            for (int i = startY; i < endY; i++)
            {
                if (table[i, startX] == wallChar || table[i, endX] == wallChar)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
