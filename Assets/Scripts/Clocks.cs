using UnityEngine;

public class Reloj : MonoBehaviour
{
    public GameManager gameManager;

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
            float tiempoA�adido = gameManager.ObtenerTiempoRelojAleatorio();
            gameManager.AumentarTiempo(tiempoA�adido);
            Destroy(gameObject);
        }
    }
}