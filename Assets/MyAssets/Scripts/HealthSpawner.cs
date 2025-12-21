using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class HealthSpawner : MonoBehaviour
{
    [Header("Spawn Config")]
    [SerializeField] private GameObject healthPickupPrefab;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private int maxPickupsAlive = 3;

    [Header("Spawn Arean")] 
    [SerializeField] private Transform[] spawnPoints;

    private int _currentPickups;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (_currentPickups >= maxPickupsAlive)
                continue;

            SpawnPickup();
        }
    }

    private void SpawnPickup()
    {
        if (spawnPoints.Length == 0) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        
        GameObject pickup = Instantiate(healthPickupPrefab, point.position, Quaternion.identity);

        _currentPickups++;

        pickup.AddComponent<PickupTracker>().Init(this);
    }

    public void NotifyPickupDestroyed()
    {
        _currentPickups--;
    }
}
