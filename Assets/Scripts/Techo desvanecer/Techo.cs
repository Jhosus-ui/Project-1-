using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofFadeInstant : MonoBehaviour
{
    public Tilemap roofTilemap; // Referencia al Tilemap que representa el techo
    public float targetAlpha = 0.3f; // Opacidad objetivo cuando el jugador está debajo

    private Color originalColor; // Color original de los tiles del techo

    private void Start()
    {
        // Guardar el color original de los tiles del techo
        originalColor = roofTilemap.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detectar si el jugador entra en la zona debajo del techo
        if (collision.CompareTag("Player"))
        {
            // Cambiar la opacidad del Tilemap al valor objetivo
            Color newColor = roofTilemap.color;
            newColor.a = targetAlpha;
            roofTilemap.color = newColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Detectar si el jugador sale de la zona debajo del techo
        if (collision.CompareTag("Player"))
        {
            // Restaurar la opacidad del Tilemap al valor original
            Color newColor = roofTilemap.color;
            newColor.a = originalColor.a;
            roofTilemap.color = newColor;
        }
    }
}