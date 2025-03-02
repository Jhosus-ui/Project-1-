using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int minAmmoAmount = 5;  // Cantidad mínima de munición
    public int maxAmmoAmount = 15; // Cantidad máxima de munición

    // Variables para el efecto de levitación
    public float levitationHeight = 0.5f; // Altura de la levitación
    public float levitationSpeed = 1f; // Velocidad de la levitación
    private Vector3 startPosition;

    // Sonidos
    public AudioClip collectSound; // Sonido cuando el jugador recolecta el item
    public AudioClip fullAmmoSound; // Sonido cuando la munición total está llena
    private AudioSource audioSource;

    private void Start()
    {
        startPosition = transform.position; // Guardar la posición inicial
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    }

    private void Update()
    {
        // Efecto de levitación
        float newY = startPosition.y + Mathf.Sin(Time.time * levitationSpeed) * levitationHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Verificar si la munición total está llena
            if (AmmoManager.Instance.totalAmmo >= AmmoManager.Instance.maxTotalAmmo)
            {
                // Reproducir sonido de munición llena
                if (fullAmmoSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(fullAmmoSound);
                }
            }
            else
            {
                // Recolectar munición
                int randomAmmoAmount = Random.Range(minAmmoAmount, maxAmmoAmount + 1);
                AmmoManager.Instance.AddAmmo(randomAmmoAmount);

                // Reproducir sonido de recolección
                if (collectSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(collectSound);
                }

                Destroy(gameObject);
            }
        }
    }
}