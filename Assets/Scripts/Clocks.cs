using UnityEngine;

public class Reloj : MonoBehaviour
{
    public GameManager gameManager;

    public float levitationHeight = 0.5f; 
    public float levitationSpeed = 1f; 
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position; // Guardar la posici�n inicial
    }

    private void Update()
    {
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