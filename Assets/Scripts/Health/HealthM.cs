using UnityEngine;
using System.Collections;

public class HealthM : MonoBehaviour
{
    public static HealthM Instance; // Singleton para acceder al HealthM

    // Configuraci�n para las curas (Health)
    public GameObject healthPrefab; // Prefab de la cura
    public Transform[] healthSpawnPoints; // Puntos de generaci�n de curas
    public float healthMinSpawnTime = 10f; // Tiempo m�nimo entre generaciones de curas
    public float healthMaxSpawnTime = 20f; // Tiempo m�ximo entre generaciones de curas
    public float healthInitialDelay = 10f; // Retraso inicial antes de comenzar la generaci�n de curas
    private int currentHealthCount = 0; // N�mero actual de curas en el mapa
    private const int maxHealthCount = 2; // M�ximo de curas permitidas en el mapa

    // Configuraci�n para el segundo prefab (Power-up u otro item)
    public GameObject secondPrefab; // Prefab del segundo objeto (power-up, etc.)
    public Transform[] secondPrefabSpawnPoints; // Puntos de generaci�n del segundo prefab
    public float secondPrefabMinSpawnTime = 15f; // Tiempo m�nimo entre generaciones del segundo prefab
    public float secondPrefabMaxSpawnTime = 25f; // Tiempo m�ximo entre generaciones del segundo prefab
    public float secondPrefabInitialDelay = 15f; // Retraso inicial antes de comenzar la generaci�n del segundo prefab
    private int currentSecondPrefabCount = 0; // N�mero actual de segundos prefabs en el mapa
    private const int maxSecondPrefabCount = 1; // M�ximo de segundos prefabs permitidos en el mapa

    private bool playerHasMoved = false; // Indica si el jugador se ha movido

    private void Awake()
    {
        // Configurar el Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Iniciar la generaci�n de objetos despu�s de que el jugador se mueva
        StartCoroutine(WaitForPlayerMovement());
    }

    private IEnumerator WaitForPlayerMovement()
    {
        // Esperar a que el jugador se mueva
        yield return new WaitUntil(() => playerHasMoved);

        // Iniciar la generaci�n de curas y el segundo prefab
        StartCoroutine(StartHealthGenerationAfterDelay());
        StartCoroutine(StartSecondPrefabGenerationAfterDelay());
    }

    private IEnumerator StartHealthGenerationAfterDelay()
    {
        yield return new WaitForSeconds(healthInitialDelay); // Esperar antes de comenzar la generaci�n de curas
        StartCoroutine(HealthGenerationRoutine());
    }

    private IEnumerator StartSecondPrefabGenerationAfterDelay()
    {
        yield return new WaitForSeconds(secondPrefabInitialDelay); // Esperar antes de comenzar la generaci�n del segundo prefab
        StartCoroutine(SecondPrefabGenerationRoutine());
    }

    private IEnumerator HealthGenerationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(healthMinSpawnTime, healthMaxSpawnTime)); // Esperar un tiempo aleatorio

            if (currentHealthCount < maxHealthCount)
            {
                SpawnHealth();
            }
        }
    }

    private IEnumerator SecondPrefabGenerationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(secondPrefabMinSpawnTime, secondPrefabMaxSpawnTime)); // Esperar un tiempo aleatorio

            if (currentSecondPrefabCount < maxSecondPrefabCount)
            {
                SpawnSecondPrefab();
            }
        }
    }

    private void SpawnHealth()
    {
        if (healthSpawnPoints.Length == 0) return;

        Transform spawnPoint = healthSpawnPoints[Random.Range(0, healthSpawnPoints.Length)]; // Elegir un punto de generaci�n aleatorio
        Instantiate(healthPrefab, spawnPoint.position, Quaternion.identity); // Generar la cura
        currentHealthCount++;
    }

    private void SpawnSecondPrefab()
    {
        if (secondPrefabSpawnPoints.Length == 0) return;

        Transform spawnPoint = secondPrefabSpawnPoints[Random.Range(0, secondPrefabSpawnPoints.Length)]; // Elegir un punto de generaci�n aleatorio
        Instantiate(secondPrefab, spawnPoint.position, Quaternion.identity); // Generar el segundo prefab
        currentSecondPrefabCount++;
    }

    public void HealthPickedUp()
    {
        currentHealthCount--;
    }

    public void SecondPrefabPickedUp()
    {
        currentSecondPrefabCount--;
    }

    private void Update()
    {
        // Detectar si el jugador se ha movido
        if (!playerHasMoved && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            playerHasMoved = true;
        }
    }
}