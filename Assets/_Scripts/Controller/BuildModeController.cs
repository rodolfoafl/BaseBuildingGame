using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildModeController : MonoBehaviour {

    World _world;

    bool _buildModeIsObject = false;
    string _buildModeObjectType;
    TileType _buildModeTile = TileType.Floor;

    GameObject _installedObjectPreview;
    InstalledObjectSpriteController _iOSC;
    MouseController _mouseController;

    void Start()
    {
        _world = WorldController.Instance.World;

        _iOSC = FindObjectOfType<InstalledObjectSpriteController>();
        _mouseController = FindObjectOfType<MouseController>();

        _installedObjectPreview = new GameObject();
        _installedObjectPreview.transform.SetParent(transform);
        _installedObjectPreview.AddComponent<SpriteRenderer>().sortingLayerName = "Jobs";
        _installedObjectPreview.SetActive(false);
    }

    void Update()
    {
        if(_buildModeIsObject && _buildModeObjectType != null && _buildModeObjectType != "")
        {
            ShowInstalledObjectSpriteAtTile(_buildModeObjectType, _mouseController.GetMouseOverTile());
        }
    }

    public bool IsObjectDraggable()
    {
        if (!_buildModeIsObject)
        {
            return true;
        }

        InstalledObject prototype = _world.InstalledObjectPrototypes[_buildModeObjectType];
        return prototype.LinksToNeighbor;
    }

    void ShowInstalledObjectSpriteAtTile(string objectType, Tile tile)
    {
        _installedObjectPreview.SetActive(true);

        SpriteRenderer renderer = _installedObjectPreview.GetComponent<SpriteRenderer>();
        renderer.sprite = _iOSC.GetSpriteForInstalledObject(objectType);

        if (_world.IsInstalledObjectPlacementValid(_buildModeObjectType, tile))
        {
            renderer.color = new Color(0.5f, 1f, 0.5f, 0.25f);
        }
        else
        {
            renderer.color = new Color(1f, 0.5f, 0.5f, 0.25f);
        }

        InstalledObject prototype = _world.InstalledObjectPrototypes[objectType];
        _installedObjectPreview.transform.position = new Vector3(tile.X + (prototype.Width - 1) / 2f, tile.Y + (prototype.Height - 1) / 2, 0);

    }

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

                newJob.InstalledObjectPrototype = _world.InstalledObjectPrototypes[installedObjectType];

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
