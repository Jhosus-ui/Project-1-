using UnityEngine;

public class ArmaMovimiento : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public new Camera camera;
    public Transform spawner;
    public GameObject balaPrefab;
    public AudioClip shootSound; 
    public AudioClip noAmmoSound; 
    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>(); 
    }

    void Update()
    {
        if (PauseManager.IsPaused()) return;
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
            audioSource.PlayOneShot(shootSound); 
        }
        else if (AmmoManager.Instance.currentAmmo <= 0)
        {
            audioSource.PlayOneShot(noAmmoSound); //Balas Disponibles
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