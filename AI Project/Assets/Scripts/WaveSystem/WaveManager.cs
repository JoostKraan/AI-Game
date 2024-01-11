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

    private int currentWave = 0;

    void Start()
    {
        StartCoroutine(InitialCountdown());
    }

    IEnumerator InitialCountdown()
    {
        float initialCountdownTimer = 5f;

        // Start de Particle System bij het begin van de countdown
        if (startWaveParticle != null)
        {
            startWaveParticle.Play();
        }

        while (initialCountdownTimer > 0)
        {
            waveTimerText.enabled = true;
            waveTimerText.text = "Next Wave in: " + initialCountdownTimer.ToString(); // Toon de resterende tijd met één decimaal
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

            // Activeer de Particle System voor het spawnen van vijanden elke keer als een vijand verschijnt
            if (enemySpawnParticle != null)
            {
                ParticleSystem particle = Instantiate(enemySpawnParticle, spawnedEnemy.transform.position, Quaternion.identity);
                Destroy(particle.gameObject, particle.main.duration);
            }

            UpdateWaveCountText();

            // Wacht met spawnen tot de volgende seconde
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator SpawnWaves()
    {
        while (true) // Oneindige loop
        {
            UpdateWaveCountText();
            waveTimerText.text = "Next Wave in: " + timeBetweenWaves.ToString();
            Debug.Log("Wave " + (currentWave + 1) + " started!");

            // Spawn de zombies voor deze golf
            StartCoroutine(SpawnEnemiesWithDelay());

            // Wacht totdat alle vijanden zijn verslagen
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Zombie").Length == 0);

            // Activeer de Particle System voor het starten van de wave aan het einde van elke wave
            if (startWaveParticle != null)
            {
                startWaveParticle.Play();
            }

            waveTimerText.enabled = false;

            // Stop de Particle System voor het starten van de wave wanneer de countdown voor de volgende wave begint
            if (startWaveParticle != null)
            {
                startWaveParticle.Stop();
            }

            currentWave++;

            // Wacht op de tussenliggende timer tussen golven
            yield return new WaitForSeconds(timeBetweenWaves);

            // Verplaats deze regel buiten de inner loop om te voorkomen dat de tekst wordt uitgeschakeld voordat de volgende wave begint
            if (timeBetweenWaves <= 0) waveTimerText.enabled = false;
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
