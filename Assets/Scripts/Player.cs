using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookAtMouseDuration = 2f; 

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isLookingAtMouse = false;
    private float lookTimer = 0f;
    private bool isWalking = false;

    public AudioClip walkingSound;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>(); 
        audioSource = GetComponent<AudioSource>(); 
    }

    void Update()
    {
        if (PauseManager.IsPaused()) return;

        Movement();

        if (Input.GetMouseButtonDown(0)) 
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
            FlipX(); 
        }

        animator.SetBool("IsWalking", isWalking);
        if (isWalking)
        {
            if (!audioSource.isPlaying && walkingSound != null)
            {
                audioSource.clip = walkingSound;
                audioSource.loop = true; 
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); 
            }
        }
    }

    void Movement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 moveDirection = new Vector2(moveX, moveY).normalized;
        rb.linearVelocity = moveDirection * moveSpeed;
        isWalking = moveDirection.magnitude > 0;
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
            spriteRenderer.flipX = false; 
        }
        else
        {
            spriteRenderer.flipX = true; 
        }
    }

    void StopLookingAtMouse()
    {
        isLookingAtMouse = false;
    }
}