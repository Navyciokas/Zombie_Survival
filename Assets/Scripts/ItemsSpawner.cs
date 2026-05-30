using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemsSpawner : MonoBehaviour
{
    [Header("Item Prefabs")]
    public GameObject medkitPrefab;
    public GameObject pistolMagPrefab;
    public GameObject arMagPrefab;

    [Header("Player")]
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnInterval = 10f;
    public float minSpawnDistanceFromPlayer = 8f;
    public float maxSpawnDistanceFromPlayer = 35f;
    public float navMeshSampleRadius = 10f;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnRandomItem();
        }
    }

    void SpawnRandomItem()
    {
        Vector3 spawnPosition;

        if (!GetRandomSpawnPosition(out spawnPosition))
        {
            Debug.LogWarning("Could not find item spawn position.");
            return;
        }

        GameObject itemPrefab = GetRandomItemPrefab();

        Instantiate(
            itemPrefab,
            spawnPosition + Vector3.up * 0.3f,
            Quaternion.identity
        );
    }

    GameObject GetRandomItemPrefab()
    {
        int random = Random.Range(0, 3);

        if (random == 0)
            return medkitPrefab;

        if (random == 1)
            return pistolMagPrefab;

        return arMagPrefab;
    }

    bool GetRandomSpawnPosition(out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(
                minSpawnDistanceFromPlayer,
                maxSpawnDistanceFromPlayer
            );

            Vector3 randomPoint = player.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, navMeshSampleRadius, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}
