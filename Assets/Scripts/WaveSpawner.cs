using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [Header("Zombie Prefabs")]
    public GameObject zombiePrefab1;
    public GameObject zombiePrefab2;

    [Header("Player")]
    public Transform player;

    [Header("Random Spawn Settings")]
    public float minSpawnDistanceFromPlayer = 15f;
    public float maxSpawnDistanceFromPlayer = 40f;
    public float navMeshSampleRadius = 10f;

    [Header("Wave Settings")]
    public int currentWave = 1;
    public int zombiesAlive = 0;

    public int baseZombieCount = 3;
    public int zombiesPerWaveIncrease = 2;

    public float spawnDelay = 0.5f;
    public float timeBetweenWaves = 3f;

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text DeathMsgText;

    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    void Update()
    {
        UpdateUI();

        if (!isSpawning && zombiesAlive <= 0)
        {
            StartCoroutine(StartNextWave());
        }
    }

    IEnumerator StartNextWave()
    {
        isSpawning = true;

        yield return new WaitForSeconds(timeBetweenWaves);

        int zombiesToSpawn = baseZombieCount + (currentWave - 1) * zombiesPerWaveIncrease;

        for (int i = 0; i < zombiesToSpawn; i++)
        {
            SpawnZombie();
            yield return new WaitForSeconds(spawnDelay);
        }

        currentWave++;
        isSpawning = false;
    }

    void SpawnZombie()
    {
        Vector3 spawnPosition;

        if (!GetRandomSpawnPosition(out spawnPosition))
        {
            Debug.LogWarning("Could not find valid zombie spawn position.");
            return;
        }

        GameObject prefabToSpawn;

        if (currentWave < 3)
        {
            prefabToSpawn = zombiePrefab1;
        }
        else
        {
            prefabToSpawn = Random.value > 0.5f ? zombiePrefab1 : zombiePrefab2;
        }

        GameObject zombie = Instantiate(
            prefabToSpawn,
            spawnPosition,
            Quaternion.identity
        );

        zombiesAlive++;

        ZombieHealth zombieHealth = zombie.GetComponent<ZombieHealth>();

        if (zombieHealth != null)
        {
            zombieHealth.player = player;
            zombieHealth.waveSpawner = this;

            zombieHealth.health += currentWave * 10;
            zombieHealth.attackDamage += currentWave * 2;
        }
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

    public void ZombieDied()
    {
        zombiesAlive--;
    }

    void UpdateUI()
    {
        if (waveText != null)
            waveText.text = "Wave: " + currentWave;
        if (DeathMsgText != null)
            DeathMsgText.text = $"You were Eaten on {currentWave} wave!";
    }
}