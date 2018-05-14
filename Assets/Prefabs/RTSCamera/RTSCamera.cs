using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCamera : MonoBehaviour {

    // Camera gestion
    public float zOffset;
    public float yOffset;
    public float xRotOffset;
    [SerializeField] private float minXCameraPosition = 75;
    [SerializeField] private float maxXCameraPosition = 225;
    [SerializeField] private float minZCameraPosition = 23;
    [SerializeField] private float maxZCameraPosition = 100;

    [SerializeField] private float minZoom = 20;
    [SerializeField] private float maxZoom = 60;
    [SerializeField] private float scrollingRate = 0.3f;

    private Camera RTScamera;

    // Use this for initialization
    void Awake () {
        RTScamera = GetComponent<Camera>();

        zOffset = -5f;
        yOffset = 12f;
        xRotOffset = 50f;
    }
	
	// Update is called once per frame
	void Update () {
        // Translate with scroll wheel button
        if (Input.GetButton("Mouse 2 X") || Input.GetButton("Mouse 2 Y"))
        {

            // Snap the camera to the terrain
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + Input.GetAxis("Mouse 2 X") * Time.deltaTime, 
                                                         minXCameraPosition - minXCameraPosition * (1 - RTScamera.orthographicSize / maxZoom),
                                                         maxXCameraPosition + minXCameraPosition * (1 - RTScamera.orthographicSize / maxZoom)),
                                            transform.position.y,
                                            Mathf.Clamp(transform.position.z + Input.GetAxis("Mouse 2 Y") * Time.deltaTime,
                                                         minZCameraPosition - 44 * (1 - RTScamera.orthographicSize / maxZoom),
                                                         maxZCameraPosition + 44 * (1 - RTScamera.orthographicSize / maxZoom)));
        }

        else
        {
            // Move right
            if (Input.mousePosition.x >= Screen.width - 5f || Input.GetAxis("Horizontal") > 0)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x + scrollingRate,
                                                             minXCameraPosition - minXCameraPosition * (1 - RTScamera.orthographicSize / maxZoom),
                                                             maxXCameraPosition + minXCameraPosition * (1 - RTScamera.orthographicSize / maxZoom)),
                                                transform.position.y,
                                                transform.position.z);
            }

            // Move left
            if (Input.mousePosition.x <= 0 + 5f || Input.GetAxis("Horizontal") < 0)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x - scrollingRate, 
                                                             minXCameraPosition - minXCameraPosition * (1 - RTScamera.orthographicSize / maxZoom),
                                                             maxXCameraPosition + minXCameraPosition * (1 - RTScamera.orthographicSize / maxZoom)),
                                                transform.position.y,
                                                transform.position.z);
            }

            // Move up
            if (Input.mousePosition.y >= Screen.height - 5f)
            {
                transform.position = new Vector3(transform.position.x,
                                               transform.position.y,
                                               Mathf.Clamp(transform.position.z + scrollingRate,
                                                           minZCameraPosition - 44 * (1 - RTScamera.orthographicSize / maxZoom),
                                                           maxZCameraPosition + 44 * (1 - RTScamera.orthographicSize / maxZoom)));
            }

            // Move down
            if (Input.mousePosition.y <= 0 + 5f)
            {
                transform.position = new Vector3(transform.position.x,
                                                transform.position.y,
                                                Mathf.Clamp(transform.position.z - scrollingRate,
                                                            minZCameraPosition - 44 * (1 - RTScamera.orthographicSize / maxZoom),
                                                            maxZCameraPosition + 44 * (1 - RTScamera.orthographicSize / maxZoom)));
            }
        }
        // Zoom / Dezoom
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(RTScamera.orthographicSize > minZoom)
                RTScamera.orthographicSize -= 5;

            // Snap the camera to the terrain
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + Input.GetAxis("Mouse 2 X") * Time.deltaTime,
                                                         minXCameraPosition - minXCameraPosition * (1 - RTScamera.orthographicSize / maxZoom),
                                                         maxXCameraPosition + minXCameraPosition * (1 - RTScamera.orthographicSize / maxZoom)),
                                            transform.position.y,
                                            Mathf.Clamp(transform.position.z + Input.GetAxis("Mouse 2 Y") * Time.deltaTime,
                                                         minZCameraPosition - 44 * (1 - RTScamera.orthographicSize / maxZoom),
                                                         maxZCameraPosition + 44 * (1 - RTScamera.orthographicSize / maxZoom)));
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (RTScamera.orthographicSize < maxZoom)
                RTScamera.orthographicSize += 5;

            // Snap the camera to the terrain
            transform.position = new Vector3(Mathf.Clamp(transform.position.x + Input.GetAxis("Mouse 2 X") * Time.deltaTime,
                                                         minXCameraPosition - minXCameraPosition * (1 - RTScamera.orthographicSize / maxZoom),
                                                         maxXCameraPosition + minXCameraPosition * (1 - RTScamera.orthographicSize / maxZoom)),
                                            transform.position.y,
                                            Mathf.Clamp(transform.position.z + Input.GetAxis("Mouse 2 Y") * Time.deltaTime,
                                                         minZCameraPosition - 44 * (1 - RTScamera.orthographicSize / maxZoom),
                                                         maxZCameraPosition + 44 * (1 - RTScamera.orthographicSize / maxZoom)));
        }

    }
}
