using UnityEngine;
using UnityEngine.Networking;

public class EnemySpawner : NetworkBehaviour
{

    public GameObject enemyPrefab;
    public int numberOfEnemies;

    void Start()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            var spawnPosition = gameObject.transform.position + new Vector3(Random.Range(-8.0f, 8.0f), 0.0f, Random.Range(-8.0f, 8.0f));

            var spawnRotation = Quaternion.Euler(0.0f, Random.Range(0, 180), 0.0f);

            var enemy = (GameObject)Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            NetworkServer.Spawn(enemy);
            
        }
    }
}