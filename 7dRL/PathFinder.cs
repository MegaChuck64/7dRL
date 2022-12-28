using _7dRL.RoomCreation;
using AStar;
using AStar.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dRL;

public class PathFinder
{
    public Map Map { get; set; }
    public List<(int x, int y)> Path { get; set; }
    public int CurrentStep { get; set; }

    public PathFinder(Map map)
    {
        Map = map;
    }

    public void Clear()
    {
        Path = new List<(int x, int y)>();
    }
    public void CreatePath(int startX, int startY, int endX, int endY, bool useDiagonals = false, bool punishDirectionChange = true)
    {
        var pathFinderOptions = new PathFinderOptions
        {
            PunishChangeDirection = punishDirectionChange,
            UseDiagonals = useDiagonals,
        };

        var tiles = new short[Map.Width, Map.Height];
        for (int x = 0; x < Map.Width; x++)
        {
            for (int y = 0; y < Map.Height; y++)
            {
                tiles[x, y] = ((Map.GetTile(x, y)?.HasCollider ?? false) || Map.ObjectTiles[(x, y)].HasCollider) ? (short)0 : (short)1;
            }
        }

        var grid = new WorldGrid(tiles);
        var pathfinder = new AStar.PathFinder(grid, pathFinderOptions);

        var path = pathfinder.FindPath(new Position(startX, startY), new Position(endX, endY))
            .Select(p => (p.Row, p.Column))
            .ToList();

        CurrentStep = 0;
        Path = path ?? Path;
    }

    public bool TryStepForward(out (int x, int y)? newPosition)
    {
        if (IsEnd())
        {
            if (Path.Count > 0) newPosition = Path.Last();
            else newPosition = null;
            return false;
        }
        else
        {
            CurrentStep++;
            if (Path.Count > CurrentStep)
            {
                newPosition = Path[CurrentStep];
                return true;
            }
            else
            {
                newPosition = null;
                return false;
            }
        }
    }


    public bool IsEnd()
    {
        if (Path.Count > 0)
        {
            if (CurrentStep >= Path.Count - 1)
            {
                return true;
            }
        }

        return false;
    }
}
