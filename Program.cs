using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace countdown
{
    class Program
    {
        static void Main(string[] args)
        {
            char[,] gameBoard = new char[23, 53];
            int[] positions_0 = new int[70];
            bool flag0 = false;
            int timer = 0;
            int life = 5;
            int pX = 16;
            int pY = 8;
            int score = 0;
            bool protection = false;

            board(ref gameBoard);
            BoardUpdater(ref gameBoard);
            InfoUpdater(score: score);
            InfoUpdater(life: life);
            
            int[,] posZero = UpdateZerosPosition(gameBoard);

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            DateTime old = DateTime.Now;
            

            while (life != 0)
            {
                
                if ((DateTime.Now - old).TotalMilliseconds > 50)
                {
                    
                    if (timer % 20 == 0)
                        InfoUpdater(time: (timer / 20));
                    if (protection == true && timer % 60 == 0) //protection timeout
                        protection = false;
                    timer++;
                    ConsoleKeyInfo key1 = new ConsoleKeyInfo();
                    Console.SetCursorPosition(52, 0);
                    

                    while (Console.KeyAvailable)
                        key1 = Console.ReadKey();

                    PlayerMove(ref gameBoard, ref posZero, ref pX, ref pY, ref score, ref life, ref protection, key1);
                    if (timer % 20 == 0)
                    {
                        zeroPosFinder(gameBoard, ref positions_0);
                        zeroMovement(ref flag0, ref gameBoard, ref positions_0, ref life);
                    }

                    if (timer % 300 == 0)
                    {
                        Countdown(ref gameBoard);
                        BoardUpdater(ref gameBoard);
                    }
                    old = DateTime.Now;
                }
            }
            Console.Clear();
            Console.WriteLine("GAME OVER");
            InfoUpdater(score: score);
            InfoUpdater(time: (timer / 20));
            Thread.Sleep(3000);
            Console.WriteLine("enter a key to close the tab");
            Console.ReadLine();
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

                            int randomNumber = random.Next(0,99);
                            if (randomNumber < 3)
                                gameBoard[i, j] = '0';
                        }
                    }
                }
            }
        }
        static void BoardUpdater(ref char[,] gameBoard)
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    if (gameBoard[i, j] == '#') Console.Write("\x1b[95m" + gameBoard[i, j]);
                    else if (gameBoard[i, j] == 'P') Console.Write("\x1b[32m" + gameBoard[i, j]);
                    else if (gameBoard[i, j] != ' ') Console.Write("\x1b[94m" + gameBoard[i, j]);
                    else Console.Write(gameBoard[i, j]);

                }
                Console.WriteLine();
            }
        }
        static void InfoUpdater(int time = -1, int life = -1, int score = -1)
        {
            Console.Write("\x1b[32m");
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
            Console.Write("\x1b[39m");

        }
        static int[,] UpdateZerosPosition(char[,] gameBoard)
        {
            int Count = 0;
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    if (gameBoard[i, j] == '0')
                        Count++;
                }
            }

            int[,] zerosPos = new int[Count, 2];
            Count = 0;
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    if (gameBoard[i, j] == '0')
                    {
                        zerosPos[Count, 0] = j;
                        zerosPos[Count, 1] = i;
                        Count++;
                    }
                }
            }
            return zerosPos;
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
                    zeroPos = UpdateZerosPosition(gameBoard);
                }
            }
            Console.Write("\x1b[32m");
            PosUpdater(ref gameBoard, 'P', pX, pY);
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
        static void board(ref char[,] gameBoard)
        {
            Random random = new Random();
            
            for (int i = 0; i < 23; i++)
            {
                for (int j = 0; j < 53; j++)
                {
                    gameBoard[i, j] = ' ';
                }
            }


            for (int i = 0; i < 23; i++)
            {
                for (int j = 0; j < 53; j++)
                {
                    if (i == 0)
                        gameBoard[i, j] = '#';
                    else if (((i != 0) && (j == 0)) || ((i != 0) && (j == 52)))
                        gameBoard[i, j] = '#';
                    else if (i == 22)
                        gameBoard[i, j] = '#';
                    else
                        gameBoard[i, j] = ' ';
                }
            }

            int wall_type = 0;
            int wall_direction = 0;
            int place_line = 0;
            int place_column = 0;
            int wall_counter = 0;
            bool flag = true;

            while (flag)
            {
                bool flag1 = true;
                int wall_controller = 0;
                wall_type = random.Next(1, 4);                           // 1 for 3 length wall, 2 for 7 length wall, 3 for 11 length wall
                wall_direction = random.Next(1, 3);                      // 1 for vertical, 2 for horizontal
                if ((wall_type == 1) && (wall_direction == 1))
                {
                    while (flag1)
                    {
                        place_line = random.Next(2, 19);
                        place_column = random.Next(2, 51);
                        for (int i = place_line - 1; i <= place_line + 3; i++)
                        {
                            for (int j = place_column - 1; j <= place_column + 1; j++)
                            {
                                if (gameBoard[i, j] == ' ')
                                    wall_controller++;
                            }
                        }
                        if (wall_controller == 15)
                        {
                            gameBoard[place_line, place_column] = '#';
                            gameBoard[place_line + 1, place_column] = '#';
                            gameBoard[place_line + 2, place_column] = '#';
                            flag1 = false;
                            wall_counter++;
                        }
                        else
                            wall_controller = 0;
                    }
                }

                else if ((wall_type == 1) && (wall_direction == 2))
                {
                    while (flag1)
                    {
                        place_line = random.Next(2, 21);
                        place_column = random.Next(2, 49);
                        for (int i = place_line - 1; i <= place_line + 1; i++)
                        {
                            for (int j = place_column - 1; j <= place_column + 3; j++)
                            {
                                if (gameBoard[i, j] == ' ')
                                    wall_controller++;
                            }
                        }
                        if (wall_controller == 15)
                        {
                            gameBoard[place_line, place_column] = '#';
                            gameBoard[place_line, place_column + 1] = '#';
                            gameBoard[place_line, place_column + 2] = '#';
                            flag1 = false;
                            wall_counter++;
                        }
                        else
                            wall_controller = 0;
                    }
                }

                else if ((wall_type == 2) && (wall_direction == 1))
                {
                    while (flag1)
                    {
                        place_line = random.Next(2, 15);
                        place_column = random.Next(2, 51);
                        for (int i = place_line - 1; i <= place_line + 7; i++)
                        {
                            for (int j = place_column - 1; j <= place_column + 1; j++)
                            {
                                if (gameBoard[i, j] == ' ')
                                    wall_controller++;
                            }
                        }
                        if (wall_controller == 27)
                        {
                            gameBoard[place_line, place_column] = '#';
                            gameBoard[place_line + 1, place_column] = '#';
                            gameBoard[place_line + 2, place_column] = '#';
                            gameBoard[place_line + 3, place_column] = '#';
                            gameBoard[place_line + 4, place_column] = '#';
                            gameBoard[place_line + 5, place_column] = '#';
                            gameBoard[place_line + 6, place_column] = '#';
                            flag1 = false;
                            wall_counter++;
                        }
                        else
                            wall_controller = 0;
                    }
                }

                else if ((wall_type == 2) && (wall_direction == 2))
                {
                    while (flag1)
                    {
                        place_line = random.Next(2, 21);
                        place_column = random.Next(2, 45);
                        for (int i = place_line - 1; i <= place_line + 1; i++)
                        {
                            for (int j = place_column - 1; j <= place_column + 7; j++)
                            {
                                if (gameBoard[i, j] == ' ')
                                    wall_controller++;
                            }
                        }
                        if (wall_controller == 27)
                        {
                            gameBoard[place_line, place_column] = '#';
                            gameBoard[place_line, place_column + 1] = '#';
                            gameBoard[place_line, place_column + 2] = '#';
                            gameBoard[place_line, place_column + 3] = '#';
                            gameBoard[place_line, place_column + 4] = '#';
                            gameBoard[place_line, place_column + 5] = '#';
                            gameBoard[place_line, place_column + 6] = '#';
                            flag1 = false;
                            wall_counter++;
                        }
                        else
                            wall_controller = 0;
                    }
                }

                else if ((wall_type == 3) && (wall_direction == 1))
                {
                    while (flag1)
                    {
                        place_line = random.Next(2, 11);
                        place_column = random.Next(2, 51);
                        for (int i = place_line - 1; i <= place_line + 11; i++)
                        {
                            for (int j = place_column - 1; j <= place_column + 1; j++)
                            {
                                if (gameBoard[i, j] == ' ')
                                    wall_controller++;
                            }
                        }
                        if (wall_controller == 39)
                        {
                            gameBoard[place_line, place_column] = '#';
                            gameBoard[place_line + 1, place_column] = '#';
                            gameBoard[place_line + 2, place_column] = '#';
                            gameBoard[place_line + 3, place_column] = '#';
                            gameBoard[place_line + 4, place_column] = '#';
                            gameBoard[place_line + 5, place_column] = '#';
                            gameBoard[place_line + 6, place_column] = '#';
                            gameBoard[place_line + 7, place_column] = '#';
                            gameBoard[place_line + 8, place_column] = '#';
                            gameBoard[place_line + 9, place_column] = '#';
                            gameBoard[place_line + 10, place_column] = '#';
                            flag1 = false;
                            wall_counter++;
                        }
                        else
                            wall_controller = 0;
                    }
                }

                else if ((wall_type == 3) && (wall_direction == 2))
                {
                    while (flag1)
                    {
                        place_line = random.Next(2, 21);
                        place_column = random.Next(2, 41);
                        for (int i = place_line - 1; i <= place_line + 1; i++)
                        {
                            for (int j = place_column - 1; j <= place_column + 11; j++)
                            {
                                if (gameBoard[i, j] == ' ')
                                    wall_controller++;
                            }
                        }
                        if (wall_controller == 39)
                        {
                            gameBoard[place_line, place_column] = '#';
                            gameBoard[place_line, place_column + 1] = '#';
                            gameBoard[place_line, place_column + 2] = '#';
                            gameBoard[place_line, place_column + 3] = '#';
                            gameBoard[place_line, place_column + 4] = '#';
                            gameBoard[place_line, place_column + 5] = '#';
                            gameBoard[place_line, place_column + 6] = '#';
                            gameBoard[place_line, place_column + 7] = '#';
                            gameBoard[place_line, place_column + 8] = '#';
                            gameBoard[place_line, place_column + 9] = '#';
                            gameBoard[place_line, place_column + 10] = '#';
                            flag1 = false;
                            wall_counter++;
                        }
                        else
                            wall_controller = 0;
                    }
                }

                if (wall_counter == 20)
                    flag = false;
            }

            bool flag2 = true;
            int counter = 0;
            while (flag2)
            {
                Random rnd1 = new Random();
                int x2 = rnd1.Next(1, 22);
                int y2 = rnd1.Next(1, 52);
                char numbers = Convert.ToChar(rnd1.Next(0, 10) + 48);

                if (gameBoard[x2, y2] == ' ')
                {
                    gameBoard[x2, y2] = numbers;
                    counter++;
                }
                if (counter == 70)
                    flag2 = false;
            }


            for (int m = 0; m < 23; m++)
            {
                for (int n = 0; n < 53; n++)
                    Console.Write(gameBoard[m, n]);
                Console.WriteLine();
            }

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
        static void zeroMovement(ref bool flag0, ref char[,]gameBoard, ref  int[]positions_0, ref int life)
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

                    
                    int direction = which_direction.Next(1, 5);
                    bool flag1 = boslukmu(gameBoard, positions_0, i, direction);
                    if (flag1 && direction == 1)
                    {
                        gameBoard = zeromovement(gameBoard, positions_0, i, direction);
                        flag0 = true;
                        positions_0 = zeronewposition(positions_0, i, direction);
                    }
                    else if ((gameBoard[positions_0[i], positions_0[i + 1] + 1] == 'P'))
                    {
                        
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
                    else if (gameBoard[positions_0[i] - 1, positions_0[i + 1]] == 'P')
                    {
                        
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
                    else if (gameBoard[positions_0[i], positions_0[i + 1] - 1] == 'P')
                    {
                        
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
                    else if (gameBoard[positions_0[i] + 1, positions_0[i + 1]] == 'P')
                    {
                        
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
            Console.CursorVisible = false;
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
    }
}

        

