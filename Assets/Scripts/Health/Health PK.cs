using UnityEngine;

public class HealthPickupG : MonoBehaviour
{
    // Variables para el efecto de levitaci�n
    public float levitationHeight = 0.5f; // Altura de la levitaci�n
    public float levitationSpeed = 1f; // Velocidad de la levitaci�n
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position; // Guardar la posici�n inicial
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
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.CanHeal())
            {
                playerHealth.Heal(1); // Sumar una vida al jugador
                Destroy(gameObject); // Destruir el objeto de salud
                HealthM.Instance.HealthPickedUp(); // Notificar al GameManager que se recogi� una cura
            }
        }
    }
}