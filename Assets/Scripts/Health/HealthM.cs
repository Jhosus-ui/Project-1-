using UnityEngine;
using System.Collections;

public class HealthM : MonoBehaviour
{
    public static HealthM Instance;

    public GameObject healthPrefab;
    public Transform[] healthSpawnPoints;
    public float healthMinSpawnTime = 10f; //Tiempo Minimo
    public float healthMaxSpawnTime = 20f; // T. MAximo 
    public float healthInitialDelay = 10f; 
    private int currentHealthCount = 0;
    private const int maxHealthCount = 2;

    public GameObject secondPrefab;
    public Transform[] secondPrefabSpawnPoints;
    public float secondPrefabMinSpawnTime = 15f; //Tiempo Minimo
    public float secondPrefabMaxSpawnTime = 25f; // T. MAximo 
    public float secondPrefabInitialDelay = 15f;
    private int currentSecondPrefabCount = 0;
    private const int maxSecondPrefabCount = 1;

    private bool playerHasMoved = false;

    private void Awake()
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

    private void Start()
    {
        StartCoroutine(WaitForPlayerMovement());
    }

    private IEnumerator WaitForPlayerMovement()
    {
        yield return new WaitUntil(() => playerHasMoved);
        StartCoroutine(StartHealthGenerationAfterDelay());
        StartCoroutine(StartSecondPrefabGenerationAfterDelay());
    }

    private IEnumerator StartHealthGenerationAfterDelay()
    {
        yield return new WaitForSeconds(healthInitialDelay);
        StartCoroutine(HealthGenerationRoutine());
    }

    private IEnumerator StartSecondPrefabGenerationAfterDelay()
    {
        yield return new WaitForSeconds(secondPrefabInitialDelay);
        StartCoroutine(SecondPrefabGenerationRoutine());
    }

    private IEnumerator HealthGenerationRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(healthMinSpawnTime, healthMaxSpawnTime));
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
            yield return new WaitForSeconds(Random.Range(secondPrefabMinSpawnTime, secondPrefabMaxSpawnTime));
            if (currentSecondPrefabCount < maxSecondPrefabCount)
            {
                SpawnSecondPrefab();
            }
        }
    }

    private void SpawnHealth()
    {
        if (healthSpawnPoints.Length == 0) return;

        Transform spawnPoint = healthSpawnPoints[Random.Range(0, healthSpawnPoints.Length)];
        if (!IsPositionOccupied(spawnPoint.position)) // Para la posici�n est� ocupada
        {
            Instantiate(healthPrefab, spawnPoint.position, Quaternion.identity);
            currentHealthCount++;
        }
    }

    private void SpawnSecondPrefab()
    {
        if (secondPrefabSpawnPoints.Length == 0) return;

        Transform spawnPoint = secondPrefabSpawnPoints[Random.Range(0, secondPrefabSpawnPoints.Length)];
        if (!IsPositionOccupied(spawnPoint.position)) // Para la posici�n est� ocupada
        {
            Instantiate(secondPrefab, spawnPoint.position, Quaternion.identity);
            currentSecondPrefabCount++;
        }
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
        if (!playerHasMoved && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            playerHasMoved = true;
        }
    }
    private bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 5f); // Radio 
        return colliders.Length > 0; 
    }
}