using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Vector2 moveInput;
    private Vector2 lastMoveDir;

    private Rigidbody2D rb;
    private Animator animator;
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;

    private bool inSpiritMode = false;
    [SerializeField] private float spiritDeceleration = 6f; // how fast the player slows to a halt

    private const float MOVE_EPSILON = 0.01f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        lastMoveDir = Vector2.down;
        animator.SetFloat("LastInputX", lastMoveDir.x);
        animator.SetFloat("LastInputY", lastMoveDir.y);
    }

    void Update()
    {
        if (inSpiritMode)
        {
            // Decelerate smoothly to zero
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * spiritDeceleration);

            // Once close enough, fully stop and set idle animation
            if (rb.linearVelocity.sqrMagnitude < 0.001f)
            {
                rb.linearVelocity = Vector2.zero;
                animator.SetBool("isWalking", false);
                animator.SetFloat("InputX", 0f);
                animator.SetFloat("InputY", 0f);
            }

            StopFootsteps();
            return; // Skip normal movement logic
        }

        // Normal movement
        rb.linearVelocity = moveInput * speed;

        bool isMoving = moveInput.sqrMagnitude > MOVE_EPSILON;
        animator.SetBool("isWalking", isMoving);

        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

        if (isMoving)
        {
            lastMoveDir = moveInput.normalized;
            animator.SetFloat("LastInputX", lastMoveDir.x);
            animator.SetFloat("LastInputY", lastMoveDir.y);
        }

        if (isMoving && !playingFootsteps)
            StartFootsteps();
        else if (!isMoving)
            StopFootsteps();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Called by SpiritVisionController. Disables input and decelerates the player,
    /// or re-enables movement when spirit mode is turned off.
    /// </summary>
    public void SetSpiritMode(bool active)
    {
        inSpiritMode = active;

        if (!active)
        {
            // Player can move again — moveInput will resume from next OnMove callback
            // Nothing else needed; Update() will resume normal logic
        }
    }

    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    }

    void PlayFootstep()
    {
        SoundEffectManager.Instance.Play("Footstep");
    }
}