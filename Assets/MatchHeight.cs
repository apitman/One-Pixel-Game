using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchHeight : MonoBehaviour
{
    public Transform target;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Just track the Y value and match it
        transform.position = new Vector3(transform.position.x, target.transform.position.y, transform.position.z);
    }
}
