using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookAtMouseDuration = 2f; // Tiempo en segundos que el personaje mirará al mouse

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isLookingAtMouse = false;
    private float lookTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Movement();

        if (Input.GetMouseButtonDown(0)) // 0 es el botón izquierdo del mouse
        {
            StartLookingAtMouse();
        }

        if (isLookingAtMouse)
        {
            LookAtMouse();
            lookTimer += Time.deltaTime;

            if (lookTimer >= lookAtMouseDuration)
            {
                StopLookingAtMouse();
            }
        }
        else
        {
            FlipX(); // Volver a la lógica normal de voltear en el eje X
        }
    }

    void Movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void FlipX()
    {
        float moveX = Input.GetAxisRaw("Horizontal");

        if (moveX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveX < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    void StartLookingAtMouse()
    {
        isLookingAtMouse = true;
        lookTimer = 0f;
    }

    void LookAtMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePosition.x > transform.position.x)
        {
            spriteRenderer.flipX = false; // Mirar a la derecha
        }
        else
        {
            spriteRenderer.flipX = true; // Mirar a la izquierda
        }
    }

    void StopLookingAtMouse()
    {
        isLookingAtMouse = false;
    }
}