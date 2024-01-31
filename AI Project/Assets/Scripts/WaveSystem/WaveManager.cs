using System.Collections;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject specialEnemyPrefab;
    public float timeBetweenWaves = 5f;
    public TextMeshProUGUI waveCountText, waveTimerText;
    public ParticleSystem startWaveParticle; // Particle System voor het starten van de wave
    public ParticleSystem enemySpawnParticle; // Nieuwe Particle System voor het spawnen van vijanden
    public Animator crystalAnimator;

    private int currentWave = 0;

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
    }

	IEnumerator InitialCountdown()
    {
        float initialCountdownTimer = 5f;

        while (initialCountdownTimer > 0)
        {
            waveTimerText.enabled = true;
            waveTimerText.text = initialCountdownTimer.ToString(); // Toon de resterende tijd met één decimaal
            yield return new WaitForSeconds(1f);
            initialCountdownTimer -= 1f;
        }

        waveTimerText.enabled = false;

        // Stop de Particle System wanneer de countdown is voltooid
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

            // Check voor specialEnemyPrefab vanaf wave 10
            if (currentWave >= 10)
            {
                float specialEnemySpawnChance = 0.2f;
                if (Random.value < specialEnemySpawnChance)
                {
                    enemyToSpawn = specialEnemyPrefab;
                }
            }

            // Genereer een willekeurige spawnpositie binnen de eenheidscirkel
            Vector2 randomCirclePosition = Random.insideUnitCircle;

            // Pas de spawnpositie aan met de positie van de WaveManager
            Vector3 spawnPosition = new Vector3(randomCirclePosition.x, 0f, randomCirclePosition.y) + transform.position;

            // Instantieer de vijand
            GameObject spawnedEnemy = Instantiate(enemyToSpawn, spawnPosition, Quaternion.identity);

            // Wacht met spawnen tot de volgende seconde
            yield return new WaitForSeconds(1f);
        }
    }


    IEnumerator SpawnWaves()
    {
        while (true) // Infinite loop
        {
            waveTimerText.text = timeBetweenWaves.ToString();
            Debug.Log("Wave " + (currentWave + 1) + " started!");

            // Spawn the zombies for this wave
            StartCoroutine(SpawnEnemiesWithDelay());

            // Wait until all enemies are defeated
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Zombie").Length == 0);

            currentWave++;

            // Call InitialCountdown every time a wave is finished
            StartCoroutine(InitialCountdown());

            // Wait for the time between waves
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
}
