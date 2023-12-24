using PlayerScripts;
using System.Collections;
using UnityEngine;

public class LevelSpawner : MonoSingleton<LevelSpawner>
{
    [SerializeField]
    private EnemyInfo[] enemySpawnData = new EnemyInfo[0];
    [SerializeField]
    private SpawnCycle[] enemySpawnCycles = new SpawnCycle[0];
    [SerializeField]
    private float timeUntilNewScene = 60.0f;
    [SerializeField]
    private string nextScene = "Scene1";

    // Start is called before the first frame update
    void Start()
    {
        // Coroutine to enforce a time limit for the level
        StartCoroutine(LevelTimeLimit());

        // Iterate through each SpawnCycle and spawn enemies accordingly
        foreach (SpawnCycle s in enemySpawnCycles)
        {
            for (int i = 0; i < s.timesToLoop; i++)
            {
                foreach (SpawnInfo e in s.spawnsInCycle)
                {
                    var spawnIndex = e.enemyIndex;
                    var spawnDelay = e.waitTime + (s.cycleDuration * i);

                    // Coroutine to spawn enemies after a specified delay
                    StartCoroutine(SpawnEnemyAfterDelay(spawnIndex, spawnDelay));
                }
            }
        }
    }

    private IEnumerator SpawnEnemyAfterDelay(int spawnIndex, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        var toEnable = enemySpawnData[spawnIndex].toEnable;
        // caches toEnable, for readability and convenience

        while (toEnable.activeSelf)
            yield return null;
        // If the spawner is trying to spawn an already enabled enemy
        // it will wait for it to be disabled

        toEnable.SetActive(true);
        toEnable.transform.position = enemySpawnData[spawnIndex].enableLocation;
        // Enables at the proper position 
    }

    // Coroutine to enforce a time limit for the level
    private IEnumerator LevelTimeLimit()
    {
        yield return new WaitForSeconds(timeUntilNewScene);
        ChangeLevel();
    }

    // Method to change levels, with an optional delay
    public void ChangeLevel(float delay = 0)
    {
        if (Watercat.current.PlayerDead)
            return;
        StopAllCoroutines();
        StartCoroutine(SwitchToNewScene(delay));
    }

    // Coroutine to switch to a new scene after a specified delay
    private IEnumerator SwitchToNewScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Stop spawn coroutine
        SceneTransitions.Instance.DoSceneTransition(nextScene);
    }

    // Method to force the spawning of a specific enemy with an optional delay
    public void ForceSpawn(int spawnIndex, float waitTime = 0.0f)
    {
        StartCoroutine(SpawnEnemyAfterDelay(spawnIndex, waitTime));
    }
}