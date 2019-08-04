using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    public Transform target;
    public SpriteRenderer spriteRenderer;
    public float fadeSpeed = 10f;

    private bool endMode = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (endMode) {
            // Start fading in
            Color currentColor = spriteRenderer.color;
            if (fadeSpeed >= 0)
            {
                currentColor.a = Mathf.Min(255f, currentColor.a + fadeSpeed * Time.fixedDeltaTime);
            } else
            {
                currentColor.a = Mathf.Max(0f, currentColor.a + fadeSpeed * Time.fixedDeltaTime);
            }
            spriteRenderer.color = currentColor;

        } else if (target.position.x >= target.GetComponent<PlayerMovement>().maxXPos)
        {
            endMode = true;
        }
    }
}
