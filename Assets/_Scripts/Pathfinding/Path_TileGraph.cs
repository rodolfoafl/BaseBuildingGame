using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph {

    Dictionary<Tile, Path_Node<Tile>> _nodes;

	public Path_TileGraph(World world)
    {
        _nodes = new Dictionary<Tile, Path_Node<Tile>>();

        for (int x = 0; x < world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                Tile tile = world.GetTileAt(x, y);
                if(tile.MovementCost > 0)
                {
                    Path_Node<Tile> node = new Path_Node<Tile>();
                    node.Data = tile;
                    _nodes.Add(tile, node);
                }
            }
        }

        foreach(Tile tile in _nodes.Keys)
        {

        }
    }

}
