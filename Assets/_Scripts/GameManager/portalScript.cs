using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class portalScript : MonoBehaviour
{
    CanvasGroup portalCanvas,winCanvas;
    float targetAlpha = 1f;
    void OnTriggerEnter2D()
    {
        StartCoroutine(fade(0.5f, targetAlpha));
        targetAlpha= targetAlpha == 1f ? 0f : 1f;
    }
    private void OnTriggerStay2D()
    {
        StartCoroutine(fade(0.3f, targetAlpha));
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(win(0.5f, 1f) );
            SceneManager.LoadScene("level2");
        }
    }
    void OnTriggerExit2D()
    {
        StartCoroutine(fade(0.5f, targetAlpha));
        targetAlpha = targetAlpha == 0f ? 1f : 0f;
    }
    IEnumerator fade(float duration,float end )
    {
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            portalCanvas.alpha = Mathf.Lerp(portalCanvas.alpha, end, counter / duration);
            yield return null;
        }
    }
    IEnumerator win(float duration, float target)
    {
       float counter=0f;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            winCanvas.alpha = Mathf.Lerp(portalCanvas.alpha, target, counter / duration);
            yield return null;
        }
    }
}
