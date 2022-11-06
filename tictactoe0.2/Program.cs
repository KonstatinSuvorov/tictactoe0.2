using System;

namespace tictactoe02
{
    class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            while (true)
            {
                int modeId = selectGameMode(new string[] { "Играть вдвоём.", "Игра с ботом." });
                beginGame(modeId);
            }
        }

        private static int selectGameMode(string[] modeTitles)
        {
            int modeId = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("\t\t*** КРЕСТИКИ-НОЛИКИ ***");

                for (int i = 0; i < modeTitles.Length; i++)
                {
                    if (i == modeId)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine(modeTitles[i]);
                    Console.ResetColor();
                }


                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Enter || key == ConsoleKey.Spacebar)
                {
                    return modeId;
                }
                else if (key == ConsoleKey.DownArrow) modeId++;
                else if (key == ConsoleKey.UpArrow) modeId--;

                modeId = clamp(modeId, 0, modeTitles.Length - 1);
            }
        }

        private static void beginGame(int gameMode)
        {
            Board gameBoard = new Board();
            gameBoard.Restart(beginFromX: true, withBot: gameMode == 1);

            int x = 0;
            int y = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("УПРАВЛЕНИЕ:\nArrows - переместиться\nSpace - сделать ход\nEsc - сброс\n\n");
                Console.WriteLine("Ход для {0}:\n", gameBoard.TurnX ? "КРЕСТИКА" : "НОЛИКА");

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (i == y && j == x)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }

                        Console.Write($" {gameBoard[i, j]} ");
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }

                Board.State state = gameBoard.GetState;
                if (state == Board.State.ZeroWins)
                    Console.WriteLine("НОЛИКИ ПОБЕДИЛИ!!!");
                else if (state == Board.State.CrossWins)
                    Console.WriteLine("КРЕСТИКИ ПОБЕДИЛИ!!!");
                else if (state == Board.State.Friedship)
                    Console.WriteLine("НИЧЬЯ!");

                // обработка клавиш
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Spacebar)
                {
                    gameBoard.Turn(x, y);

                    if (gameBoard.WithBot)
                    {
                        gameBoard.TurnBot();
                    }
                }
                else if (key == ConsoleKey.LeftArrow) x--;
                else if (key == ConsoleKey.RightArrow) x++;
                else if (key == ConsoleKey.UpArrow) y--;
                else if (key == ConsoleKey.DownArrow) y++;
                else if (key == ConsoleKey.Escape)
                {
                    return;
                }

                x = clamp(x, 0, 2);
                y = clamp(y, 0, 2);

            }
        }

        private static int clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

    }

    
        internal class Board
        {
            public enum State
            {
                None, CrossWins, ZeroWins, Friedship
            }

            public State GetState => getBoardState();
            public bool TurnX { get; private set; }
            public bool WithBot { get; private set; }

            private char[,] _board;

            private const char X = 'X';
            private const char O = 'O';
            private const char EMPTY = '-';

            private Random _rand = new Random();

            public char this[int y, int x] => _board[y, x];

            public void Restart(bool beginFromX = true, bool withBot = false)
            {
                if (_board == null)
                    _board = new char[3, 3];

                for (int i = 0; i < 9; i++)
                    _board[i / 3, i % 3] = EMPTY;

                TurnX = beginFromX;
                WithBot = withBot;
            }

            public void Turn(int x, int y)
            {
                if (x < 0 || x > 2) return;
                if (y < 0 || y > 2) return;
                if (_board[y, x] != EMPTY) return;
                if (GetState != State.None) return;

                _board[y, x] = TurnX ? X : O;
                TurnX = !TurnX;
            }

            public void TurnBot()
            {
                int cell = _rand.Next(9);

                for (int i = 0; i < 9; i++)
                {
                    int x = cell % 3;
                    int y = cell / 3;
                    if (_board[y, x] == EMPTY)
                    {
                        Turn(x, y);
                        break;
                    }
                    cell = ++cell % 9;
                }
            }

            private State getBoardState()
            {
                int XXX = X * 3;
                int OOO = O * 3;

                for (int i = 0; i < 3; i++)
                {
                    // строка
                    int resRow = _board[i, 0] + _board[i, 1] + _board[i, 2];

                    // колонка
                    int resCol = _board[0, i] + _board[1, i] + _board[2, i];

                    if (resRow == XXX || resCol == XXX) return State.CrossWins;
                    else if (resRow == OOO || resCol == OOO) return State.ZeroWins;
                }

                //диагонали
                int d1 = _board[0, 0] + _board[1, 1] + _board[2, 2];
                int d2 = _board[2, 0] + _board[1, 1] + _board[0, 2];

                if (d1 == XXX || d2 == XXX) return State.CrossWins;
                else if (d1 == OOO || d2 == OOO) return State.ZeroWins;


                // нет победы, но остались пустые  места
                for (int i = 0; i < 9; i++)
                    if (_board[i / 3, i % 3] == EMPTY) return State.None;

                // нет победы и нет пустых мест
                return State.Friedship;

            }

        }

}