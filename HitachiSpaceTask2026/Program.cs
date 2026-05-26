using System;
using System.Collections.Generic;
using System.Linq;

namespace HitachiSpaceTask2026
{
    public record Position(int Row, int Col);

    public class Astronaut
    {
        public string Name { get; set; }
        public Position StartPoint { get; set; }
        public int Steps { get; set; } = -1;
        public List<Position> PathToStation { get; set; } = new List<Position>();

        public Astronaut(string name, Position startPoint)
        {
            Name = name;
            StartPoint = startPoint;
        }
    }

    class Program
    {
        static void Main()
        {
            try
            {
                Console.Write("Map rows: ");
                if (!int.TryParse(Console.ReadLine(), out int rows) || rows < 2)
                {
                    Console.WriteLine("Грешка: Невалиден брой редове. Трябва да е число >= 2.");
                    return;
                }

                Console.Write("Map columns: ");
                if (!int.TryParse(Console.ReadLine(), out int cols) || cols < 2)
                {
                    Console.WriteLine("Грешка: Невалиден брой колони. Трябва да е число >= 2.");
                    return;
                }

                Console.WriteLine("Cosmic map:");
                string[,] grid = new string[rows, cols];
                List<Astronaut> astronauts = new List<Astronaut>();
                Position station = null;

                for (int r = 0; r < rows; r++)
                {
                    string inputLine = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(inputLine))
                    {
                        Console.WriteLine("Грешка: Въведохте празен ред. Опитайте отново.");
                        return;
                    }

                    string[] cells = inputLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (cells.Length != cols)
                    {
                        Console.WriteLine($"Грешка на ред {r + 1}: Очакваха се {cols} символа, но бяха въведени {cells.Length}.");
                        Console.WriteLine("Моля, въвеждайте символите разделени с интервал (напр. 'O X O S1 F').");
                        return;
                    }

                    for (int c = 0; c < cols; c++)
                    {
                        string cellValue = cells[c].ToUpper();
                        if (cellValue == "0") cellValue = "O";

                        grid[r, c] = cellValue;

                        if (cellValue.StartsWith("S"))
                        {
                            astronauts.Add(new Astronaut(cellValue, new Position(r, c)));
                        }
                        else if (cellValue == "F")
                        {
                            station = new Position(r, c);
                        }
                    }
                }

                if (station == null)
                {
                    Console.WriteLine("Грешка: Не е намерена космическа станция (F) на картата!");
                    return;
                }

                foreach (var astro in astronauts)
                {
                    FindPathBFS(grid, astro, station, rows, cols);
                }

                Console.WriteLine();

                var lostAstronauts = astronauts.Where(a => a.Steps == -1).OrderBy(a => a.Name).ToList();
                foreach (var lost in lostAstronauts)
                {
                    Console.WriteLine($"Mission failed - Astronaut {lost.Name} lost in space!");
                }

                var savedAstronauts = astronauts.Where(a => a.Steps > -1).OrderBy(a => a.Steps).ToList();
                foreach (var saved in savedAstronauts)
                {
                    Console.WriteLine($"Astronaut {saved.Name} - Shortest path: {saved.Steps} steps");
                    PrintMapWithRoute(grid, saved, rows, cols);
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Възникна неочаквана грешка: {ex.Message}");
            }
        }

        static void FindPathBFS(string[,] grid, Astronaut astro, Position target, int rows, int cols)
        {
            int[] dRow = { -1, 1, 0, 0 };
            int[] dCol = { 0, 0, -1, 1 };

            Queue<Position> queue = new Queue<Position>();

            Dictionary<Position, Position> cameFrom = new Dictionary<Position, Position>();

            HashSet<Position> visited = new HashSet<Position>();

            queue.Enqueue(astro.StartPoint);
            visited.Add(astro.StartPoint);

            bool found = false;

            while (queue.Count > 0)
            {
                Position current = queue.Dequeue();

                if (current.Row == target.Row && current.Col == target.Col)
                {
                    found = true;
                    break;
                }

                for (int i = 0; i < 4; i++)
                {
                    int newRow = current.Row + dRow[i];
                    int newCol = current.Col + dCol[i];
                    Position nextMove = new Position(newRow, newCol);

                    if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
                    {
                        if (grid[newRow, newCol] != "X" && !visited.Contains(nextMove))
                        {
                            visited.Add(nextMove);
                            cameFrom[nextMove] = current;
                            queue.Enqueue(nextMove);
                        }
                    }
                }
            }

            if (found)
            {
                Position step = target;
                while (step != astro.StartPoint)
                {
                    astro.PathToStation.Add(step);
                    step = cameFrom[step];
                }

                astro.PathToStation.Add(astro.StartPoint);

                astro.PathToStation.Reverse();

                astro.Steps = astro.PathToStation.Count - 1;
            }
        }

        static void PrintMapWithRoute(string[,] grid, Astronaut astro, int rows, int cols)
        {
            HashSet<Position> route = new HashSet<Position>(astro.PathToStation);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Position currentPos = new Position(r, c);

                    if (currentPos == astro.StartPoint)
                    {
                        Console.Write($"{grid[r, c]} ");
                    }
                    else if (grid[r, c] == "F")
                    {
                        Console.Write("F ");
                    }
                    else if (route.Contains(currentPos))
                    {
                        Console.Write("* ");
                    }
                    else
                    {
                        Console.Write($"{grid[r, c]} ");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}   