using UnityEngine;

public class FadeOutObject : MonoBehaviour
{
    public float fadeDuration = 2f; // Duración del desvanecimiento en segundos
    public float delayBeforeFade = 1f; // Tiempo de espera antes de comenzar el desvanecimiento

    private SpriteRenderer spriteRenderer; // Para objetos 2D
    private MeshRenderer meshRenderer;     // Para objetos 3D
    private float timer = 0f;
    private bool isFading = false;

    private void Start()
    {
        // Intentar obtener el componente SpriteRenderer (para objetos 2D)
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Intentar obtener el componente MeshRenderer (para objetos 3D)
        meshRenderer = GetComponent<MeshRenderer>();

        // Verificar si el objeto tiene un componente de renderizado
        if (spriteRenderer == null && meshRenderer == null)
        {
            Debug.LogError("El objeto no tiene un SpriteRenderer ni un MeshRenderer.");
            enabled = false; // Desactivar el script si no hay un componente de renderizado
        }
    }

    private void Update()
    {
        // Esperar el tiempo de retraso antes de comenzar el desvanecimiento
        if (!isFading && timer < delayBeforeFade)
        {
            timer += Time.deltaTime;
            return;
        }

        // Comenzar el desvanecimiento
        if (!isFading)
        {
            isFading = true;
            timer = 0f; // Reiniciar el temporizador para el desvanecimiento
        }

        // Calcular el alpha basado en el tiempo transcurrido
        float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

        // Aplicar el alpha al color del objeto
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

        // Incrementar el temporizador
        timer += Time.deltaTime;

        // Destruir el objeto cuando el desvanecimiento haya terminado
        if (timer >= fadeDuration)
        {
            Destroy(gameObject);
        }
    }
}