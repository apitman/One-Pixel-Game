using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float runSpeed = 5f;
    public float rotationSpeed = 5f;
    public float maxXPos = 22.05f;
    public bool controlsEnabled = true;
    public GameObject textPrefab;
    public float chainTextDelay = 4f;

    private float horizontalMove = 0f;
    private float desiredRotation = 0f;
    private int activeRotations = 0;
    private bool playerHasMovedEver = false;
    private bool text1Created = false;

    // Update is called once per frame
    void Update()
    {
        if (controlsEnabled)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            if (horizontalMove != 0f)
            {
                playerHasMovedEver = true;
            }
        } else
        {
            horizontalMove = 0f;
        }
    }

    private void FixedUpdate()
    {
        // Text creation code
        if (playerHasMovedEver && !text1Created)
        {
            StartCoroutine(spawnText(0, "I don't belong here."));
            StartCoroutine(spawnText(chainTextDelay, "I need to find where I belong."));
            text1Created = true;
        }

        // Movement code
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
        if (newXPos >= maxXPos)
        {
            controlsEnabled = false;
        }

        // Rotation code
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

    private IEnumerator spawnText(float delay, string message, Vector3? inputLocation = null)
    {
        Vector3 location = inputLocation ?? Vector3.zero;

        yield return new WaitForSeconds(delay);

        GameObject text = Instantiate(textPrefab, location, Quaternion.identity);
        text.transform.SetParent(GameObject.Find("Canvas").transform, false);
        text.GetComponent<Text>().text = message;

        yield return null;
    }
}
