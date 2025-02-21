using UnityEngine;

public class Reloj : MonoBehaviour
{
    public GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            float tiempoAñadido = gameManager.ObtenerTiempoRelojAleatorio();
            gameManager.AumentarTiempo(tiempoAñadido);
            Destroy(gameObject);
        }
    }
}