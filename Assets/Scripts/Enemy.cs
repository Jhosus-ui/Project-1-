using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent; 
    private GameObject player; 


    public int maxHealth = 3; 
    private int currentHealth; 


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = GameObject.FindWithTag("Player");
        currentHealth = maxHealth;
    }

    void Update()
    {
        MoverseHaciaElJugador();
    }

    void MoverseHaciaElJugador()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position); 
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount; 

        if (currentHealth <= 0)
        {
            Morir();
        }
    }

    public void Morir()
    {
        GameManager.Instance.EnemigoDerrotado();
        Destroy(gameObject); 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bala")) 
        {
            TakeDamage(1); 
            Destroy(collision.gameObject); 
        }
    }
}