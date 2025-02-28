using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    private SpriteRenderer spriteRenderer; // Para manejar el flip X

    // Variables de vida (ahora públicas para configurar el rango)
    public int minHealth = 1; // Vida mínima del enemigo
    public int maxHealth = 3; // Vida máxima del enemigo
    public int currentHealth; // Vida actual del enemigo

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = GameObject.FindWithTag("Player");

        // Obtener el componente SpriteRenderer para manejar el flip X
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Inicializar la vida del enemigo con un valor aleatorio dentro del rango
        InicializarVida();
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

    // Método para inicializar la vida del enemigo con un valor aleatorio dentro del rango
    void InicializarVida()
    {
        // Asignar una vida aleatoria dentro del rango [minHealth, maxHealth]
        currentHealth = Random.Range(minHealth, maxHealth + 1); // +1 para incluir el valor máximo
        Debug.Log($"Enemigo generado con {currentHealth} de vida.");
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