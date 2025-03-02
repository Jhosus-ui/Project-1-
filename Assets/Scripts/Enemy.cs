using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameObject player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public float minHealth = 1;
    public float maxHealth = 3;
    public float currentHealth;
    private float multiplicadorVida = 1.0f;

    public AudioClip deathSound; 
    public AudioClip[] zombieSounds;
    public float soundRange = 5f;
    public float soundCooldown = 3f; 
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
        float distanciaAlJugador = Vector3.Distance(transform.position, player.transform.position);

        if (distanciaAlJugador <= soundRange && Time.time - lastSoundTime >= soundCooldown)
        {
            AudioClip sonidoAleatorio = zombieSounds[Random.Range(0, zombieSounds.Length)];
            if (sonidoAleatorio != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoAleatorio);
            }

            lastSoundTime = Time.time;
        }
    }
}