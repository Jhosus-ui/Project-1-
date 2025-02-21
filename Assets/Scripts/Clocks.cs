using UnityEngine;

public class Reloj : MonoBehaviour
{
    public GameManager gameManager;

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