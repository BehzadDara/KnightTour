/*for (int i = 0; i < 8; i++)
    for (int j = i; j < 8; j++)
    {
        new Chess().StartSolving(i, j);
    }*/
new Chess().StartSolving(0, 0);


class Chess
{
    public Chess()
    {
        for (int i = 0; i < 64; i++)
        {
            var point = new Point
            {
                PointCore = new PointCore
                {
                    X = i % 8,
                    Y = i / 8
                }
            };
            point.FillNeighbors();
            Points.Add(point);
        }
    }

    public List<Point> Points { get; set; } = new();
    public bool IsDone { get; set; } = false;
    public int OriginalX { get; set; }
    public int OriginalY { get; set; }

    public void StartSolving(int x, int y)
    {
        Console.WriteLine($"search for x = {x}, y = {y}");
        OriginalX = x;
        OriginalY = y;
        SearchForSolution(x, y, 0, Points);
    }

    public List<Point> SearchForSolution(int x, int y, int publishCheckOrder, List<Point> points)
    {
        var point = points.FirstOrDefault(a => a.PointCore.X == x && a.PointCore.Y == y);
        if (point is null) throw new Exception();
        point.Check(publishCheckOrder++);

        var nexts = ChooseNexts(point, points);
        if (nexts.Count == 0)
        {
            point.UnCheck();
            publishCheckOrder--;
            return points;
        }

        foreach(var next in nexts)
        {
            points = SearchForSolution(next.PointCore.X, next.PointCore.Y, publishCheckOrder, points);

            if (!points.Where(a => a.CheckOrder is null).Any() && !IsDone)
            {
                IsDone = true;
                Console.WriteLine($"with x = {OriginalX}, y = {OriginalY} problem solved");
                PrintSolution(points);
                break;
            }
            continue;
        }
        point.UnCheck();
        publishCheckOrder--;
        return points;
    }

    public static List<Point> ChooseNexts(Point point, List<Point> points)
    {
        var neighbors = new List<Point>();
        foreach (var neighbor in point.Neighbors)
        {
            var neighborFullInfo = points.FirstOrDefault(a => a.PointCore.X == neighbor.X && a.PointCore.Y == neighbor.Y && a.CheckOrder is null);
            if (neighborFullInfo is not null) neighbors.Add(neighborFullInfo);
        }
        //return neighbors.Where(a => a.NumberOfNotCheckedNeighbors == neighbors.Min(b => b.NumberOfNotCheckedNeighbors)).ToList();
        return neighbors.OrderBy(a => a.NumberOfNotCheckedNeighbors).ToList()?? new List<Point>();
    }

    public static void PrintSolution(List<Point> points)
    {
        Console.WriteLine("------------------------------------------------------------");
        for (int i = 0; i < 64; i++)
        {
            Console.Write($"{points[i].CheckOrder}     ");
            if (i % 8 == 7) Console.WriteLine();
        }
        Console.WriteLine("------------------------------------------------------------");
    }

}

class Point
{
    public PointCore PointCore { get; set; } = new();
    public int? CheckOrder { get; set; } = null;
    public List<PointCore> Neighbors { get; set; } = new();
    public int NumberOfNotCheckedNeighbors { get; set; } = 0;

    public void FillNeighbors()
    {
        var x = PointCore.X;
        var y = PointCore.Y;

        if (x + 2 < 8)
        {
            if (y + 1 < 8) Neighbors.Add(new PointCore { X = x + 2, Y = y + 1 });
            if (y - 1 >= 0) Neighbors.Add(new PointCore { X = x + 2, Y = y - 1 });
        }

        if (x + 1 < 8)
        {
            if (y + 2 < 8) Neighbors.Add(new PointCore { X = x + 1, Y = y + 2 });
            if (y - 2 >= 0) Neighbors.Add(new PointCore { X = x + 1, Y = y - 2 });
        }

        if (x - 2 >= 0)
        {
            if (y + 1 < 8) Neighbors.Add(new PointCore { X = x - 2, Y = y + 1 });
            if (y - 1 >= 0) Neighbors.Add(new PointCore { X = x - 2, Y = y - 1 });
        }

        if (x - 1 >= 0)
        {
            if (y + 2 < 8) Neighbors.Add(new PointCore { X = x - 1, Y = y + 2 });
            if (y - 2 >= 0) Neighbors.Add(new PointCore { X = x - 1, Y = y - 2 });
        }

        NumberOfNotCheckedNeighbors = Neighbors.Count;
    }

    public void Check(int checkOrder)
    {
        CheckOrder = checkOrder;
    }

    public void UnCheck()
    {
        CheckOrder = null;
    }
}

class PointCore
{
    public int X { get; set; }
    public int Y { get; set; }
}