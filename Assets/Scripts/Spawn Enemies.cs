using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int minEnemies = 10; 
    public int incrementoEnemigosPorOleada = 3; 
    public float startDelay = 10f;
    public float intervaloEntreEnemigos = 0.5f; 
    public float tiempoEntreOleadas = 10f;
   
    public AudioClip startWaveSound; 
    private AudioSource audioSource;

    private int currentEnemies = 0;
    private bool juegoIniciado = false;
    private int oleadaActual = 0; 
    private bool esperandoSiguienteOleada = false; 
    private float multiplicadorVida = 1.0f; 
    public static EnemySpawner Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Invoke(nameof(IniciarPrimeraOleada), startDelay); 
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
                Enemy enemyScript = enemy.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    enemyScript.SetMultiplicadorVida(multiplicadorVida);
                }
                currentEnemies++;
            }      
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