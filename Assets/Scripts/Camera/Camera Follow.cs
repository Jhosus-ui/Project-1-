using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Referencia al transform del personaje
    public float smoothSpeed = 0.125f; // Suavizado del movimiento de la cámara
    public Vector3 offset; // Desplazamiento de la cámara respecto al personaje
    public float fixedZ = -10f; // Posición Z fija para la cámara

    // Límites de la cámara (en coordenadas del mundo)
    public Vector2 minBounds; // Límite mínimo (esquina inferior izquierda)
    public Vector2 maxBounds; // Límite máximo (esquina superior derecha)

    // Temblor de cámara
    public float shakeDuration = 0.2f; // Duración del temblor
    public float shakeMagnitude = 0.1f; // Intensidad del temblor

    private Vector3 _originalPosition; // Posición original de la cámara
    private bool _isShaking = false; // Indica si la cámara está temblando

    private void LateUpdate()
    {
        if (target == null) return; // Si no hay objetivo, no hacer nada

        // Calcular la posición deseada de la cámara en X e Y, pero mantener Z fija
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = fixedZ;

        // Aplicar los límites a la posición deseada
        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);

        // Si la cámara no está temblando, suavizar el movimiento hacia la posición deseada
        if (!_isShaking)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }

    // Método para activar el temblor de la cámara
    public void ShakeCamera()
    {
        if (!_isShaking)
        {
            _originalPosition = transform.position; // Guardar la posición original de la cámara
            StartCoroutine(ShakeCoroutine());
        }
    }

    // Corrutina para el temblor de la cámara
    private IEnumerator ShakeCoroutine()
    {
        _isShaking = true; // Indicar que la cámara está temblando

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            // Mover la cámara a una posición aleatoria dentro de un círculo
            Vector3 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            transform.position = _originalPosition + new Vector3(shakeOffset.x, shakeOffset.y, 0);

            elapsedTime += Time.deltaTime;
            yield return null; // Esperar al siguiente frame
        }

        // Restaurar la posición original de la cámara
        transform.position = _originalPosition;
        _isShaking = false; // Indicar que el temblor ha terminado
    }

    // Método para dibujar los límites en el editor (solo para debug)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, fixedZ);
        Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}