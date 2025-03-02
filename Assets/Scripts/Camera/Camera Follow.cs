using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public float smoothSpeed = 0.125f; 
    public Vector3 offset; 
    public float fixedZ = -10f; 
  
    public Vector2 minBounds; 
    public Vector2 maxBounds; 

    public float shakeDuration = 0.2f; 
    public float shakeMagnitude = 0.1f; 

    private Vector3 _originalPosition; 
    private bool _isShaking = false;

    private void LateUpdate()
    {
        if (target == null) return; 
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = fixedZ;

        desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
        desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        if (!_isShaking)
        {
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed); // Si la cámara tiembla, suavisas
            transform.position = smoothedPosition;
        }
    }
    public void ShakeCamera()
    {
        if (!_isShaking)
        {
            _originalPosition = transform.position; 
            StartCoroutine(ShakeCoroutine());
        }
    }
    private IEnumerator ShakeCoroutine()
    {
        _isShaking = true; 

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector3 shakeOffset = Random.insideUnitCircle * shakeMagnitude;
            transform.position = _originalPosition + new Vector3(shakeOffset.x, shakeOffset.y, 0);

            elapsedTime += Time.deltaTime;
            yield return null; // Esperar siguiente frame
        }
        transform.position = _originalPosition;
        _isShaking = false; 
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, (minBounds.y + maxBounds.y) / 2, fixedZ);
        Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0.1f);
        Gizmos.DrawWireCube(center, size);
    }
}