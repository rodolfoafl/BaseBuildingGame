using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    [SerializeField] GameObject _circleCursor;

    Vector3 _lastFramePosition;
    Vector3 _dragStartPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        Vector3 currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        Tile tileUnderMouse = GetTileAtWorldCoordinate(currFramePosition);
        if(tileUnderMouse != null)
        {
            _circleCursor.SetActive(true);
            Vector3 cursorPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y);
            _circleCursor.transform.position = cursorPosition;
        }
        else
        {
            _circleCursor.SetActive(false);
        }

        //Start mouse drag
        if (Input.GetMouseButtonDown(0))
        {
            _dragStartPosition = currFramePosition;
        }

        //End mouse drag
        if (Input.GetMouseButtonUp(0))
        {
            int start_x = Mathf.FloorToInt(_dragStartPosition.x);
            int end_x = Mathf.FloorToInt(currFramePosition.x);
            if(end_x < start_x)
            {
                int tmp = end_x;
                end_x = start_x;
                start_x = tmp;
            }

            int start_y = Mathf.FloorToInt(_dragStartPosition.y);
            int end_y = Mathf.FloorToInt(currFramePosition.y);
            if (end_y < start_y)
            {
                int tmp = end_y;
                end_y = start_y;
                start_y = tmp;
            }

            for (int x = start_x; x <= end_x; x++)
            {
                for (int y = start_y; y <= end_y; y++)
                {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if(t != null)
                    {
                        t.Type = Tile.TileType.Floor;
                    }
                }
            }
        }

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 diff = _lastFramePosition - currFramePosition;
            Camera.main.transform.Translate(diff);
        }

        _lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _lastFramePosition.z = 0;
        //currFramePosition.z = 0;
    }

    Tile GetTileAtWorldCoordinate(Vector3 coordinate)
    {
        int x = Mathf.FloorToInt(coordinate.x);
        int y = Mathf.FloorToInt(coordinate.y);

        return WorldController.Instance.World.GetTileAt(x, y);
    }
}
