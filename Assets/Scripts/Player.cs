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
        FlipBasedOnMousePosition();
    }

    void Movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void HandleAnimation()
    {
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
    }

    void FlipBasedOnMousePosition()
    {
        // Obtener la posici�n del mouse en el mundo
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Determinar si el mouse est� a la izquierda o derecha del personaje
        if (mousePosition.x < transform.position.x && facingRight)
        {
            Flip();
        }
        else if (mousePosition.x > transform.position.x && !facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        // Cambiar la direcci�n del personaje
        facingRight = !facingRight;
        transform.localScale = new Vector3(facingRight ? 1 : -1, 1, 1);
    }
}