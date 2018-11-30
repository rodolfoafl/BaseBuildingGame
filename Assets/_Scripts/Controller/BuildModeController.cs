using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour {

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
    #endregion

    public void Build(Tile tile)
    {
        if (_buildModeIsObject)
        {
            string installedObjectType = _buildModeObjectType;
            if (WorldController.Instance.World.IsInstalledObjectPlacementValid(installedObjectType, tile)
                && tile.PendingInstalledObjectJob == null)
            {
                Job newJob = new Job(tile, (theJob) => {
                    OnInstalledObjectJobCompleted(installedObjectType, theJob.Tile);
                    tile.PendingInstalledObjectJob = null;
                });

                tile.PendingInstalledObjectJob = newJob;
                newJob.RegisterJobCancelledCallback((theJob) => { theJob.Tile.PendingInstalledObjectJob = null; });

                WorldController.Instance.World.JobQueue.Enqueue(newJob);
                Debug.Log("JobQueue size: " + WorldController.Instance.World.JobQueue.Count);
            }
        }
        else
        {
            tile.Type = _buildModeTile;
        }
    }

    void OnInstalledObjectJobCompleted(string objectType, Tile tile)
    {
        WorldController.Instance.World.PlaceInstalledObject(_buildModeObjectType, tile);
    }

}
