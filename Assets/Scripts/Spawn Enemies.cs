using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    // Configuración pública
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int minEnemies = 10; // Enemigos iniciales en la primera oleada
    public int incrementoEnemigosPorOleada = 3; // Enemigos adicionales por oleada
    public float startDelay = 10f; // Tiempo de espera antes de la primera oleada
    public float intervaloEntreEnemigos = 0.5f; // Tiempo entre la aparición de cada enemigo
    public float tiempoEntreOleadas = 10f; // Tiempo de espera entre oleadas

    // Sonidos
    public AudioClip startWaveSound; // Sonido cuando comienza una oleada
    private AudioSource audioSource;

    // Variables privadas
    private int currentEnemies = 0;
    private bool juegoIniciado = false;
    private int oleadaActual = 0; // Número de la oleada actual
    private bool esperandoSiguienteOleada = false; // Para controlar el paso a la siguiente oleada
    private float multiplicadorVida = 1.0f; // Multiplicador de vida por oleada

    // Singleton
    public static EnemySpawner Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Ya existe una instancia de EnemySpawner. Destruyendo esta instancia.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Obtener el componente AudioSource
        Invoke(nameof(IniciarPrimeraOleada), startDelay); // Iniciar la espera para la primera oleada
    }

    void IniciarPrimeraOleada()
    {
        if (!juegoIniciado)
        {
            juegoIniciado = true;
        }

        if (startWaveSound != null)
        {
            audioSource.PlayOneShot(startWaveSound);
        }

        StartCoroutine(GenerarOleadaEnemigosGradualmente());
    }

    IEnumerator GenerarOleadaEnemigosGradualmente()
    {
        int cantidadEnemigos = minEnemies + (oleadaActual * incrementoEnemigosPorOleada);
        GameManager.Instance.ActualizarTextoOleada($"Round: {oleadaActual}");
        GameManager.Instance.MostrarTextoOleadaTemporal($"{oleadaActual}");

        for (int i = 0; i < cantidadEnemigos; i++)
        {
            GenerarEnemigo();
            yield return new WaitForSeconds(intervaloEntreEnemigos);
        }
    }

    void GenerarEnemigo()
    {
        if (enemyPrefab != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            if (enemy != null)
            {
                Debug.Log($"Enemigo generado en {spawnPoint.position}");
                Enemy enemyScript = enemy.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    enemyScript.SetMultiplicadorVida(multiplicadorVida);
                }
                else
                {
                    Debug.LogError("El prefab del enemigo no tiene el componente Enemy.");
                }
                currentEnemies++;
            }
            else
            {
                Debug.LogError("No se pudo generar el enemigo.");
            }
        }
        else
        {
            Debug.LogError("El prefab del enemigo o los puntos de spawn no están configurados correctamente.");
        }
    }

    public void EnemigoDerrotado()
    {
        currentEnemies--;
        ScoreManager.Instance.AddScore(13);
        if (currentEnemies <= 0 && !esperandoSiguienteOleada)
        {
            esperandoSiguienteOleada = true;
            oleadaActual++;
            multiplicadorVida += 0.1f; // Aumentar el multiplicador en 0.1 por oleada

            GameManager.Instance.MostrarTextoOleadaTemporal($"{oleadaActual}");
            if (startWaveSound != null)
            {
                audioSource.PlayOneShot(startWaveSound);
            }
            Invoke(nameof(ComenzarNuevaOleada), tiempoEntreOleadas);
        }
    }

    void ComenzarNuevaOleada()
    {
        esperandoSiguienteOleada = false;
        StartCoroutine(GenerarOleadaEnemigosGradualmente());
    }
}