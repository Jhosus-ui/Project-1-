using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int minAmmoAmount = 5;  
    public int maxAmmoAmount = 15; 

    public float levitationHeight = 0.5f; 
    public float levitationSpeed = 1f; 
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position; 
    }

    private void Update()
    {
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
                AmmoManager.Instance.AddAmmo(randomAmmoAmount); 
                Destroy(gameObject); 
            }
        }
    }
}