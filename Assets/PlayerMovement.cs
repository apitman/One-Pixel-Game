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
    public float minXPos = -16f;
    public float yOffsetForText = 180f;

    private float horizontalMove = 0f;
    private float desiredRotation = 0f;
    private int activeRotations = 0;
    private bool playerHasMovedEver = false;
    private bool text1Created = false; // Don't @ me for these text*Created Booleans
    private bool textLeftBoundCreated = false;
    private bool textHillCreated = false;
    private bool textSteeperCreated = false;
    private bool textSteepestCreated = false;
    private bool textMadeItCreated = false;
    private bool textDownhillCreated = false;
    private bool textColorCreated = false;
    private bool textCloseCreated = false;
    private bool textCongratulationsCreated = false;

    private void Start()
    {
        StartCoroutine(spawnText(0, "One Pixel", new Vector3(0, yOffsetForText, 0)));
    }
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
        // Text creation code - I know this is all very hardcoded and hacky, but it's a 48 hour game jam and I don't have time to make it better
        if (playerHasMovedEver && !text1Created)
        {
            StartCoroutine(spawnText(0, "I don't belong here."));
            StartCoroutine(spawnText(chainTextDelay, "I need to find where I belong."));
            text1Created = true;
        } else if (!textHillCreated && desiredRotation > 0f)
        {
            StartCoroutine(spawnText(0, "A hill?"));
            textHillCreated = true;
        } else if (textHillCreated && !textSteeperCreated && desiredRotation > 31f)
        {
            StartCoroutine(spawnText(0, "I'm not sure if I can make it."));
            textSteeperCreated = true;
        }
        else if (textSteeperCreated && !textSteepestCreated && desiredRotation > 46f)
        {
            StartCoroutine(spawnText(0, "So steep!"));
            textSteepestCreated = true;
        }
        else if (textSteepestCreated && !textMadeItCreated && desiredRotation < 10f && transform.position.x > 5f)
        {
            StartCoroutine(spawnText(0, "Phew, the top flattens out."));
            textMadeItCreated = true;
        }
        else if (textSteepestCreated && !textColorCreated && transform.position.x >= 11.5f)
        {
            StartCoroutine(spawnText(0, "What are these colors?"));
            textColorCreated = true;
        } else if (textColorCreated && !textCloseCreated && transform.position.x >= 18f)
        {
            StartCoroutine(spawnText(0, "I think I'm close to where I belong."));
            textCloseCreated = true;
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
        if (newXPos <= minXPos && !textLeftBoundCreated)
        {
            if (!textLeftBoundCreated)
            {
                StartCoroutine(spawnText(0, "This direction feels wrong."));
                StartCoroutine(spawnText(chainTextDelay, "Moving to the right feels better."));
                textLeftBoundCreated = true;
            }
            newXPos = minXPos; // Block the player from moving any farther left
        } else if (!textDownhillCreated && FindObjectsOfType<Text>().Length == 0 && newXPos < transform.position.x && desiredRotation > 0f)
        {
            StartCoroutine(spawnText(0, "I can't give up."));
            textDownhillCreated = true;
        }
        transform.position = new Vector3(newXPos, newYPos, transform.position.z);
        if (!textCongratulationsCreated && newXPos >= maxXPos)
        {
            controlsEnabled = false;
            StartCoroutine(spawnText(0, "Congratulations!", new Vector3(0, yOffsetForText, 0)));
            textCongratulationsCreated = true;
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
