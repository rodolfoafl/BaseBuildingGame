using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    [SerializeField] GameObject _circleCursor;

    Vector3 _lastFramePosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 currFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;

        _circleCursor.transform.position = currFramePosition;

        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 diff = _lastFramePosition - currFramePosition;
            Camera.main.transform.Translate(diff);
        }

        _lastFramePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currFramePosition.z = 0;
    }
}
