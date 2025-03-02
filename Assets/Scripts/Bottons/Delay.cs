using UnityEngine;

public class FadeOutObject : MonoBehaviour
{
    public float fadeDuration = 2f; 
    public float delayBeforeFade = 1f; 

    private SpriteRenderer spriteRenderer; 
    private MeshRenderer meshRenderer;    
    private float timer = 0f;
    private bool isFading = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (spriteRenderer == null && meshRenderer == null)
        {
            enabled = false; // Desactivar el script 
        }
    }

    private void Update()
    {
       
        if (!isFading && timer < delayBeforeFade)
        {
            timer += Time.deltaTime;
            return;
        }

        if (!isFading)
        {
            isFading = true;
            timer = 0f; 
        }
       
        float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);  // Calcular el alpha basado en el tiempo transcurrido
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        else if (meshRenderer != null)
        {
            foreach (Material material in meshRenderer.materials)
            {
                Color color = material.color;
                color.a = alpha;
                material.color = color;
            }
        }

        timer += Time.deltaTime;
        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}