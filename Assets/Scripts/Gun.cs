using UnityEngine;

public class ArmaMovimiento : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public new Camera camera;
    public Transform spawner;
    public GameObject balaPrefab;
    public AudioClip shootSound; // Sonido de disparo
    public AudioClip noAmmoSound; // Sonido cuando no hay balas
    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1")) Disparar();
        RotateTowardsMouse();
    }

    void Disparar()
    {
        if (AmmoManager.Instance.currentAmmo > 0 && !AmmoManager.Instance.IsReloading())
        {
            animator.SetTrigger("Shoot");
            Instantiate(balaPrefab, spawner.position, transform.rotation);
            AmmoManager.Instance.UseAmmo();
            audioSource.PlayOneShot(shootSound); // Reproducir el sonido de disparo
        }
        else if (AmmoManager.Instance.currentAmmo <= 0)
        {
            Debug.Log("No hay balas disponibles.");
            audioSource.PlayOneShot(noAmmoSound); // Reproducir el sonido de "sin balas"
        }
    }

    void RotateTowardsMouse()
    {
        float angle = GetAngleTowardsMouse();
        transform.rotation = Quaternion.Euler(0, 0, angle);
        spriteRenderer.flipY = angle >= 90 && angle <= 270;
    }

    float GetAngleTowardsMouse()
    {
        Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDirection = mouseWorldPosition - transform.position;
        mouseDirection.z = 0;
        return (Vector3.SignedAngle(Vector3.right, mouseDirection, Vector3.forward) + 360) % 360;
    }
}