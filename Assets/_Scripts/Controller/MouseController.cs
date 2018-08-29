using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    [SerializeField] GameObject cursorPrefab;
    [SerializeField] float maxZoomIn;
    [SerializeField] float maxZoomOut;

    Vector3 lastFramePosition;
    Vector3 dragStartPosition;
    Vector3 currentFramePosition;

    List<GameObject> dragPreviewGameObjects;
    // Use this for initialization
    void Start () {
        dragPreviewGameObjects = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        currentFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentFramePosition.z = 0;

        UpdateDragging();
        UpdateCameraMovement();

        lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastFramePosition.z = 0;
    }

    /*void UpdateCursor()
    {
        Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoordinate(currentFramePosition);
        if (tileUnderMouse != null)
        {
            cursorPrefab.SetActive(true);
            Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
            cursorPrefab.transform.position = cursorPosition;
        }
        else
        {
            cursorPrefab.SetActive(false);
        }
    }*/

    void UpdateCameraMovement()
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 diff = lastFramePosition - currentFramePosition;
            Camera.main.transform.Translate(diff);
        }

        //Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize -= Input.mouseScrollDelta.y, maxZoomIn, maxZoomOut);
    }

    void UpdateDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = currentFramePosition;
        }

        int start_x = Mathf.FloorToInt(dragStartPosition.x);
        int end_x = Mathf.FloorToInt(currentFramePosition.x);
        int start_y = Mathf.CeilToInt(dragStartPosition.y);
        int end_y = Mathf.CeilToInt(currentFramePosition.y);

        if (end_x < start_x)
        {
            int temp = end_x;
            end_x = start_x;
            start_x = temp;
        }

        if (end_y < start_y)
        {
            int temp = end_y;
            end_y = start_y;
            start_y = temp;
        }
        
        while (dragPreviewGameObjects.Count > 0)
        {
            GameObject go = dragPreviewGameObjects[0];
            dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }

        if (Input.GetMouseButton(0))
        {
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null)
                    {
                        //t.Type = Tile.TileType.Floor;
                        GameObject go = SimplePool.Spawn(cursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        go.transform.SetParent(transform, true);
                        dragPreviewGameObjects.Add(go);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null)
                    {
                        t.Type = Tile.TileType.Floor;
                    }
                }
            }
        }
    }
}
