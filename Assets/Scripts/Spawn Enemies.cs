using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    // Configuración pública
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int minEnemies = 10; // Enemigos iniciales en la primera oleada
    public int incrementoEnemigosPorOleada = 3; // Enemigos adicionales por oleada
    public float minSpawnInterval = 2f, maxSpawnInterval = 5f;
    public float startDelay = 10f; // Tiempo de espera antes de la primera oleada
    public float intervaloEntreEnemigos = 0.5f; // Tiempo entre la aparición de cada enemigo

    // Variables privadas
    private int currentEnemies = 0;
    private float nextSpawnTime;
    private bool juegoIniciado = false;
    private int oleadaActual = 0; // Número de la oleada actual
    private bool esperandoSiguienteOleada = false; // Para controlar el paso a la siguiente oleada
    private float multiplicadorVida = 1.0f; // Multiplicador de vida por oleada
    private bool primeraOleadaGenerada = false; // Controlar la primera oleada


    public static EnemySpawner Instance;

    void Start()
    {
        // Iniciar la espera para la primera oleada
        nextSpawnTime = Time.time + startDelay;
    }

    void Update()
    {
        // Iniciar el juego cuando el jugador se mueva
        if (!juegoIniciado && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            juegoIniciado = true;
        }

        // Generar la primera oleada solo después del delay y cuando el jugador se haya movido
        if (juegoIniciado && !primeraOleadaGenerada && Time.time >= nextSpawnTime)
        {
            StartCoroutine(GenerarOleadaEnemigosGradualmente()); // Usamos la corutina
            primeraOleadaGenerada = true;
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        // Verificar si todos los enemigos de la oleada actual fueron derrotados y esperar para la siguiente oleada
        if (currentEnemies <= 0 && juegoIniciado && !esperandoSiguienteOleada && primeraOleadaGenerada)
        {
            esperandoSiguienteOleada = true;
            oleadaActual++;
            multiplicadorVida *= 1.1f; // Aumentar la vida en un 10% por oleada

            // Mostrar el texto temporal de la oleada
            GameManager.Instance.MostrarTextoOleadaTemporal($" Round: {oleadaActual}");

            // Esperar 5 segundos antes de la siguiente oleada
            Invoke(nameof(ComenzarNuevaOleada), 5f);
        }
    }

    void ComenzarNuevaOleada()
    {
        esperandoSiguienteOleada = false;
        StartCoroutine(GenerarOleadaEnemigosGradualmente()); // Usamos la corutina
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval); // Definir tiempo hasta la siguiente oleada
    }

    IEnumerator GenerarOleadaEnemigosGradualmente()
    {
        int cantidadEnemigos = minEnemies + (oleadaActual * incrementoEnemigosPorOleada);

        // Actualizar el texto al inicio de la oleada
        GameManager.Instance.ActualizarTextoOleada($"{oleadaActual}");

        for (int i = 0; i < cantidadEnemigos; i++)
        {
            GenerarEnemigo();

            // Esperar un tiempo antes de generar el próximo enemigo
            yield return new WaitForSeconds(intervaloEntreEnemigos);
        }
    }

    void GenerarEnemigo()
    {
        if (enemyPrefab != null && spawnPoints.Length > 0)
        {
            // Seleccionar un punto de spawn aleatorio
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Instanciar el enemigo en el punto de spawn
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            // Verificar si el enemigo se instanció correctamente
            if (enemy != null)
            {
                Debug.Log($"Enemigo generado en {spawnPoint.position}");

                // Obtener el componente Enemy del enemigo generado
                Enemy enemyScript = enemy.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    // Aplicar multiplicador de vida
                    enemyScript.SetMultiplicadorVida(multiplicadorVida);
                }
                else
                {
                    Debug.LogError("El prefab del enemigo no tiene el componente Enemy.");
                }

                // Incrementar el contador de enemigos activos
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

        // Verificar si todos los enemigos de la oleada actual fueron derrotados
        if (currentEnemies <= 0 && !esperandoSiguienteOleada)
        {
            esperandoSiguienteOleada = true;
            oleadaActual++;
            multiplicadorVida *= 1.1f; // Aumentar la vida en un 10% por oleada

            // Mostrar el texto temporal de la oleada
            GameManager.Instance.MostrarTextoOleadaTemporal($"Ronda {oleadaActual}");

            // Esperar 5 segundos antes de la siguiente oleada
            Invoke(nameof(ComenzarNuevaOleada), 5f);
        }
    }
}
