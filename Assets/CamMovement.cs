using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 10f;
    public Vector3 finalOffset;
    public float zoomTime = 3f;
    public float finalZoom = 10f;

    private bool endMode = false;
    private float endModeStartTime = 0f;
    private void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        if (endMode) {
            desiredPosition = target.position + offset + finalOffset;
            float percentDoneWithZoom = Mathf.Min(1f, (Time.fixedTime - endModeStartTime) / (zoomTime));
            // Debug.Log(" endModeStartTime = " + endModeStartTime + " Time.fixedTime = " + Time.fixedTime + " percentageDone = " + percentDoneWithZoom);
            GetComponent<Camera>().orthographicSize = finalZoom * percentDoneWithZoom + GetComponent<Camera>().orthographicSize * (1 - percentDoneWithZoom);
        } else if (target.position.x >= target.GetComponent<PlayerMovement>().maxXPos)
        {
            endMode = true;
            endModeStartTime = Time.fixedTime;
        }
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime); // Might want different smoothSpeed for final zoom out
        transform.position = smoothedPosition;
    }
}
