using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Variables de vida
    public float minHealth = 1;
    public float maxHealth = 3;
    public float currentHealth;
    private float multiplicadorVida = 1.0f;

    // Sonidos
    public AudioClip deathSound; // Sonido de muerte
    public AudioClip[] zombieSounds; // Sonidos aleatorios del zombie
    public float soundRange = 5f; // Rango para reproducir sonidos
    public float soundCooldown = 3f; // Tiempo entre sonidos
    private float lastSoundTime;
    private AudioSource audioSource;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        player = GameObject.FindWithTag("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        InicializarVida();
    }

    void Update()
    {
        if (currentHealth > 0)
        {
            MoverseHaciaElJugador();
            FlipTowardsPlayer();
            ReproducirSonidoAleatorio();
        }
    }

    void MoverseHaciaElJugador()
    {
        if (player != null && agent.enabled)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    void FlipTowardsPlayer()
    {
        if (player != null)
        {
            float deltaX = player.transform.position.x - transform.position.x;
            spriteRenderer.flipX = deltaX < 0;
        }
    }

    void InicializarVida()
    {
        currentHealth = Mathf.RoundToInt(Random.Range(minHealth, maxHealth + 1) * multiplicadorVida);
        Debug.Log($"Enemigo generado con {currentHealth} de vida.");
    }

    public void SetMultiplicadorVida(float multiplicador)
    {
        multiplicadorVida = multiplicador;
        InicializarVida();
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0) Morir();
    }

    public void Morir()
    {
        agent.enabled = false;

        // Reproducir el sonido de muerte
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        if (animator != null)
        {
            animator.SetTrigger("Dead");
        }

        StartCoroutine(DestruirDespuesDeAnimacion());
    }

    private IEnumerator DestruirDespuesDeAnimacion()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.EnemigoDerrotado();
        }
        else
        {
            Debug.LogWarning("EnemySpawner.Instance es null.");
        }

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

    void ReproducirSonidoAleatorio()
    {
        if (player == null || zombieSounds == null || zombieSounds.Length == 0) return;

        // Calcular la distancia al jugador
        float distanciaAlJugador = Vector3.Distance(transform.position, player.transform.position);

        // Verificar si el jugador está dentro del rango y si ha pasado el tiempo de cooldown
        if (distanciaAlJugador <= soundRange && Time.time - lastSoundTime >= soundCooldown)
        {
            // Reproducir un sonido aleatorio
            AudioClip sonidoAleatorio = zombieSounds[Random.Range(0, zombieSounds.Length)];
            if (sonidoAleatorio != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoAleatorio);
            }

            // Actualizar el tiempo del último sonido
            lastSoundTime = Time.time;
        }
    }
}