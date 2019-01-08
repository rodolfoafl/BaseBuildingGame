using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour {

    World _world;

    bool _buildModeIsObject = false;
    string _buildModeObjectType;
    TileType _buildModeTile = TileType.Floor;    

    #region UI Related Methods
    public void SetMode_BuildFloor()
    {
        _buildModeIsObject = false;
        _buildModeTile = TileType.Floor;
    }

    public void SetMode_Bulldoze()
    {
        _buildModeIsObject = false;
        _buildModeTile = TileType.Empty;
    }

    public void SetMode_BuildInstalledObject(string objectType)
    {
        _buildModeIsObject = true;
        _buildModeObjectType = objectType;
    }

    public void SetupPathfinding()
    {
        _world.SetupPathfindingExample();
    }
    #endregion

    public void Build(Tile tile)
    {
        _world = WorldController.Instance.World;

        if (_buildModeIsObject)
        {
            string installedObjectType = _buildModeObjectType;
            if (_world.IsInstalledObjectPlacementValid(installedObjectType, tile)
                && tile.PendingInstalledObjectJob == null)
            {
                Job newJob;

                if (_world.InstalledObjectJobPrototypes.ContainsKey(installedObjectType))
                {
                    newJob = _world.InstalledObjectJobPrototypes[installedObjectType].Clone();
                    newJob.Tile = tile;
                }
                else
                {
                    Debug.LogError("There is no installedObject job prototype for: " + installedObjectType);
                    newJob = new Job(tile, installedObjectType, InstalledObjectAction.OnInstalledObjectJobCompleted, 0.1f, null);
                }

                tile.PendingInstalledObjectJob = newJob;
                newJob.RegisterJobCancelledCallback((theJob) => { theJob.Tile.PendingInstalledObjectJob = null; });

                _world.JobQueue.Enqueue(newJob);
                //Debug.Log("JobQueue size: " + WorldController.Instance.World.JobQueue.Count);
            }
        }
        else
        {
            tile.Type = _buildModeTile;
        }
    }
}
