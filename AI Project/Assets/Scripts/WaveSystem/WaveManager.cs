using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject specialEnemyPrefab;
    public float timeBetweenWaves = 5f;
    public TextMeshProUGUI waveCountText;

    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        while (true) // Oneindige loop
        {
            UpdateWaveCountText();
            Debug.Log("Wave " + (currentWave + 1) + " started!");

            SpawnEnemies();

            // Wacht totdat alle vijanden zijn verslagen
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Zombie").Length == 0);

            yield return new WaitForSeconds(timeBetweenWaves);

            currentWave++;
        }
    }

    void SpawnEnemies()
    {
        int numberOfEnemies = (currentWave + 1) * 2;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject enemyToSpawn = enemyPrefab;

            // Check voor specialEnemyPrefab vanaf wave 10
            if (currentWave >= 10)
            {
                float specialEnemySpawnChance = 0.2f;
                if (Random.value < specialEnemySpawnChance)
                {
                    enemyToSpawn = specialEnemyPrefab;
                }
            }

            Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));

            Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);

            UpdateWaveCountText();
        }
    }

    void UpdateWaveCountText()
    {
        if (waveCountText != null)
        {
            waveCountText.text = "Wave: " + (currentWave + 1);
        }
    }
}
