using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] List<GameObject> enemyPrefab;
    [SerializeField] List<GameObject> spawners;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject spawner in spawners)
            {
                int randomIndex = Random.Range(0, enemyPrefab.Count);
                SpawnEnemy(enemyPrefab[randomIndex]);
            }
        }
    }
    public void SpawnEnemy(GameObject enemyPrefab)
    {
        for (int i = 0; i < spawners.Count; i++)
        {
            Instantiate(enemyPrefab, spawners[i].transform.position, Quaternion.identity);
        }
    }
}
