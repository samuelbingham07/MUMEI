using System.Collections;
using UnityEngine;

public class WalkBehindFader : MonoBehaviour
{
    [SerializeField] private float fadedAlpha = 0.5f;
    [SerializeField] private float fadeSpeed = 5f;

    private SpriteRenderer sr;
    private float targetAlpha = 1f;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // Add a trigger collider sized to this sprite if none exists
        if (GetComponent<Collider2D>() == null)
        {
            BoxCollider2D col = gameObject.AddComponent<BoxCollider2D>();
            col.size = sr.sprite.bounds.size;
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SetAlpha(fadedAlpha);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            SetAlpha(1f);
    }

    void SetAlpha(float target)
    {
        targetAlpha = target;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeTo(target));
    }

    IEnumerator FadeTo(float target)
    {
        Color c = sr.color;
        while (!Mathf.Approximately(c.a, target))
        {
            c.a = Mathf.MoveTowards(c.a, target, fadeSpeed * Time.deltaTime);
            sr.color = c;
            yield return null;
        }
        c.a = target;
        sr.color = c;
    }
}
