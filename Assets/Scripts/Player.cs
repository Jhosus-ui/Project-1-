using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Movement();
        HandleAnimation();
    }

    void Movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = moveDirection * moveSpeed;


        if (moveX > 0) // Derecha
        {
            facingRight = true;
        }
        else if (moveX < 0) // Izquierda
        {
            facingRight = false;
        }

        if (facingRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void HandleAnimation()
    {
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
    }
}