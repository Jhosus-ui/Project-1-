using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int minAmmoAmount = 5;  // Cantidad m�nima de munici�n
    public int maxAmmoAmount = 15; // Cantidad m�xima de munici�n

    // Variables para el efecto de levitaci�n
    public float levitationHeight = 0.5f; // Altura de la levitaci�n
    public float levitationSpeed = 1f; // Velocidad de la levitaci�n
    private Vector3 startPosition;

    // Sonidos
    public AudioClip collectSound; // Sonido cuando el jugador recolecta el item
    public AudioClip fullAmmoSound; // Sonido cuando la munici�n total est� llena
    private AudioSource audioSource;

    private void Start()
    {
        startPosition = transform.position; // Guardar la posici�n inicial
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
    }

    private void Update()
    {
        // Efecto de levitaci�n
        float newY = startPosition.y + Mathf.Sin(Time.time * levitationSpeed) * levitationHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Verificar si la munici�n total est� llena
            if (AmmoManager.Instance.totalAmmo >= AmmoManager.Instance.maxTotalAmmo)
            {
                // Reproducir sonido de munici�n llena
                if (fullAmmoSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(fullAmmoSound);
                }
            }
            else
            {
                // Recolectar munici�n
                int randomAmmoAmount = Random.Range(minAmmoAmount, maxAmmoAmount + 1);
                AmmoManager.Instance.AddAmmo(randomAmmoAmount);

                // Reproducir sonido de recolecci�n
                if (collectSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(collectSound);
                }

                Destroy(gameObject);
            }
        }
    }
}