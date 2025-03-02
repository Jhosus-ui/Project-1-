using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Referencia al transform del personaje
    public float smoothSpeed = 0.125f; // Suavizado del movimiento de la c�mara
    public Vector3 offset; // Desplazamiento de la c�mara respecto al personaje
    public float fixedZ = -10f; // Posici�n Z fija para la c�mara

    // L�mites de la c�mara (en coordenadas del mundo)
    public Vector2 minBounds; // L�mite m�nimo (esquina inferior izquierda)
    public Vector2 maxBounds; // L�mite m�ximo (esquina superior derecha)

    // Temblor de c�mara
    public float shakeDuration = 0.2f; // Duraci�n del temblor
    public float shakeMagnitude = 0.1f; // Intensidad del temblor

    private Vector3 _originalPosition; // Posici�n original de la c�mara
    private bool _isShaking = false; // Indica si la c�mara est� temblando

    private void LateUpdate()
    {
        if (target == null) return; // Si no hay objetivo, no hacer nada

        // Calcular la posici�n deseada de la c�mara en X e Y, pero mantener Z fija
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = fixedZ;

        // Aplicar los l�mites a la posici�n deseada
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

        // Si la c�mara no est� temblando, suavizar el movimiento hacia la posici�n deseada
        if (!_isShaking)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    // M�todo para activar el temblor de la c�mara
    public void ShakeCamera()
    {
        if (!_isShaking)
        {
            _originalPosition = transform.position; // Guardar la posici�n original de la c�mara
            StartCoroutine(ShakeCoroutine());
        }
    }

    // Corrutina para el temblor de la c�mara
    private IEnumerator ShakeCoroutine()
    {
        _isShaking = true; // Indicar que la c�mara est� temblando

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Mover la c�mara a una posici�n aleatoria dentro de un c�rculo
            Vector3 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            transform.position = _originalPosition + new Vector3(shakeOffset.x, shakeOffset.y, 0);

            elapsedTime += Time.deltaTime;
            yield return null; // Esperar al siguiente frame
        }

        // Restaurar la posici�n original de la c�mara
        transform.position = _originalPosition;
        _isShaking = false; // Indicar que el temblor ha terminado
    }

    // M�todo para dibujar los l�mites en el editor (solo para debug)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, fixedZ);
        Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}