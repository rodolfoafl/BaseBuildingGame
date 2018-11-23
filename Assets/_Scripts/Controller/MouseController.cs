using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {

    [SerializeField] Transform _cursorGameObjectsContainer;
    [SerializeField] GameObject _circleCursorPrefab;
    [SerializeField] float _mouseZoomInMax;
    [SerializeField] float _mouseZoomOutMax;

    bool _buildModeIsObject = false;
    TileType _buildModeTile = TileType.Floor;
    string _buildModeObjectType;
    Vector3 _lastFramePosition;
    Vector3 _currFramePosition;
    Vector3 _dragStartPosition;
    List<GameObject> _dragPreviewGameObjects;

    void Start()
    {
        _dragPreviewGameObjects = new List<GameObject>();
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        _currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _currFramePosition.z = 0;

        //UpdateCursor();
        UpdateDragging();
        UpdateCameraMovement();

        _lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _lastFramePosition.z = 0;
    }

    void UpdateDragging()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //Start mouse drag
        if (Input.GetMouseButtonDown(0))
        {
            _dragStartPosition = _currFramePosition;
        }

        int start_x = Mathf.FloorToInt(_dragStartPosition.x);
        int end_x = Mathf.FloorToInt(_currFramePosition.x);
        if (end_x < start_x)
        {
            int tmp = end_x;
            end_x = start_x;
            start_x = tmp;
        }

        int start_y = Mathf.FloorToInt(_dragStartPosition.y);
        int end_y = Mathf.FloorToInt(_currFramePosition.y);
        if (end_y < start_y)
        {
            int tmp = end_y;
            end_y = start_y;
            start_y = tmp;
        }

        //Clean old drag previews
        for (int i = 0; i < _dragPreviewGameObjects.Count; i++)
        {
            SimplePool.Despawn(_dragPreviewGameObjects[i]);
        }
        _dragPreviewGameObjects.Clear();

        /*while (_dragPreviewGameObjects.Count > 0)
        {
            GameObject go = _dragPreviewGameObjects[0];
            _dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }*/

        if (Input.GetMouseButton(0))
        {
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null)
                    {
                        GameObject go = SimplePool.Spawn(_circleCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        go.transform.SetParent(_cursorGameObjectsContainer, true);
                        _dragPreviewGameObjects.Add(go);
                    }
                }
            }
        }

        //End mouse drag
        if (Input.GetMouseButtonUp(0))
        {          
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);

                    if (t != null)
                    {
                        if (_buildModeIsObject)
                        {
                            WorldController.Instance.World.PlaceInstalledObject(_buildModeObjectType, t);
                        }
                        else
                        {
                            t.Type = _buildModeTile;
                        }
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

    /*void UpdateCursor()
    {
        Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoordinate(_currFramePosition);
        if (tileUnderMouse != null)
        {
            _circleCursor.SetActive(true);
            Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y);
            _circleCursor.transform.position = cursorPosition;
        }
        else
        {
            _circleCursor.SetActive(false);
        }
    }*/

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
}
