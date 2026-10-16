using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveInput = Vector2.down;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        bool isWalking = moveInput.sqrMagnitude > 0f;
        animator.SetBool("isWalking", isWalking);

        if (isWalking)
        {
            lastMoveInput = moveInput;
        }

        animator.SetFloat("LastInputX", lastMoveInput.x);
        animator.SetFloat("LastInputY", lastMoveInput.y);

        if (moveInput.x != 0)
        {
            spriteRenderer.flipX = moveInput.x < 0;
        }
    }
}
