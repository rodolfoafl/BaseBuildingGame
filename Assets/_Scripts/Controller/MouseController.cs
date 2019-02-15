using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

    [SerializeField] Transform _cursorGameObjectsContainer;

    [SerializeField] GameObject _circleCursorPrefab;

    [SerializeField] float _mouseZoomInMax;
    [SerializeField] float _mouseZoomOutMax;

    Vector3 _lastFramePosition;
    Vector3 _currFramePosition;
    Vector3 _dragStartPosition;

    List<GameObject> _dragPreviewGameObjects;

    BuildModeController _buildModeController;
    InstalledObjectSpriteController _iOSC;
    MouseController _mouseController;

    World _world;

    bool _isDragging = false;

    enum MouseMode
    {
        SELECT, BUILD
    }

    MouseMode _currentMode = MouseMode.SELECT;

    void Start()
    {
        _world = WorldController.Instance.World;

        _dragPreviewGameObjects = new List<GameObject>();
        _buildModeController = FindObjectOfType<BuildModeController>();

        _iOSC = FindObjectOfType<InstalledObjectSpriteController>();
        _mouseController = FindObjectOfType<MouseController>();
    }

    void LateUpdate()
    {
        _currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _currFramePosition.z = 0;

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (_currentMode == MouseMode.BUILD)
            {
                _currentMode = MouseMode.SELECT;
            }
            else if (_currentMode == MouseMode.SELECT)
            {
                Debug.Log("Show game menu!");
            }
        }

        UpdateDragging();
        UpdateCameraMovement();

        _lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _lastFramePosition.z = 0;
    }

    public void StartBuildMode()
    {
        _currentMode = MouseMode.BUILD;
    }

    public Vector3 GetMousePosition()
    {
        return _currFramePosition;
    }

    public Tile GetMouseOverTile()
    {
        return WorldController.Instance.GetTileAtWorldCoordinate(_currFramePosition);
    }

    void ShowInstalledObjectSpriteAtTile(string objectType, Tile tile)
    {
        GameObject go = new GameObject();
        go.transform.SetParent(_cursorGameObjectsContainer, true);
        _dragPreviewGameObjects.Add(go);

        SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
        renderer.sprite = _iOSC.GetSpriteForInstalledObject(objectType);
        renderer.sortingLayerName = "Jobs";

        if (_world.IsInstalledObjectPlacementValid(objectType, tile))
        {
            renderer.color = new Color(0.5f, 1f, 0.5f, 0.25f);
        }
        else
        {
            renderer.color = new Color(1f, 0.5f, 0.5f, 0.25f);
        }

        InstalledObject prototype = _world.InstalledObjectPrototypes[objectType];
        go.transform.position = new Vector3(tile.X + (prototype.Width - 1) / 2f, tile.Y + (prototype.Height - 1) / 2, 0);

    }

    #region Mouse Related Methods
    void UpdateDragging()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //Clean old drag previews
        for (int i = 0; i < _dragPreviewGameObjects.Count; i++)
        {
            SimplePool.Despawn(_dragPreviewGameObjects[i]);
        }
        _dragPreviewGameObjects.Clear();

        if(_currentMode != MouseMode.BUILD)
        {
            return;
        }

        //Start mouse drag
        if (Input.GetMouseButtonDown(0))
        {
            _dragStartPosition = _currFramePosition;
            _isDragging = true;
        }
        else if(!_isDragging)
        {
            _dragStartPosition = _currFramePosition;
        }

        if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Escape))
        {
            _isDragging = false;
        }

        if (!_buildModeController.IsObjectDraggable())
        {
            _dragStartPosition = _currFramePosition;
        }

        int start_x = Mathf.RoundToInt(_dragStartPosition.x);
        int end_x = Mathf.RoundToInt(_currFramePosition.x);
        if (end_x < start_x)
        {
            int tmp = end_x;
            end_x = start_x;
            start_x = tmp;
        }

        int start_y = Mathf.RoundToInt(_dragStartPosition.y);
        int end_y = Mathf.RoundToInt(_currFramePosition.y);
        if (end_y < start_y)
        {
            int tmp = end_y;
            end_y = start_y;
            start_y = tmp;
        }

        /*while (_dragPreviewGameObjects.Count > 0)
        {
            GameObject go = _dragPreviewGameObjects[0];
            _dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }*/

        /*if (_isDragging)
        {*/
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null)
                    {
                        if (_buildModeController.BuildModeIsObject)
                        {
                            ShowInstalledObjectSpriteAtTile(_buildModeController.BuildModeObjectType, t);
                        }
                        else
                        {
                            GameObject go = SimplePool.Spawn(_circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                            go.transform.SetParent(_cursorGameObjectsContainer, true);
                            _dragPreviewGameObjects.Add(go);
                        }
                    }
                }
            }
        /*}*/

        //End mouse drag
        if (_isDragging && Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile tile = WorldController.Instance.World.GetTileAt(x, y);

                    if (tile != null)
                    {
                        _buildModeController.Build(tile);
                    }
                }
            }
        }
    }

    void UpdateCameraMovement()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 diff = _lastFramePosition - _currFramePosition;
            Camera.main.transform.Translate(diff);
        }

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize -= Input.mouseScrollDelta.y, _mouseZoomInMax, _mouseZoomOutMax);
    }
    #endregion
}
