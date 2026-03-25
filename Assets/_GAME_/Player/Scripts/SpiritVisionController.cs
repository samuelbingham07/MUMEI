using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpiritVisionController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference spiritVisionAction;

    [Header("Overlay")]
    [Tooltip("A full-screen UI Image on a Canvas set to Screen Space - Overlay. " +
             "Set its color to purple in the Inspector — the script controls alpha only.")]
    [SerializeField] private Image overlayImage;
    [SerializeField] private float maxAlpha = 0.35f;       // how opaque the purple gets (0-1)
    [SerializeField] private float transitionDuration = 0.5f;

    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;

    private bool isActive = false;
    private float startAlpha = 0f;
    private float targetAlpha = 0f;
    private float transitionTimer = 0f;
    private bool transitioning = false;

    private SpiritObject[] spiritObjects;

    void Awake()
    {
        if (overlayImage == null)
            Debug.LogError("SpiritVisionController: No overlay Image assigned!");
        else
            SetOverlayAlpha(0f); // start invisible

        if (playerMovement == null)
            playerMovement = FindAnyObjectByType<PlayerMovement>();
    }

    void Start()
    {
        spiritObjects = FindObjectsByType<SpiritObject>(FindObjectsSortMode.None);
    }

    void OnEnable()
    {
        if (spiritVisionAction != null)
        {
            spiritVisionAction.action.performed += OnSpiritVisionToggle;
            spiritVisionAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (spiritVisionAction != null)
            spiritVisionAction.action.performed -= OnSpiritVisionToggle;
    }

    private void OnSpiritVisionToggle(InputAction.CallbackContext context)
    {
        isActive = !isActive;

        // Start transition from current alpha
        startAlpha = overlayImage != null ? overlayImage.color.a : 0f;
        targetAlpha = isActive ? maxAlpha : 0f;
        transitionTimer = 0f;
        transitioning = true;

        playerMovement?.SetSpiritMode(isActive);

        foreach (SpiritObject obj in spiritObjects)
            if (obj != null) obj.SetVisible(isActive);
    }

    void Update()
    {
        if (transitioning && overlayImage != null)
        {
            transitionTimer += Time.deltaTime;
            float t = Mathf.Clamp01(transitionTimer / transitionDuration);
            SetOverlayAlpha(Mathf.Lerp(startAlpha, targetAlpha, t));

            if (t >= 1f)
                transitioning = false;
        }
    }

    private void SetOverlayAlpha(float alpha)
    {
        Color c = overlayImage.color;
        c.a = alpha;
        overlayImage.color = c;
    }
}