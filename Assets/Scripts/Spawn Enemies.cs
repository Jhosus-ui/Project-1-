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
        // Implementación del Singleton
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
        // Iniciar la espera para la primera oleada
        Invoke(nameof(IniciarPrimeraOleada), startDelay);
    }

    void IniciarPrimeraOleada()
    {
        if (!juegoIniciado)
        {
            juegoIniciado = true;
        }

        // Iniciar la primera oleada
        StartCoroutine(GenerarOleadaEnemigosGradualmente());
    }

    IEnumerator GenerarOleadaEnemigosGradualmente()
    {
        int cantidadEnemigos = minEnemies + (oleadaActual * incrementoEnemigosPorOleada);

        // Notificar al GameManager para actualizar el texto de la oleada
        GameManager.Instance.ActualizarTextoOleada($"Round: {oleadaActual}");
        GameManager.Instance.MostrarTextoOleadaTemporal($"{oleadaActual}");

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

        // Verificar si todos los enemigos de la oleada actual fueron derrotados
        if (currentEnemies <= 0 && !esperandoSiguienteOleada)
        {
            esperandoSiguienteOleada = true;
            oleadaActual++;
            multiplicadorVida *= 1.1f; // Aumentar la vida en un 10% por oleada

            // Mostrar el texto temporal de la oleada
            GameManager.Instance.MostrarTextoOleadaTemporal($"Ronda {oleadaActual}");

            // Esperar 10 segundos antes de la siguiente oleada
            Invoke(nameof(ComenzarNuevaOleada), tiempoEntreOleadas);
        }
    }

    void ComenzarNuevaOleada()
    {
        esperandoSiguienteOleada = false;
        StartCoroutine(GenerarOleadaEnemigosGradualmente()); // Usamos la corutina
    }
}