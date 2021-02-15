using System;

namespace MatchThreeGame
{
    enum Tile
    {
        EmptyTile,
        Type1,
        Type2,
        Type3,
        Type4
    }

    enum State
    {
        Game,
        Win,
        GameOver
    }

    class Game
    {
        private int height;
        private int width;
        private int lineWithTiles;
        private int numOfTileTypes;
        private State state;
        private Tile[,] area;

        // Инициализация игрового поля
        public Game(int _height, int _width, int _lineWithTiles, int _numOfTileTypes)
        {
            height = _height;
            width = _width;
            lineWithTiles = _lineWithTiles;
            numOfTileTypes = _numOfTileTypes;
            state = State.Game;
            area = new Tile[height, width];
            for (int i = 0; i < lineWithTiles; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    area[i, j] = GetRandTile();
                }
            }
            for (int i = lineWithTiles; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    area[i, j] = Tile.EmptyTile;
                }
            }
        }

        // Игровая сессия
        public void GameSession()
        {
            while (state == State.Game)
            {
                Tile currentTile = GetRandTile();
                while (!CheckTile(currentTile)) { currentTile = GetRandTile(); }
                int line = GetLine(currentTile);
                FindPlace(currentTile, line - 1);
                state = GameState();
            }
            AreaPrint();
            if (state == State.Win) { Console.WriteLine("WIN!!!"); }
            if (state == State.GameOver) { Console.WriteLine("Game Over"); }
        }

        // Визуализация тайлов
        private void SymPrint(Tile tile)
        {
            switch (tile)
            {
                case Tile.EmptyTile:
                    Console.Write("   ");
                    break;
                case Tile.Type1:
                    Console.Write("@  ");
                    break;
                case Tile.Type2:
                    Console.Write("*  ");
                    break;
                case Tile.Type3:
                    Console.Write("#  ");
                    break;
                case Tile.Type4:
                    Console.Write("+  ");
                    break;
            }
        }

        // Визуализация игрового поля
        private void AreaPrint()
        {
            Console.Write("  ");
            for (int i = 0; i < width; i++)
            {
                Console.Write("{0, -3}", i + 1);
            }
            Console.Write("\n _");
            for (int i = 0; i < width; i++)
            {
                Console.Write("___");
            }
            Console.WriteLine();
            for (int i = 0; i < height; i++)
            {
                Console.Write("| ");
                for (int j = 0; j < width; j++)
                {
                    SymPrint(area[i, j]);
                }
                Console.WriteLine("|");
            }
            Console.WriteLine("");
        }

        // Визуализация текущего положения на игровом поле, текущего тайла и ввод пользователем линии
        private int GameStep(Tile tile)
        {
            AreaPrint();
            SymPrint(tile);
            Console.Write("\nline: ");
            int.TryParse(Console.ReadLine(), out int line);
            Console.WriteLine();
            return line;
        }

        // Получение введенной пользователем линии до тех пор, пока он не введет корректно
        private int GetLine(Tile tile)
        {
            int line = GameStep(tile);
            while (line < 1 || line > width || area[height - 1, line - 1] != Tile.EmptyTile)
            {
                Console.WriteLine("Wrong line!\n");
                line = GameStep(tile);
            }
            return line;
        }

        // Получение рандомного тайла (кроме пустого) из списка тайлов
        private Tile GetRandTile()
        {
            var values = Enum.GetValues(typeof(Tile));
            Random rnd = new Random();
            return (Tile)values.GetValue(rnd.Next(numOfTileTypes) + 1);
        }

        // Проверка, остались ли еще тайлы такого же типа на игровом поле
        private bool CheckTile(Tile tile)
        {
            for (int i = 0; i < height - 1; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (area[i, j] == tile) { return true; }
                }
            }
            return false;
        }

        // Проверка на выигрыш/проигрыш/продолжение игры
        private State GameState()
        {
            int n = 0;
            for (int i = 0; i < width; i++)
            {
                if (area[height - 1, i] != Tile.EmptyTile) { n++; }
            }
            if (n == width) { return State.GameOver; }
            for (int i = 0; i < height - 1; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (area[i, j] != Tile.EmptyTile) { return State.Game; }
                }
            }
            return State.Win;
        }

        // Поиск места для нового тайла
        private void FindPlace(Tile tile, int line)
        {
            int i = height - 1;
            while (i >= 0)
            {
                if (area[i, line] == Tile.EmptyTile && (i == 0 || area[i - 1, line] != Tile.EmptyTile))
                {
                    area[i, line] = tile;
                    Vertical(tile, i, line);
                    Horizontal(tile, i, line);
                    break;
                }
                i--;
            }
        }

        // Проверка, есть ли по вертикали 3 и более однотипных тайла и их удаление
        private void Vertical(Tile tile, int i, int line)
        {
            int j = i, count = 1;
            while (j > 0 && area[j - 1, line] == tile)
            {
                count++;
                j--;
            }
            if (count >= 3)
            {
                for(int k = j; k <= i; k++)
                {
                    area[k, line] = Tile.EmptyTile;
                }
            }
        }

        // Проверка, есть ли по горизонтали 3 и более однотипных тайла и их удаление
        private void Horizontal(Tile tile, int i, int line)
        {
            int j1 = line, j2 = line, count = 1;
            while (j1 > 0 && area[i, j1 - 1] == tile)
            {
                count++;
                j1--;
            }
            while (j2 < width - 1 && area[i, j2 + 1] == tile)
            {
                count++;
                j2++;
            }
            if (count >= 3)
            {
                for (int k = j1; k <= j2; k++)
                {
                    area[i, k] = Tile.EmptyTile;
                }
            }
        }
    }
    
    class Program
    {   
        static void Main(string[] args)
        {
            Game game = new Game(5, 5, 3, 3);
            game.GameSession();
        }
    }
}
