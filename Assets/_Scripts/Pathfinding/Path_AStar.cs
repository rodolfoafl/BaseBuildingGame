using Priority_Queue;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path_AStar {

    Queue<Tile> _path;

	public Path_AStar(World world, Tile tileStart, Tile tileEnd)
    {
        if(world.TileGraph == null)
        {
            world.TileGraph = new Path_TileGraph(world);
        }

        Dictionary<Tile, Path_Node<Tile>> nodes = world.TileGraph.Nodes;

        Path_Node<Tile> nodeStart;
        if (!nodes.TryGetValue(tileStart, out nodeStart))
        {
            Debug.LogError("Path_AStar -- the starting tile isn't in the list of tile nodes");
            return;
        }

        Path_Node<Tile> nodeEnd;
        if (!nodes.TryGetValue(tileStart, out nodeEnd))
        {
            Debug.LogError("Path_AStar -- the ending tile isn't in the list of tile nodes");
            return;
        }

        Path_Node<Tile> start = nodes[tileStart];
        Path_Node<Tile> goal = nodes[tileEnd];

        List<Path_Node<Tile>> closedSet = new List<Path_Node<Tile>>();

        /*List<Path_Node<Tile>> openSet = new List<Path_Node<Tile>>();
        openSet.Add(start);*/

        SimplePriorityQueue<Path_Node<Tile>> openSet = new SimplePriorityQueue<Path_Node<Tile>>();
        openSet.Enqueue(start, 0);

        Dictionary<Path_Node<Tile>, Path_Node<Tile>> cameFrom = new Dictionary<Path_Node<Tile>, Path_Node<Tile>>();

        Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float>();
        foreach(Path_Node<Tile> node in nodes.Values)
        {
            g_score[node] = Mathf.Infinity;
        }
        g_score[start] = 0;

        Dictionary<Path_Node<Tile>, float> f_score = new Dictionary<Path_Node<Tile>, float>();
        foreach (Path_Node<Tile> node in nodes.Values)
        {
            f_score[node] = Mathf.Infinity;
        }
        f_score[start] = HeuristicCostEstimate(start, goal);

        while(openSet.Count > 0)
        {
            Path_Node<Tile> current = openSet.Dequeue();
            if(current == goal)
            {
                ReconstructPath(cameFrom, current);
                return;
            }

            closedSet.Add(current);

            foreach(Path_Edge<Tile> neighbour in current.Edges)
            {
                if (closedSet.Contains(neighbour.Node))
                {
                    continue;
                }

                float neighbourMovementCost = neighbour.Cost * DistanceBetween(current, neighbour.Node);
                float tentative_g_score = g_score[current] + neighbourMovementCost;

                if(openSet.Contains(neighbour.Node) && tentative_g_score >= g_score[neighbour.Node])
                {
                    continue;
                }

                cameFrom[neighbour.Node] = current;
                g_score[neighbour.Node] = tentative_g_score;
                f_score[neighbour.Node] = g_score[neighbour.Node] + HeuristicCostEstimate(neighbour.Node, goal);

                if (!openSet.Contains(neighbour.Node))
                {
                    openSet.Enqueue(neighbour.Node, f_score[neighbour.Node]);
                }
                else
                {
                    openSet.UpdatePriority(neighbour.Node, f_score[neighbour.Node]);
                }
            }
        }


    }

    float HeuristicCostEstimate(Path_Node<Tile> a, Path_Node<Tile> b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.Data.X - b.Data.X, 2) + Mathf.Pow(a.Data.Y - b.Data.Y, 2));
    }

    float DistanceBetween(Path_Node<Tile> a, Path_Node<Tile> b)
    {
        if(Mathf.Abs(a.Data.X - b.Data.X) + Mathf.Abs(a.Data.Y - b.Data.Y) == 1)
        {
            return 1f;
        }

        if(Mathf.Abs(a.Data.X - b.Data.X) == 1 && Mathf.Abs(a.Data.Y - b.Data.Y) == 1){
            return 1.414f;
        }

        return Mathf.Sqrt(Mathf.Pow(a.Data.X - b.Data.X, 2) + Mathf.Pow(a.Data.Y - b.Data.Y, 2));
    }

    void ReconstructPath(Dictionary<Path_Node<Tile>, Path_Node<Tile>> cameFrom, Path_Node<Tile> current)
    {
        Queue<Tile> totalPath = new Queue<Tile>();
        totalPath.Enqueue(current.Data);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Enqueue(current.Data);
        }

        _path = new Queue<Tile>(totalPath.Reverse());
    }

    public Tile GetNextTile()
    {
        return _path.Dequeue();
    }

    public int Length()
    {
        if(_path == null)
        {
            return 0;
        }
        return _path.Count;
    }
}
