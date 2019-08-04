using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeAwayText : MonoBehaviour
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
        Text text = GetComponent<Text>();
        Color currentColor = text.color;
        if (Time.fixedTime - startTime > holdTime)
        {
            if (startFadingTime <= 0f)
            {
                startFadingTime = Time.fixedTime;
            } else
            {
                currentColor.a = Mathf.Max(0f, currentColor.a - fadeSpeed * (Time.fixedTime - startFadingTime));
                text.color = currentColor;
            }
        }

    }
}
