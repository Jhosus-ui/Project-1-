using UnityEngine;
using System.Collections;

public class AmmoRespawner : MonoBehaviour
{
    public GameObject ammoBoxPrefab;
    public Transform[] spawnPoints;
    public float respawnTime = 10f;

    private bool hasPlayerMoved = false;
    private Vector3 lastPlayerPosition;

    private void Start()
    {
        lastPlayerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        InvokeRepeating("CheckPlayerMovement", 0f, 1f);
    }

    private void CheckPlayerMovement()
    {
        Vector3 currentPlayerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        if (currentPlayerPosition != lastPlayerPosition)
        {
            if (!hasPlayerMoved)
            {
                hasPlayerMoved = true;
                StartCoroutine(StartAmmoRespawn());
            }

            lastPlayerPosition = currentPlayerPosition;
        }
    }

    private IEnumerator StartAmmoRespawn()
    {
        while (true)
        {
            SpawnAmmoBox();

            yield return new WaitForSeconds(respawnTime);
        }
    }

    private void SpawnAmmoBox()
    {
        if (spawnPoints.Length > 0)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[randomIndex];

            // Verificar si hay algo en la posición de spawn
            if (!IsPositionOccupied(spawnPoint.position))
            {
                Instantiate(ammoBoxPrefab, spawnPoint.position, Quaternion.identity);
            }
        }
        else
        {
            Debug.LogError("No hay puntos de respawn asignados en el AmmoRespawner.");
        }
    }

    // Método para verificar si una posición está ocupada
    private bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 2f); // Radio 
        return colliders.Length > 0; // Si hay colisiones, la posición está ocupada
    }
}