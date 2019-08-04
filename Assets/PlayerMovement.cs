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
    public GameObject textPanelPrefab;
    public float chainTextDelay = 4f;
    public float minXPos = -16f;
    public float yOffsetForText = 180f;

    private float horizontalMove = 0f;
    private float desiredRotation = 0f;
    private int activeRotations = 0;
    private bool playerHasMovedEver = false;
    private bool text1Created = false; // Don't @ me for these text*Created Booleans
    private bool textLeftBoundCreated = false;
    private bool textBoringStretchCreated = false;
    private bool textHillCreated = false;
    private bool textSteeperCreated = false;
    private bool textSteepestCreated = false;
    private bool textMadeItCreated = false;
    private bool textUpperLeftBoundCreated = false;
    private bool textDownhillCreated = false;
    private bool textColorCreated = false;
    private bool textCloseCreated = false;
    private bool textCongratulationsCreated = false;

    private void Start()
    {
        StartCoroutine(spawnText(0, "One Pixel", 72, false, Color.cyan, new Vector2(800, 200), new Vector3(0, yOffsetForText, 0))); // TODO: Probably change the size or color here
        controlsEnabled = false;
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
        bool canShowText = FindObjectsOfType<Text>().Length == 0;
        if (canShowText && transform.position.x < maxXPos - 1 && Time.fixedTime > 1f)
        {
            // Enable the controls once the title fades away
            controlsEnabled = true;
        }

        // Text creation code - I know this is all very hardcoded and hacky, but it's a 48 hour game jam and I don't have time to make it better
        if (canShowText && playerHasMovedEver && !text1Created)
        {
            StartCoroutine(spawnText(0, "I don't belong here.", inputRectSizeDelta: new Vector2(300, 50)));
            StartCoroutine(spawnText(chainTextDelay, "I need to find where I belong."));
            text1Created = true;
        } else if (canShowText && textLeftBoundCreated && !textBoringStretchCreated && transform.position.x > -7 && transform.position.x < -5) {
            StartCoroutine(spawnText(0, "This empty void is so unsettling.", inputRectSizeDelta: new Vector2(450, 50)));
            StartCoroutine(spawnText(chainTextDelay, "I just want to go home.", inputRectSizeDelta: new Vector2(320, 50)));
            textBoringStretchCreated = true;
        } else if (canShowText && !textHillCreated && desiredRotation > 0f)
        {
            StartCoroutine(spawnText(0, "A hill?", inputRectSizeDelta: new Vector2(100, 50)));
            textHillCreated = true;
        } else if (canShowText && textHillCreated && !textSteeperCreated && desiredRotation > 31f)
        {
            StartCoroutine(spawnText(0, "I'm not sure if I can make it.", inputRectSizeDelta: new Vector2(380, 50)));
            textSteeperCreated = true;
        }
        else if (canShowText && textSteeperCreated && !textSteepestCreated && desiredRotation > 46f)
        {
            StartCoroutine(spawnText(0, "So steep!", inputRectSizeDelta: new Vector2(150, 50)));
            textSteepestCreated = true;
        }
        else if (canShowText && textSteepestCreated && !textMadeItCreated && desiredRotation < 10f && transform.position.x > 5f)
        {
            StartCoroutine(spawnText(0, "Phew, the top flattens out.", inputRectSizeDelta: new Vector2(350, 50)));
            textMadeItCreated = true;
        }
        else if (canShowText && textSteepestCreated && !textColorCreated && transform.position.x >= 11.5f && FindObjectsOfType<Text>().Length == 0)
        {
            StartCoroutine(spawnText(0, "Have I escaped the void?", inputRectSizeDelta: new Vector2(340, 50)));
            textColorCreated = true;
        } else if (canShowText && textColorCreated && !textCloseCreated && transform.position.x >= 18f)
        {
            StartCoroutine(spawnText(0, "I think I'm close to where I belong.", inputRectSizeDelta: new Vector2(450, 50)));
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
        if (newXPos <= minXPos)
        {
            if (canShowText && !textLeftBoundCreated)
            {
                StartCoroutine(spawnText(0, "This direction feels wrong.", inputRectSizeDelta: new Vector2(350, 50)));
                StartCoroutine(spawnText(chainTextDelay, "Moving to the right feels better.", inputRectSizeDelta: new Vector2(420, 50)));
                textLeftBoundCreated = true;
            }
            newXPos = minXPos; // Block the player from moving any farther left
        } else if (canShowText && !textDownhillCreated && newXPos < transform.position.x && desiredRotation > 0f)
        {
            StartCoroutine(spawnText(0, "I can't give up.", inputRectSizeDelta: new Vector2(200, 50)));
            textDownhillCreated = true;
        } else if (newXPos < transform.position.x && newXPos < 5f && newYPos > 7f)
        {
            newXPos = transform.position.x; // Block the player from moving too far left at the top of the hill
            if (canShowText && !textUpperLeftBoundCreated)
            {
                StartCoroutine(spawnText(0, "Whoa, this isn't the right way."));
                StartCoroutine(spawnText(chainTextDelay, "I need to move right.", inputRectSizeDelta: new Vector2(300, 50)));
                textUpperLeftBoundCreated = true;
            }
        }
        transform.position = new Vector3(newXPos, newYPos, transform.position.z);
        if (!textCongratulationsCreated && newXPos >= maxXPos)
        {
            controlsEnabled = false;
            StartCoroutine(spawnText(0, "Congratulations!", 72, false, Color.cyan, new Vector2(1000, 200), new Vector3(10, yOffsetForText, 0))); // TODO: Probably change the size or color here
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

    private IEnumerator spawnText(float delay, string message, int size = 28, bool showPanel = true, Color? inputColor = null, Vector2? inputRectSizeDelta = null, Vector3? inputLocation = null)
    {
        Vector3 location = inputLocation ?? Vector3.zero;
        Vector2 rectSizeDelta = inputRectSizeDelta ?? new Vector2(400, 50);
        Color color = inputColor ?? Color.black;

        yield return new WaitForSeconds(delay);

        GameObject text = Instantiate(textPrefab, location, Quaternion.identity);
        text.transform.SetParent(GameObject.Find("Canvas").transform, false);
        text.transform.SetParent(GameObject.Find("Canvas").transform, false);
        text.GetComponent<Text>().text = message;
        text.GetComponent<Text>().fontSize = size;
        text.GetComponent<Text>().color = color;
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.sizeDelta = rectSizeDelta;
        Debug.Log("TextRect width = " + textRect.sizeDelta.x + " and height = " + textRect.sizeDelta.y);
        if (showPanel)
        {
            Debug.Log("Instantiating panel");
            GameObject panel = Instantiate(textPanelPrefab, location, Quaternion.identity);
            panel.transform.SetParent(GameObject.Find("Canvas").transform, false);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.position = textRect.position;
            panelRect.sizeDelta = textRect.sizeDelta;
            text.transform.SetParent(panel.transform, false); // Re-set the text parent to be the panel, so it shows up on top of the panel
        }

        yield return null;
    }
}
