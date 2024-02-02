using TMPro;
using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject specialEnemyPrefab;
    public float timeBetweenWaves = 5f;
    public TextMeshProUGUI waveCountText, waveTimerText, moneyText;
    public ParticleSystem startWaveParticle;
    public ParticleSystem enemySpawnParticle;
    public Animator crystalAnimator;

    private int currentWave = 0;
    private int money = 500;

    void Start()
    {
        StartCoroutine(InitialCountdown());
    }

    private void Update()
    {
        if (GameObject.FindGameObjectsWithTag("Zombie").Length == 0)
        {
            startWaveParticle.Play();
            crystalAnimator.SetBool("SpawnEnemies", true);
            waveTimerText.enabled = true;
        }
        else if (GameObject.FindGameObjectsWithTag("Zombie").Length != 0)
        {
            startWaveParticle.Stop();
            crystalAnimator.SetBool("SpawnEnemies", false);
            waveTimerText.enabled = false;
        }

        waveCountText.text = "Wave: " + (currentWave + 1);
        moneyText.text = "Money: " + money;
    }

    IEnumerator InitialCountdown()
    {
        float initialCountdownTimer = 5f;

        while (initialCountdownTimer > 0)
        {
            waveTimerText.enabled = true;
            waveTimerText.text = initialCountdownTimer.ToString();
            yield return new WaitForSeconds(1f);
            initialCountdownTimer -= 1f;
        }

        waveTimerText.enabled = false;

        if (startWaveParticle != null)
        {
            startWaveParticle.Stop();
        }

        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnEnemiesWithDelay()
    {
        int numberOfEnemies = (currentWave + 1) * 2;

        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject enemyToSpawn = enemyPrefab;

            enemySpawnParticle.Play();

            if (currentWave >= 10)
            {
                float specialEnemySpawnChance = 0.2f;
                if (Random.value < specialEnemySpawnChance)
                {
                    enemyToSpawn = specialEnemyPrefab;
                }
            }

            Vector2 randomCirclePosition = Random.insideUnitCircle;
            Vector3 spawnPosition = new Vector3(randomCirclePosition.x, 0f, randomCirclePosition.y) + transform.position;

            GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator SpawnWaves()
    {
        while (true)
        {
            waveTimerText.text = timeBetweenWaves.ToString();
            Debug.Log("Wave " + (currentWave + 1) + " started!");

            // Spawn the zombies for this wave
            StartCoroutine(SpawnEnemiesWithDelay());

            // Wait until all enemies are defeated
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Zombie").Length == 0);

            // Calculate money earned for the current wave
            int waveMoney = (currentWave == 0) ? 10 : Mathf.RoundToInt(10 * (1 + 0.1f * currentWave));

            money += waveMoney;

            Debug.Log("Earned " + waveMoney + " money for Wave " + (currentWave + 1));

            currentWave++;

            // Call InitialCountdown every time a wave is finished
            StartCoroutine(InitialCountdown());

            // Wait for the time between waves
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
}
