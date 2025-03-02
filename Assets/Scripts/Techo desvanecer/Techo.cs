using UnityEngine;
using UnityEngine.Tilemaps;

public class RoofFadeInstant : MonoBehaviour
{
    public Tilemap roofTilemap; 
    public float targetAlpha = 0.3f; 
    private Color originalColor; 

    private void Start()
    {
        originalColor = roofTilemap.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Color newColor = roofTilemap.color;
            newColor.a = targetAlpha;
            roofTilemap.color = newColor;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Color newColor = roofTilemap.color;
            newColor.a = originalColor.a;
            roofTilemap.color = newColor;
        }
    }
}