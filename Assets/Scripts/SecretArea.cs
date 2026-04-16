using System.Collections;
using UnityEngine;

public class SecretArea : MonoBehaviour
{
    public float fadeDuration = 1f;

    SpriteRenderer spriteRenderer;
    Color hiddenColour;
    Coroutine currentCorutine;




    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hiddenColour = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentCorutine != null)
            {
                StopCoroutine(currentCorutine);
            }
            currentCorutine = StartCoroutine(FadeSprite(true));  
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
          if (collision.gameObject.CompareTag("Player"))
        {
            if (currentCorutine != null)
            {
                StopCoroutine(currentCorutine);
            }
            currentCorutine = StartCoroutine(FadeSprite(false));  
        }
    }

    private IEnumerator FadeSprite(bool fadeOut)
    {
        Color startColour = spriteRenderer.color;
        Color targetColour = fadeOut ? new Color(hiddenColour.r, hiddenColour.g, hiddenColour.b, 0f) : hiddenColour;
        float timeFading = 0f;

        while (timeFading < fadeDuration)
        {
            spriteRenderer.color = Color.Lerp(startColour, targetColour, timeFading / fadeDuration);
            timeFading += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = targetColour;

    }
}
