using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 10; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            if (AmmoManager.Instance.CanRegenerateAmmo()) 
            {
                AmmoManager.Instance.AddAmmo(ammoAmount); 
                Destroy(gameObject); 
            }
        }
    }
}