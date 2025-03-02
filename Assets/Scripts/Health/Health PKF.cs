using UnityEngine;

public class HealthPickupF : MonoBehaviour
{
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
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.CanHeal())
            {
                playerHealth.Heal(3); 
                Destroy(gameObject); 
                HealthM.Instance.HealthPickedUp(); 
            }
        }
    }
}