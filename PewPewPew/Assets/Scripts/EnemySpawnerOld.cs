using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerOld : MonoBehaviour {

    [SerializeField] List<WaveConfig> waveConfigs;
    [SerializeField] int startingWave = 0;
    [SerializeField] bool looping = false;

    // Use this for initialization

    IEnumerator Start ()
    {
        do
        {
            yield return StartCoroutine(SpawnAllWaves());
        }
        while (looping);
	}

    private IEnumerator SpawnAllWaves()
    {
        for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++)
        {
            var currentWave = waveConfigs[waveIndex];
            yield return StartCoroutine(SpawnAllEnemiesInWave(currentWave));
        }
    }

	private IEnumerator SpawnAllEnemiesInWave(WaveConfig waveConfig)
    {
        for (int enemyCount = 0; enemyCount < waveConfig.GetNumberOfEnemies(); enemyCount++)
        {
            Vector3 rotateOffset = new Vector3(transform.rotation.x + 180, transform.rotation.y, transform.rotation.z);
            Quaternion rotateOffsetQ = Quaternion.Euler(rotateOffset);
            var newEnemy = Instantiate(waveConfig.GetEnemyPrefab(), waveConfig.GetWaypoints()[0].transform.position, rotateOffsetQ);
            newEnemy.GetComponent<EnemyPathing>().SetWaveConfig(waveConfig);

            yield return new WaitForSeconds(waveConfig.GetTimeBetweenSpawns());
        }
    }

    public int EnemyCount()
    {
        int enemyCount = 0;
        for (int waveIndex = startingWave; waveIndex < waveConfigs.Count; waveIndex++)
        {
            var currentWave = waveConfigs[waveIndex];
            enemyCount += currentWave.GetNumberOfEnemies();
        }
        return enemyCount;
    }
}
