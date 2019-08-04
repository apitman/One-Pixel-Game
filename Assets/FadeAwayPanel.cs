using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeAwayPanel : MonoBehaviour
{
    public float holdTime = 10f;
    public float fadeSpeed = 5f;

    private float startTime = 0f;
    private float startFadingTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
        Image image = GetComponent<Image>();
        Color currentColor = image.color;
        if (Time.fixedTime - startTime > holdTime)
        {
            if (startFadingTime <= 0f)
            {
                startFadingTime = Time.fixedTime;
            }
            else
            {
                currentColor.a = Mathf.Max(0f, currentColor.a - fadeSpeed * (Time.fixedTime - startFadingTime));
                image.color = currentColor;
            }
        }

        if (currentColor.a <= 0f)
        {
            Destroy(this.gameObject);
        }
    }
}
