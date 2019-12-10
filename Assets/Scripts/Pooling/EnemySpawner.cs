using System.Collections;
using Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    public GameManager.EnemyTags enemyTag;
    private IEnumerator spawnRoutine;

    private void Start()
    {
        spawnRoutine = SpawnEnemies();
    }

    public void StartSpawn()
    {
        StopSpawn();
        spawnRoutine = SpawnEnemies();
        StartCoroutine(spawnRoutine);
    }
    
    public void StopSpawn()
    {
        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);
    }

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitUntil(() => GameManager.Instance.currentGameState == GameManager.GameState.GAME);
        while (true)
        {
            if (GameManager.Instance.levelSlider.value >= 1 || GameManager.Instance.currentGameState != GameManager.GameState.GAME)
            {
                break;
            }
            yield return new WaitForSeconds(5f);

            enemyTag = Random.Range(0, 2) == 0 ? GameManager.EnemyTags.Light : GameManager.EnemyTags.Heavy;
            Vector3 distance = Camera.main.transform.position - transform.position;
            float angle = (Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg) - 90;
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(angle - 30, angle + 30), Vector3.forward);
            ObjectPooler.Instance.SpawnFromPool(enemyTag.ToString(), transform.position, rotation, GameManager.Instance.enemiesPlaceHolder);
        }
    }
}

