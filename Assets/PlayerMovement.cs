using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float runSpeed = 5f;

    private float horizontalMove = 0f;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
    }

    private void FixedUpdate()
    {
        float newXPos = transform.position.x + horizontalMove * Time.fixedDeltaTime;
        transform.position = new Vector3(newXPos, transform.position.y, transform.position.z);
    }
}
