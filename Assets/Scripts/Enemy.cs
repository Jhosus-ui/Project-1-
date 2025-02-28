using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    private SpriteRenderer spriteRenderer; // Para manejar el flip X

    public int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = GameObject.FindWithTag("Player");
        currentHealth = maxHealth;

        // Obtener el componente SpriteRenderer para manejar el flip X
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MoverseHaciaElJugador();
        FlipTowardsPlayer(); // Llamar a la función para hacer el flip X
    }

    void MoverseHaciaElJugador()
    {
        if (player != null)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    void FlipTowardsPlayer()
    {
        if (player != null)
        {
            // Calcular la diferencia en la posición X entre el enemigo y el jugador
            float deltaX = player.transform.position.x - transform.position.x;

            // Si el jugador está a la derecha del enemigo
            if (deltaX > 0)
            {
                spriteRenderer.flipX = false; // Mirar a la derecha
            }
            // Si el jugador está a la izquierda del enemigo
            else if (deltaX < 0)
            {
                spriteRenderer.flipX = true; // Mirar a la izquierda
            }
            // Si están en la misma posición en X, no hacer nada (mantener el último flip)
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