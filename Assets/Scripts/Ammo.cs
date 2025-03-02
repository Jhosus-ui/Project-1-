using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int minAmmoAmount = 5;  // Cantidad m�nima de munici�n
    public int maxAmmoAmount = 15; // Cantidad m�xima de munici�n

    // Variables para el efecto de levitaci�n
    public float levitationHeight = 0.5f; // Altura de la levitaci�n
    public float levitationSpeed = 1f; // Velocidad de la levitaci�n
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position; // Guardar la posici�n inicial
    }

    private void Update()
    {
        // Efecto de levitaci�n
        float newY = startPosition.y + Mathf.Sin(Time.time * levitationSpeed) * levitationHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (AmmoManager.Instance.CanRegenerateAmmo())
            {
                int randomAmmoAmount = Random.Range(minAmmoAmount, maxAmmoAmount + 1);
                AmmoManager.Instance.AddAmmo(randomAmmoAmount); // A�adir munici�n
                Destroy(gameObject); // Destruir el objeto
            }
        }
    }
}