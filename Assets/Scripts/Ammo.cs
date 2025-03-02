using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int minAmmoAmount = 5;  // Cantidad mínima de munición
    public int maxAmmoAmount = 15; // Cantidad máxima de munición

    // Variables para el efecto de levitación
    public float levitationHeight = 0.5f; // Altura de la levitación
    public float levitationSpeed = 1f; // Velocidad de la levitación
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position; // Guardar la posición inicial
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
            if (AmmoManager.Instance.CanRegenerateAmmo())
            {
                int randomAmmoAmount = Random.Range(minAmmoAmount, maxAmmoAmount + 1);
                AmmoManager.Instance.AddAmmo(randomAmmoAmount); // Añadir munición
                Destroy(gameObject); // Destruir el objeto
            }
        }
    }
}