﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float runSpeed = 5f;
    public float rotationSpeed = 5f;
    public float maxXPos = 22.05f;

    private float horizontalMove = 0f;
    private float desiredRotation = 0f;
    private int activeRotations = 0;

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
    }

    private void FixedUpdate()
    {
        float verticalMove = 0f;
        if (activeRotations > 0)
        {
            float vertRatio = desiredRotation / 90f;
            verticalMove = horizontalMove * (vertRatio);
            horizontalMove *= 1 - vertRatio;
        }
        float newXPos = Mathf.Min(maxXPos, transform.position.x + horizontalMove * Time.fixedDeltaTime);
        float newYPos = transform.position.y + verticalMove * Time.fixedDeltaTime;
        transform.position = new Vector3(newXPos, newYPos, transform.position.z);

        // Note: This is apparently not the right way to do rotations in general, but it works for me around 1 axis so there
        float currentRotation = transform.rotation.eulerAngles.z;
        float desiredRotationRemaining = desiredRotation - currentRotation;
        float smoothRotation = desiredRotationRemaining * rotationSpeed * Time.deltaTime;
        if (Mathf.Abs(desiredRotationRemaining) > 0.01f) // Give up once we get close enough
        {
            // Debug.Log("Current rotation is " + currentRotation + " so we are rotating " + smoothRotation);
            transform.Rotate(0f, 0f, smoothRotation);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("rotationEffect"))
        {
            activeRotations++;
            Debug.Log("ActiveRotations =" + activeRotations + "\nCollided with a rotationEffect and using this as desired rotation amount:" + collision.gameObject.GetComponent<HillRotation>().rotationAmount);
            desiredRotation = collision.gameObject.GetComponent<HillRotation>().rotationAmount;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("rotationEffect"))
        {
            Debug.Log("Leaving collision zone");
            activeRotations--;
            if (activeRotations == 0)
            {
                Debug.Log("Setting desiredRotation to 0");
                desiredRotation = 0f;
            }
        }
    }
}
