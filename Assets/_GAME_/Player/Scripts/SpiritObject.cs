using UnityEngine;
using TMPro;

/// <summary>
/// Attach to any decor sprite that should react to spirit vision.
/// The object stays on its normal layer (e.g. Decor) at all times.
///
/// When spirit vision activates:
///   - The sprite swaps to a gray/white gradient material
///   - A TextMeshPro label fades in showing the Japanese translation
///
/// Setup in the Inspector:
///   - spiritMaterial: a URP Sprite material with a gray/white gradient (see notes below)
///   - japaneseText: the word/phrase to display (e.g. "木" or "つくえ")
///   - The TMP label is created automatically as a child if not assigned
/// </summary>
public class SpiritObject : MonoBehaviour
{
    [Header("Japanese Label")]
    [Tooltip("The Japanese text to display over this object (e.g. 木, つくえ, 水)")]
    public string japaneseText = "";
    [Tooltip("A TMP font asset that supports Japanese characters")]
    [SerializeField] private TMP_FontAsset japaneseFont;

    [Header("Visuals")]
    [Tooltip("A grayscale/gradient sprite material to swap to in spirit mode. " +
             "Create one via: Assets > Create > Material, set shader to Universal Render Pipeline/2D/Sprite-Lit-Default, " +
             "then tint it a light gray.")]
    [SerializeField] private Material spiritMaterial;
    [SerializeField] private float fadeSpeed = 4f;

    [Header("Floating Animation")]
    [SerializeField] private bool floatUpDown = true;
    [SerializeField] private float floatAmplitude = 0.08f;
    [SerializeField] private float floatFrequency = 1.5f;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private TextMeshPro label;
    private float targetAlpha = 0f;
    private Vector3 startLocalPosition;
    private bool isVisible = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"SpiritObject on '{gameObject.name}': no SpriteRenderer found.");
            return;
        }

        originalMaterial = spriteRenderer.material;
        startLocalPosition = transform.localPosition;

        SetupLabel();
        SetLabelAlpha(0f);
    }

    private void SetupLabel()
    {
        // Look for an existing TMP child first
        label = GetComponentInChildren<TextMeshPro>();

        if (label == null)
        {
            // Auto-create a child TextMeshPro object
            GameObject labelGO = new GameObject("SpiritLabel");
            labelGO.transform.SetParent(transform);

            // Position it centered slightly above the sprite
            labelGO.transform.localPosition = new Vector3(0f, 0f, -0.1f);
            labelGO.transform.localScale = Vector3.one;

            label = labelGO.AddComponent<TextMeshPro>();
            label.text = japaneseText;
            label.fontSize = 3f;
            label.alignment = TextAlignmentOptions.Center;
            label.color = Color.black;
            if (japaneseFont != null)
                label.font = japaneseFont;

            // Sorting: render above the sprite
            label.sortingLayerID = spriteRenderer.sortingLayerID;
            label.sortingOrder = spriteRenderer.sortingOrder + 1;
        }
        else
        {
            label.text = japaneseText;
            label.color = Color.black;
        }
    }

    void Update()
    {
        if (spriteRenderer == null) return;

        // Fade label alpha
        Color c = label.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        label.color = c;

        // Gentle float while visible
        if (floatUpDown && isVisible)
        {
            float offset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.localPosition = startLocalPosition + new Vector3(0f, offset, 0f);
        }
        else if (!isVisible)
        {
            // Snap back to resting position when hidden
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                startLocalPosition,
                Time.deltaTime * fadeSpeed
            );
        }
    }

    /// <summary>
    /// Called by SpiritVisionController to show or hide this spirit object.
    /// </summary>
    public void SetVisible(bool visible)
    {
        isVisible = visible;
        targetAlpha = visible ? 1f : 0f;

        // Swap material
        if (spriteRenderer != null)
        {
            spriteRenderer.material = visible
                ? (spiritMaterial != null ? spiritMaterial : originalMaterial)
                : originalMaterial;
        }
    }

    private void SetLabelAlpha(float alpha)
    {
        if (label == null) return;
        Color c = label.color;
        c.a = alpha;
        label.color = c;
    }
}