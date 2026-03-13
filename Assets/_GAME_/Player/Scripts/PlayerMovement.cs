using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Vector2 moveInput;      // what the player is pressing *this* frame
    private Vector2 lastMoveDir;    // the direction we want to keep for idle

    private Rigidbody2D rb;
    private Animator animator;
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;
    // How small is "basically not moving"
    private const float MOVE_EPSILON = 0.01f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Default facing direction if you want (e.g. down)
        lastMoveDir = Vector2.down;
        animator.SetFloat("LastInputX", lastMoveDir.x);
        animator.SetFloat("LastInputY", lastMoveDir.y);
    }

    void Update()
    {
        // Move the player
        rb.linearVelocity = moveInput * speed;

        // Are we actually moving?
        bool isMoving = moveInput.sqrMagnitude > MOVE_EPSILON;
        animator.SetBool("isWalking", isMoving);

        // While moving, update the "live" direction for the walk blend tree
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

        // Only update the remembered direction when we're clearly moving
        if (isMoving)


        {
            lastMoveDir = moveInput.normalized;
            animator.SetFloat("LastInputX", lastMoveDir.x);
            animator.SetFloat("LastInputY", lastMoveDir.y);
        }

        if (isMoving && !playingFootsteps)
        {
            StartFootsteps();
        }
        else if (!isMoving)
        {
            StopFootsteps();
        }

        // When we stop moving, we **do not** touch LastInputX / LastInputY.
        // They stay as the last non-zero direction, including diagonals.
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Just read the current value. Don't try to be clever in here.
        moveInput = context.ReadValue<Vector2>();

        // We do NOT change LastInput here.
        // We let Update() handle when to commit a new facing direction.
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

